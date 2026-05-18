using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.SoundVis.Objects;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.SoundVis.UI
{
    public partial class SoundVisPlayfield : Playfield
    {
        private SoundVisLogoDisplay logoDisplay = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;

            AddInternal(new Container
            {
                RelativeSizeAxes = Axes.Both,
                Depth = 1, // behind hit objects
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
                    logoDisplay = new SoundVisLogoDisplay
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                    },
                }
            });
        }

        public override void Add(DrawableHitObject h)
        {
            base.Add(h);

            if (h is DrawableSoundVisHitObject dh)
            {
                // Move the logo to the new target position when the object enters
                logoDisplay.MoveToNormalisedPosition(dh.HitObject.Position);
                // Position the invisible hit zone
                UpdateHitObjectPosition(dh);
            }
        }

        protected override void Update()
        {
            base.Update();

            foreach (var drawable in HitObjectContainer.AliveObjects)
            {
                if (drawable is DrawableSoundVisHitObject dh)
                    UpdateHitObjectPosition(dh);
            }
        }

        private void UpdateHitObjectPosition(DrawableSoundVisHitObject dh)
        {
            dh.Anchor = Anchor.TopLeft;
            dh.Origin = Anchor.Centre;
            dh.X = dh.HitObject.Position.X * DrawWidth;
            dh.Y = dh.HitObject.Position.Y * DrawHeight;
        }
    }
}
