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
    public partial class MusicVisualizerDisplay : BeatSyncedContainer
    {
        private const int BAR_COUNT = 200;
        private const float LOGO_RADIUS = 120f;
        private const float MAX_BAR_LENGTH = 200f;
        private const float BAR_WIDTH = 3f;
        private const float SMOOTHING = 0.78f;

        private readonly Box[] bars = new Box[BAR_COUNT];
        private readonly float[] smoothed = new float[BAR_COUNT];

        // Rotation state
        private float baseRotationDegsPerMs = 360f / 4000f; // default: 120 BPM, 1 rev per 8 beats
        private float speedMultiplier = 1f;

        private Container logoContainer = null!;

        [Resolved(CanBeNull = true)]
        private Bindable<WorkingBeatmap>? beatmap { get; set; }

        [BackgroundDependencyLoader]
        private void load(GameHost host)
        {
            RelativeSizeAxes = Axes.Both;

            // --- Frequency bars (radiate outward from logo perimeter) ---
            var barsLayer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            };

            for (int i = 0; i < BAR_COUNT; i++)
            {
                float angleDeg = (float)i / BAR_COUNT * 360f;
                float angleRad = angleDeg * MathF.PI / 180f;

                bars[i] = new Box
                {
                    Width = BAR_WIDTH,
                    Height = 2,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.BottomCentre,
                    Rotation = angleDeg,
                    X = MathF.Sin(angleRad) * LOGO_RADIUS,
                    Y = -MathF.Cos(angleRad) * LOGO_RADIUS,
                };

                barsLayer.Add(bars[i]);
            }

            // --- Logo (real osu! lazer logo image, circular masked) ---
            Sprite logoSprite;

            logoContainer = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(LOGO_RADIUS * 2),
                Masking = true,
                CornerRadius = LOGO_RADIUS,
                Children = new Drawable[]
                {
                    // Dark background behind the logo
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
                },
            };

            // Load the embedded logo texture from our own DLL resources
            var resources = new DllResourceStore(typeof(SoundVisRuleset).Assembly);
            var byteStore = new NamespacedResourceStore<byte[]>(resources, "Resources");
            using var textureStore = new TextureStore(
                host.Renderer,
                new TextureLoaderStore(byteStore));
            logoSprite.Texture = textureStore.Get("Textures/lazer-logo");

            AddRangeInternal(new Drawable[]
            {
                barsLayer,
                logoContainer,
            });
        }

        protected override void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes)
        {
            base.OnNewBeat(beatIndex, timingPoint, effectPoint, amplitudes);

            // 1 revolution per 8 beats at the current BPM
            baseRotationDegsPerMs = (float)(360.0 / (timingPoint.BeatLength * 8));

            // Kick the spin on every beat — it decays back in Update()
            speedMultiplier = 2.8f;

            // Heartbeat pulse — quick scale up, ease back over the beat
            logoContainer
                .ScaleTo(1.12f, 55, Easing.OutQuint)
                .Then()
                .ScaleTo(1f, timingPoint.BeatLength * 0.88, Easing.OutQuint);
        }

        protected override void Update()
        {
            base.Update();

            float elapsed = (float)Clock.ElapsedFrameTime;

            // Decay the kick multiplier back towards 1.0 linearly
            // 0.003 units per ms → 1.8 units (kick amount) decays over ~600ms
            speedMultiplier = MathF.Max(1f, speedMultiplier - 0.003f * elapsed);

            logoContainer.Rotation += baseRotationDegsPerMs * speedMultiplier * elapsed;

            // --- Update frequency bars ---
            var track = beatmap?.Value?.Track;
            if (track == null) return;

            var amplitudes = track.CurrentAmplitudes.FrequencyAmplitudes.Span;

            for (int i = 0; i < BAR_COUNT; i++)
            {
                int bin = Math.Clamp((int)((float)i / BAR_COUNT * amplitudes.Length), 0, amplitudes.Length - 1);
                float raw = amplitudes[bin];

                smoothed[i] = smoothed[i] * SMOOTHING + raw * (1f - SMOOTHING);

                bars[i].Height = 2f + smoothed[i] * MAX_BAR_LENGTH;

                float hue = ((float)i / BAR_COUNT + (float)(Clock.CurrentTime / 12000.0)) % 1f;
                bars[i].Colour = Color4.FromHsv(new Vector4(hue, 0.65f + smoothed[i] * 0.35f, 1f, 1f));
            }
        }
    }
}
