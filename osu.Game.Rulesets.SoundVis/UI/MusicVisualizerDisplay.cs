using System;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.SoundVis.UI
{
    /// <summary>
    /// Circular frequency-bar visualiser with a beat-synced spinning osu! cookie in the centre.
    /// Bars radiate outward from the logo edge, one per frequency bin.
    /// The logo pulses on each beat (heartbeat) and spins continuously at BPM speed.
    /// </summary>
    public partial class MusicVisualizerDisplay : BeatSyncedContainer
    {
        private const int BAR_COUNT = 200;
        private const float LOGO_RADIUS = 110f;
        private const float MAX_BAR_LENGTH = 180f;
        private const float BAR_WIDTH = 3f;
        private const float SMOOTHING = 0.80f;

        private readonly Box[] bars = new Box[BAR_COUNT];
        private readonly float[] smoothed = new float[BAR_COUNT];

        private Container logoContainer = null!;
        private float rotationDegreesPerMs = 360f / 2000f; // default: 120 BPM

        [Resolved(CanBeNull = true)]
        private Bindable<WorkingBeatmap>? beatmap { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;

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
                    // Positioned at the logo's perimeter in the bar's outward direction
                    X = MathF.Sin(angleRad) * LOGO_RADIUS,
                    Y = -MathF.Cos(angleRad) * LOGO_RADIUS,
                };

                barsLayer.Add(bars[i]);
            }

            logoContainer = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(LOGO_RADIUS * 2),
                Masking = true,
                CornerRadius = LOGO_RADIUS,
                Children = new Drawable[]
                {
                    // Pink gradient background — the "cookie"
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = new Color4(220, 80, 165, 255),
                    },
                    // Inner glow ring
                    new CircularContainer
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(LOGO_RADIUS * 1.7f),
                        Masking = true,
                        BorderThickness = 4,
                        BorderColour = new Color4(255, 180, 220, 120),
                        Child = new Box { RelativeSizeAxes = Axes.Both, Alpha = 0 },
                    },
                    new OsuSpriteText
                    {
                        Text = "osu!",
                        Font = OsuFont.GetFont(size: 50, weight: FontWeight.Bold),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Colour = Color4.White,
                        Shadow = true,
                    },
                },
            };

            AddRangeInternal(new Drawable[]
            {
                barsLayer,
                logoContainer,
            });
        }

        protected override void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes)
        {
            base.OnNewBeat(beatIndex, timingPoint, effectPoint, amplitudes);

            // Spin speed: one full revolution per 4 beats
            rotationDegreesPerMs = (float)(360.0 / (timingPoint.BeatLength * 4));

            // Heartbeat pulse — quick scale up then decay back over the beat
            logoContainer
                .ScaleTo(1.10f, 60, Easing.OutQuint)
                .Then()
                .ScaleTo(1f, timingPoint.BeatLength * 0.9, Easing.OutQuint);
        }

        protected override void Update()
        {
            base.Update();

            // Continuous spin at BPM-derived speed
            logoContainer.Rotation += rotationDegreesPerMs * (float)Clock.ElapsedFrameTime;

            var track = beatmap?.Value?.Track;
            if (track == null) return;

            var amplitudes = track.CurrentAmplitudes.FrequencyAmplitudes.Span;

            for (int i = 0; i < BAR_COUNT; i++)
            {
                int bin = (int)((float)i / BAR_COUNT * amplitudes.Length);
                float raw = bin < amplitudes.Length ? amplitudes[bin] : 0f;

                smoothed[i] = smoothed[i] * SMOOTHING + raw * (1f - SMOOTHING);

                float barLength = 2f + smoothed[i] * MAX_BAR_LENGTH;
                float hue = ((float)i / BAR_COUNT + (float)(Clock.CurrentTime / 12000.0)) % 1f;
                var colour = Color4.FromHsv(new Vector4(hue, 0.65f + smoothed[i] * 0.35f, 1f, 1f));

                bars[i].Height = barLength;
                bars[i].Colour = colour;
            }
        }
    }
}
