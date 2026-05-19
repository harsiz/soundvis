using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.SoundVis.Objects;
using osu.Game.Rulesets.UI;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.SoundVis.UI
{
    public partial class SoundVisPlayfield : Playfield, IKeyBindingHandler<SoundVisAction>
    {
        private SoundVisLogoDisplay logoDisplay = null!;
        private readonly Dictionary<SoundVisAction, CircularContainer> cornerIndicators = new();

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;

            // ── Background (bars + now playing) ──────────────────────────────────
            AddInternal(new Container
            {
                RelativeSizeAxes = Axes.Both,
                Depth = 1,
                Children = new Drawable[]
                {
                    new MusicVisualizerDisplay
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                    },
                    new NowPlayingPanel
                    {
                        Anchor = Anchor.TopLeft,
                        Origin = Anchor.TopLeft,
                        Margin = new MarginPadding(20),
                    },
                }
            });

            // ── Logo ─────────────────────────────────────────────────────────────
            logoDisplay = new SoundVisLogoDisplay
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            };
            AddInternal(logoDisplay);

            // ── Corner key-press feedback (around the logo) ───────────────────────
            float ind = SoundVisLogoDisplay.LOGO_RADIUS + 28f;
            var feedbackLayer = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                AutoSizeAxes = Axes.Both,
            };

            foreach (SoundVisAction action in Enum.GetValues<SoundVisAction>())
            {
                var dot = new CircularContainer
                {
                    Size = new Vector2(18),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Position = CornerOffset(action, ind),
                    Masking = true,
                    Alpha = 0,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = SoundVisActionHelper.GetColour(action),
                        }
                    }
                };
                cornerIndicators[action] = dot;
                feedbackLayer.Add(dot);
            }
            AddInternal(feedbackLayer);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            ShowStartOverlay();
        }

        // ── Key assignment overlay shown at map start ─────────────────────────────

        private void ShowStartOverlay()
        {
            var overlay = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0,
            };

            // Place a label at each screen corner
            foreach (SoundVisAction action in Enum.GetValues<SoundVisAction>())
            {
                var anchor = CornerAnchor(action);
                overlay.Add(new KeyCornerLabel(action)
                {
                    Anchor = anchor,
                    Origin = anchor,
                    Margin = new MarginPadding(36),
                });
            }

            AddInternal(overlay);
            overlay.FadeIn(300).Delay(2800).FadeOut(700).OnComplete(d => d.Expire());
        }

        // ── Hit-object lifecycle ──────────────────────────────────────────────────

        public override void Add(DrawableHitObject h)
        {
            base.Add(h);

            if (h is DrawableSoundVisHitObject dh)
            {
                dh.Anchor = Anchor.Centre;
                dh.Origin = Anchor.Centre;

                dh.OnNewResult += (_, result) =>
                {
                    if (result.IsHit)
                        logoDisplay.ReverseSpinDirection();
                };
            }
        }

        // ── Key handling ──────────────────────────────────────────────────────────

        public bool OnPressed(KeyBindingPressEvent<SoundVisAction> e)
        {
            // Find the alive hit object matching this action with the smallest time offset
            DrawableSoundVisHitObject? best = null;
            double bestOffset = double.MaxValue;

            foreach (var drawable in HitObjectContainer.AliveObjects)
            {
                if (drawable is DrawableSoundVisHitObject dh
                    && !dh.Judged
                    && dh.HitObject.RequiredAction == e.Action)
                {
                    double offset = Math.Abs(dh.Time.Current - dh.HitObject.StartTime);
                    if (offset < bestOffset)
                    {
                        bestOffset = offset;
                        best = dh;
                    }
                }
            }

            bool isHit = best != null;
            FlashCorner(e.Action, isHit);

            if (isHit)
                best!.TriggerResult();

            return true; // always consume so osu! doesn't route it elsewhere
        }

        public void OnReleased(KeyBindingReleaseEvent<SoundVisAction> e) { }

        // ── Corner helpers ────────────────────────────────────────────────────────

        private void FlashCorner(SoundVisAction action, bool hit)
        {
            if (!cornerIndicators.TryGetValue(action, out var dot)) return;

            var col = hit ? SoundVisActionHelper.GetColour(action) : Color4.Red;
            ((Box)dot.Children[0]).Colour = col;
            dot.ScaleTo(1.4f, 30, Easing.OutQuint)
               .FadeTo(0.95f, 20)
               .Then()
               .ScaleTo(1f, 250, Easing.OutQuint)
               .FadeOut(250);
        }

        private static Vector2 CornerOffset(SoundVisAction action, float dist) => action switch
        {
            SoundVisAction.TopLeft     => new Vector2(-dist, -dist),
            SoundVisAction.TopRight    => new Vector2( dist, -dist),
            SoundVisAction.BottomLeft  => new Vector2(-dist,  dist),
            SoundVisAction.BottomRight => new Vector2( dist,  dist),
            _                          => Vector2.Zero,
        };

        private static Anchor CornerAnchor(SoundVisAction action) => action switch
        {
            SoundVisAction.TopLeft     => Anchor.TopLeft,
            SoundVisAction.TopRight    => Anchor.TopRight,
            SoundVisAction.BottomLeft  => Anchor.BottomLeft,
            SoundVisAction.BottomRight => Anchor.BottomRight,
            _                          => Anchor.Centre,
        };

        // ── Inner drawable: corner key label ──────────────────────────────────────

        private partial class KeyCornerLabel : FillFlowContainer
        {
            private readonly SoundVisAction action;

            public KeyCornerLabel(SoundVisAction action)
            {
                this.action = action;
                AutoSizeAxes = Axes.Both;
                Direction = FillDirection.Vertical;
                Spacing = new Vector2(0, 4);
            }

            [BackgroundDependencyLoader]
            private void load()
            {
                var colour = SoundVisActionHelper.GetColour(action);
                string key = SoundVisActionHelper.GetKeyLabel(action);

                Children = new Drawable[]
                {
                    // Coloured square swatch
                    new Container
                    {
                        Size = new Vector2(36),
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Masking = true,
                        CornerRadius = 6,
                        Children = new Drawable[]
                        {
                            new Box { RelativeSizeAxes = Axes.Both, Colour = colour, Alpha = 0.85f },
                        }
                    },
                    // Key label
                    new OsuSpriteText
                    {
                        Text = key,
                        Font = OsuFont.GetFont(size: 22, weight: FontWeight.Bold),
                        Colour = colour,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                    },
                };
            }
        }
    }
}
