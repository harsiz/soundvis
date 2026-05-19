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
        public const float LOGO_RADIUS = 100f;

        private float baseRotationDegsPerMs = 360f / 4000f;
        private float speedMultiplier = 1f;
        private int spinDirection = 1;

        private TextureStore? textureStore;
        private Container logoContainer = null!;
        private Box hoverGlow = null!;

        private static readonly Color4[] SparkColours =
        {
            new Color4(255, 100, 150, 255),
            new Color4(100, 200, 255, 255),
            new Color4(255, 220, 80, 255),
            new Color4(150, 255, 120, 255),
            new Color4(220, 100, 255, 255),
            new Color4(255, 160, 60, 255),
        };

        [Resolved(CanBeNull = true)]
        private Bindable<WorkingBeatmap>? beatmap { get; set; }

        [BackgroundDependencyLoader]
        private void load(GameHost host)
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Size = new Vector2(LOGO_RADIUS * 2);

            Sprite logoSprite;

            logoContainer = new Container
            {
                // Anchor + Origin = Centre so Rotation pivots around the logo's own centre
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(LOGO_RADIUS * 2),
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
            textureStore = new TextureStore(host.Renderer, new TextureLoaderStore(byteStore));
            logoSprite.Texture = textureStore.Get("Textures/osuvis-logo");

            InternalChild = logoContainer;
        }

        protected override void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes)
        {
            base.OnNewBeat(beatIndex, timingPoint, effectPoint, amplitudes);

            baseRotationDegsPerMs = (float)(360.0 / (timingPoint.BeatLength * 8));
            speedMultiplier = 2.0f;

            logoContainer
                .ScaleTo(1.10f, 60, Easing.OutQuint)
                .Then()
                .ScaleTo(1f, timingPoint.BeatLength * 0.88, Easing.OutQuint);
        }

        protected override void Update()
        {
            base.Update();

            float elapsed = (float)Clock.ElapsedFrameTime;
            speedMultiplier = MathF.Max(1f, speedMultiplier - 0.003f * elapsed);
            logoContainer.Rotation += baseRotationDegsPerMs * speedMultiplier * spinDirection * elapsed;
        }

        public void ReverseSpinDirection()
        {
            spinDirection *= -1;
            speedMultiplier = 3.5f;

            // White flash
            hoverGlow.FadeTo(0.65f, 20).Then().FadeOut(350, Easing.OutQuint);

            // Expanding ring
            var ring = new CircularContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(LOGO_RADIUS * 2),
                BorderColour = Color4.White,
                BorderThickness = 3f,
                Masking = true,
                Alpha = 0.9f,
                Child = new Box { RelativeSizeAxes = Axes.Both, Alpha = 0, AlwaysPresent = true },
            };
            AddInternal(ring);
            ring.ScaleTo(2.8f, 500, Easing.OutQuint)
                .FadeOut(500, Easing.OutQuint)
                .OnComplete(d => d.Expire());

            // Spark particles
            for (int i = 0; i < 8; i++)
            {
                float angleDeg = i / 8f * 360f;
                float angleRad = angleDeg * MathF.PI / 180f;
                var colour = SparkColours[i % SparkColours.Length];

                var spark = new Box
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Size = new Vector2(4, 14),
                    Rotation = angleDeg,
                    Colour = colour,
                    Alpha = 1f,
                };
                AddInternal(spark);

                float targetX = MathF.Sin(angleRad) * 110f;
                float targetY = -MathF.Cos(angleRad) * 110f;

                spark.MoveTo(new Vector2(targetX, targetY), 450, Easing.OutQuint)
                     .ScaleTo(new Vector2(1f, 0.1f), 450, Easing.OutQuint)
                     .FadeOut(450, Easing.OutQuint)
                     .OnComplete(d => d.Expire());
            }
        }

        public void SetHovered(bool hovered)
        {
            hoverGlow.FadeTo(hovered ? 0.3f : 0f, 80);
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            textureStore?.Dispose();
        }
    }
}
