using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.SoundVis.Objects;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.SoundVis.UI
{
    public partial class SoundVisPlayfield : Playfield, IKeyBindingHandler<SoundVisAction>
    {
        private SoundVisLogoDisplay logoDisplay = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;

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

            logoDisplay = new SoundVisLogoDisplay
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            };
            AddInternal(logoDisplay);
        }

        public override void Add(DrawableHitObject h)
        {
            base.Add(h);

            if (h is DrawableSoundVisHitObject dh)
            {
                dh.Anchor = Anchor.Centre;
                dh.Origin = Anchor.Centre;
                dh.OnNewResult += (_, result) =>
                {
                    if (result.Type == Scoring.HitResult.Great)
                        logoDisplay.ReverseSpinDirection();
                };
            }
        }

        public bool OnPressed(KeyBindingPressEvent<SoundVisAction> e)
        {
            if (e.Action != SoundVisAction.Hit) return false;

            // Find the alive hit object with the smallest absolute time offset
            DrawableSoundVisHitObject? best = null;
            double bestOffset = double.MaxValue;

            foreach (var drawable in HitObjectContainer.AliveObjects)
            {
                if (drawable is DrawableSoundVisHitObject dh && !dh.Judged)
                {
                    double offset = Math.Abs(dh.Time.Current - dh.HitObject.StartTime);
                    if (offset < bestOffset)
                    {
                        bestOffset = offset;
                        best = dh;
                    }
                }
            }

            if (best != null)
            {
                best.TriggerResult();
                return true;
            }

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<SoundVisAction> e) { }
    }
}
