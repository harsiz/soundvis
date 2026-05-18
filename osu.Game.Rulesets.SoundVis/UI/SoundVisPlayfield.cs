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

            // Background layer (bars + now playing)
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

            // Logo display — uses RelativePositionAxes so MoveTo(0.8, 0.3) means
            // 80% across, 30% down the playfield. Anchor TopLeft + Origin Centre
            // means position = logo centre in relative space.
            logoDisplay = new SoundVisLogoDisplay
            {
                Anchor = Anchor.TopLeft,
                Origin = Anchor.Centre,
                RelativePositionAxes = Axes.Both,
                Position = new osuTK.Vector2(0.5f, 0.5f),
            };
            AddInternal(logoDisplay);
        }

        public override void Add(DrawableHitObject h)
        {
            base.Add(h);

            if (h is DrawableSoundVisHitObject dh)
            {
                // Glide the logo to the new target position
                logoDisplay.MoveToNormalisedPosition(dh.HitObject.Position);

                // Wire hover feedback: when cursor is on the hit zone, brighten the logo
                dh.HoverStateChanged += logoDisplay.SetHovered;
            }
        }

        protected override void Update()
        {
            base.Update();

            // Position each alive hit zone to match the logo's current normalised target.
            // Using absolute pixels (DrawWidth/DrawHeight) avoids needing RelativePositionAxes
            // inside HitObjectContainer.
            foreach (var drawable in HitObjectContainer.AliveObjects)
            {
                if (drawable is DrawableSoundVisHitObject dh)
                {
                    dh.Anchor = Anchor.TopLeft;
                    dh.Origin = Anchor.Centre;
                    dh.X = dh.HitObject.Position.X * DrawWidth;
                    dh.Y = dh.HitObject.Position.Y * DrawHeight;
                }
            }
        }
    }
}
