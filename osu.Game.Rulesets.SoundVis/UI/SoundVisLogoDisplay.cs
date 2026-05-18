using System;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics.Containers;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.SoundVis.UI
{
    public partial class SoundVisLogoDisplay : BeatSyncedContainer
    {
        public const float LOGO_RADIUS = 80f;

        private float baseRotationDegsPerMs = 360f / 4000f;
        private float speedMultiplier = 1f;

        private Container logoContainer = null!;
        private Box hoverGlow = null!;

        [Resolved(CanBeNull = true)]
        private Bindable<WorkingBeatmap>? beatmap { get; set; }

        [BackgroundDependencyLoader]
        private void load(GameHost host)
        {
            Size = new Vector2(LOGO_RADIUS * 2);

            Sprite logoSprite;

            logoContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = LOGO_RADIUS,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = new Color4(20, 20, 30, 255),
                    },
                    logoSprite = new Sprite
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        FillMode = FillMode.Fit,
                        RelativeSizeAxes = Axes.Both,
                    },
                    hoverGlow = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.White,
                        Alpha = 0,
                        Blending = BlendingParameters.Additive,
                    },
                },
            };

            var resources = new DllResourceStore(typeof(SoundVisRuleset).Assembly);
            var byteStore = new NamespacedResourceStore<byte[]>(resources, "Resources");
            using var textureStore = new TextureStore(
                host.Renderer,
                new TextureLoaderStore(byteStore));
            logoSprite.Texture = textureStore.Get("Textures/lazer-logo");

            InternalChild = logoContainer;
        }

        protected override void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes)
        {
            base.OnNewBeat(beatIndex, timingPoint, effectPoint, amplitudes);

            baseRotationDegsPerMs = (float)(360.0 / (timingPoint.BeatLength * 8));
            speedMultiplier = 2.8f;

            logoContainer
                .ScaleTo(1.12f, 55, Easing.OutQuint)
                .Then()
                .ScaleTo(1f, timingPoint.BeatLength * 0.88, Easing.OutQuint);
        }

        protected override void Update()
        {
            base.Update();

            float elapsed = (float)Clock.ElapsedFrameTime;
            speedMultiplier = MathF.Max(1f, speedMultiplier - 0.003f * elapsed);
            logoContainer.Rotation += baseRotationDegsPerMs * speedMultiplier * elapsed;
        }

        public void SetHovered(bool hovered)
        {
            hoverGlow.FadeTo(hovered ? 0.35f : 0f, 80);
        }

        /// <summary>
        /// Glide the logo to a position in normalised [0,1] parent-relative space.
        /// Requires the parent to have RelativeSizeAxes = Both and this drawable to
        /// use RelativePositionAxes = Both with Anchor = TopLeft.
        /// </summary>
        public void MoveToNormalisedPosition(Vector2 normalised, double duration = 500)
        {
            this.MoveTo(normalised, duration, Easing.OutQuint);
        }
    }
}
