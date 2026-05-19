using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.SoundVis.Configuration;
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

        // ── Next-note indicator ───────────────────────────────────────────────────
        private CircularContainer nextNoteRing = null!;
        private SoundVisAction?   lastIndicatorAction;
        private readonly BindableBool showNextNote = new BindableBool(true);

        [Resolved(CanBeNull = true)]
        private SoundVisRulesetConfigManager? rulesetConfig { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;

            // ── Background beat flashes (behind everything) ───────────────────────
            AddInternal(new BeatFlashDisplay
            {
                RelativeSizeAxes = Axes.Both,
                Depth            = 2,
            });

            // ── Background (freq bars + now-playing) ──────────────────────────────
            AddInternal(new Container
            {
                RelativeSizeAxes = Axes.Both,
                Depth            = 1,
                Children = new Drawable[]
                {
                    new MusicVisualizerDisplay
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor           = Anchor.Centre,
                        Origin           = Anchor.Centre,
                    },
                    new NowPlayingPanel
                    {
                        Anchor = Anchor.TopLeft,
                        Origin = Anchor.TopLeft,
                        Margin = new MarginPadding(20),
                    },
                },
            });

            // ── Next-note indicator ring (sits just behind the logo) ──────────────
            // Transparent interior + coloured border that changes with the next note.
            nextNoteRing = new CircularContainer
            {
                Size            = new Vector2(SoundVisLogoDisplay.LOGO_RADIUS * 2 + 28f),
                Anchor          = Anchor.Centre,
                Origin          = Anchor.Centre,
                Masking         = true,
                BorderThickness = 5f,
                BorderColour    = Color4.White,
                Alpha           = 0f,
                Depth           = 0.5f,
                Child           = new Box { RelativeSizeAxes = Axes.Both, Alpha = 0, AlwaysPresent = true },
            };
            AddInternal(nextNoteRing);

            // ── Logo ──────────────────────────────────────────────────────────────
            logoDisplay = new SoundVisLogoDisplay
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            };
            AddInternal(logoDisplay);

            // ── Corner key-press feedback dots ────────────────────────────────────
            float ind         = SoundVisLogoDisplay.LOGO_RADIUS + 28f;
            var feedbackLayer = new Container
            {
                Anchor       = Anchor.Centre,
                Origin       = Anchor.Centre,
                AutoSizeAxes = Axes.Both,
            };

            foreach (SoundVisAction action in Enum.GetValues<SoundVisAction>())
            {
                var dot = new CircularContainer
                {
                    Size         = new Vector2(18),
                    Anchor       = Anchor.Centre,
                    Origin       = Anchor.Centre,
                    Position     = CornerOffset(action, ind),
                    Masking      = true,
                    Alpha        = 0,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour           = SoundVisActionHelper.GetColour(action),
                        },
                    },
                };
                cornerIndicators[action] = dot;
                feedbackLayer.Add(dot);
            }
            AddInternal(feedbackLayer);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            rulesetConfig?.BindWith(SoundVisRulesetSetting.ShowNextNoteIndicator, showNextNote);
            showNextNote.BindValueChanged(v => { if (!v.NewValue) nextNoteRing.FadeOut(200); }, true);

            ShowStartOverlay();
        }

        // ── Update: drive the next-note indicator ─────────────────────────────────

        protected override void Update()
        {
            base.Update();
            UpdateNextNoteIndicator();
        }

        private void UpdateNextNoteIndicator()
        {
            if (!showNextNote.Value)
                return;

            // Find the soonest upcoming unjudged note
            SoundVisHitObject? next = null;
            double             bestTimeToHit = double.MaxValue;

            foreach (var drawable in HitObjectContainer.AliveObjects)
            {
                if (drawable is DrawableSoundVisHitObject dh && !dh.Judged)
                {
                    double timeToHit = dh.HitObject.StartTime - Time.Current;
                    if (timeToHit > -80 && timeToHit < bestTimeToHit)
                    {
                        bestTimeToHit = timeToHit;
                        next          = dh.HitObject;
                    }
                }
            }

            if (next == null)
            {
                nextNoteRing.FadeOut(250, Easing.OutQuint);
                lastIndicatorAction = null;
                return;
            }

            // Update border colour on action change.
            // BorderColour is ColourInfo at runtime so TransformTo won't work;
            // set it directly — it only fires on action change, not every frame.
            if (next.RequiredAction != lastIndicatorAction)
            {
                lastIndicatorAction = next.RequiredAction;
                nextNoteRing.BorderColour = SoundVisActionHelper.GetColour(next.RequiredAction);
            }

            // Alpha: fade in from 800 ms out, brighten in the last 150 ms
            const double FADE_WINDOW  = 800;
            const double PULSE_WINDOW = 150;

            float baseAlpha  = (float)Math.Clamp((FADE_WINDOW - bestTimeToHit) / FADE_WINDOW, 0, 1);
            float pulseBoost = bestTimeToHit < PULSE_WINDOW
                ? (float)Math.Pow(1.0 - bestTimeToHit / PULSE_WINDOW, 1.5) * 0.55f
                : 0f;

            nextNoteRing.Alpha = 0.25f * baseAlpha + pulseBoost;

            // Ring thickness: grows as the note approaches
            nextNoteRing.BorderThickness = 4f + 5f * (float)Math.Clamp(1.0 - bestTimeToHit / 400.0, 0, 1);
        }

        // ── Key assignment overlay shown at map start ─────────────────────────────

        private void ShowStartOverlay()
        {
            var overlay = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Alpha            = 0,
            };

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
            DrawableSoundVisHitObject? best       = null;
            double                     bestOffset = double.MaxValue;

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
                        best       = dh;
                    }
                }
            }

            FlashCorner(e.Action, best != null);
            best?.TriggerResult();
            return true;
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
               .ScaleTo(1f,   250, Easing.OutQuint)
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

        // ── Inner drawable: corner key label shown at map start ───────────────────

        private partial class KeyCornerLabel : FillFlowContainer
        {
            private readonly SoundVisAction action;

            public KeyCornerLabel(SoundVisAction action)
            {
                this.action  = action;
                AutoSizeAxes = Axes.Both;
                Direction    = FillDirection.Vertical;
                Spacing      = new Vector2(0, 4);
            }

            [BackgroundDependencyLoader]
            private void load()
            {
                var    colour = SoundVisActionHelper.GetColour(action);
                string key    = SoundVisActionHelper.GetKeyLabel(action);

                Children = new Drawable[]
                {
                    new Container
                    {
                        Size         = new Vector2(36),
                        Anchor       = Anchor.TopCentre,
                        Origin       = Anchor.TopCentre,
                        Masking      = true,
                        CornerRadius = 6,
                        Children = new Drawable[]
                        {
                            new Box { RelativeSizeAxes = Axes.Both, Colour = colour, Alpha = 0.85f },
                        },
                    },
                    new OsuSpriteText
                    {
                        Text   = key,
                        Font   = OsuFont.GetFont(size: 22, weight: FontWeight.Bold),
                        Colour = colour,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                    },
                };
            }
        }
    }
}
