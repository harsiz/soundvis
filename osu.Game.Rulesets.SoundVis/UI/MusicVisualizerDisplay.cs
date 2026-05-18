using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics.Containers;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.SoundVis.UI
{
    /// <summary>
    /// Background-only radial frequency bars. The moving logo is handled by SoundVisLogoDisplay.
    /// </summary>
    public partial class MusicVisualizerDisplay : BeatSyncedContainer
    {
        private const int BAR_COUNT = 200;
        private const float LOGO_RADIUS = 120f;
        private const float MAX_BAR_LENGTH = 200f;
        private const float BAR_WIDTH = 3f;
        private const float SMOOTHING = 0.78f;

        private readonly Box[] bars = new Box[BAR_COUNT];
        private readonly float[] smoothed = new float[BAR_COUNT];

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
                    X = MathF.Sin(angleRad) * LOGO_RADIUS,
                    Y = -MathF.Cos(angleRad) * LOGO_RADIUS,
                };

                barsLayer.Add(bars[i]);
            }

            AddInternal(barsLayer);
        }

        protected override void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes)
        {
            base.OnNewBeat(beatIndex, timingPoint, effectPoint, amplitudes);
        }

        protected override void Update()
        {
            base.Update();

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
