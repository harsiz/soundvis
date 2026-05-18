using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.SoundVis.Objects;
using osu.Game.Rulesets.UI;
using osuTK;

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
                Anchor = Anchor.TopLeft,
                Origin = Anchor.Centre,
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(0.5f, 0.5f),
            };
            AddInternal(logoDisplay);
        }

        public override void Add(DrawableHitObject h)
        {
            base.Add(h);

            if (h is DrawableSoundVisHitObject dh)
                logoDisplay.MoveToNormalisedPosition(dh.HitObject.Position);
        }

        protected override void Update()
        {
            base.Update();

            // Get mouse position in this playfield's local coordinate space.
            var inputManager = GetContainingInputManager();
            if (inputManager == null) return;

            var mouseLocal = ToLocalSpace(inputManager.CurrentState.Mouse.Position);

            // Logo visual centre in playfield pixels — read the animated X/Y directly
            // (these update each frame as the MoveTo transform runs).
            var logoCentre = new Vector2(logoDisplay.X * DrawWidth, logoDisplay.Y * DrawHeight);

            float radius = SoundVisLogoDisplay.LOGO_RADIUS;
            bool isOver = (mouseLocal - logoCentre).LengthSquared <= radius * radius;

            logoDisplay.SetHovered(isOver);

            foreach (var drawable in HitObjectContainer.AliveObjects)
            {
                if (drawable is DrawableSoundVisHitObject dh)
                    dh.MouseIsOverLogo = isOver;
            }
        }
    }
}
