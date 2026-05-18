using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.SoundVis.UI
{
    public partial class MusicVisualizerDisplay : CompositeDrawable
    {
        private const int BAR_COUNT = 150;
        private const float BAR_WIDTH = 4f;
        private const float BAR_GAP = 2f;
        private const float SMOOTHING = 0.82f;

        private readonly Box[] rightBars = new Box[BAR_COUNT];
        private readonly Box[] leftBars = new Box[BAR_COUNT];
        private readonly float[] smoothed = new float[BAR_COUNT];

        [Resolved(CanBeNull = true)]
        private Bindable<WorkingBeatmap>? beatmap { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;

            var barsContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
            };

            for (int i = 0; i < BAR_COUNT; i++)
            {
                float x = i * (BAR_WIDTH + BAR_GAP);

                rightBars[i] = new Box
                {
                    Width = BAR_WIDTH,
                    Height = 2,
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomLeft,
                    X = x,
                };

                leftBars[i] = new Box
                {
                    Width = BAR_WIDTH,
                    Height = 2,
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomRight,
                    X = -x,
                };

                barsContainer.Add(rightBars[i]);
                barsContainer.Add(leftBars[i]);
            }

            AddInternal(new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                Child = barsContainer,
            });
        }

        protected override void Update()
        {
            base.Update();

            var track = beatmap?.Value?.Track;
            if (track == null) return;

            var amplitudes = track.CurrentAmplitudes.FrequencyAmplitudes.Span;
            double time = Clock.CurrentTime;

            for (int i = 0; i < BAR_COUNT; i++)
            {
                float raw = i < amplitudes.Length ? amplitudes[i] : 0f;
                smoothed[i] = smoothed[i] * SMOOTHING + raw * (1f - SMOOTHING);

                float height = Math.Max(2f, smoothed[i] * DrawHeight * 0.85f);
                float hue = ((float)i / BAR_COUNT + (float)(time / 8000.0)) % 1f;
                var colour = Color4.FromHsv(new Vector4(hue, 0.6f + smoothed[i] * 0.4f, 1f, 1f));

                rightBars[i].Height = height;
                rightBars[i].Colour = colour;
                leftBars[i].Height = height;
                leftBars[i].Colour = colour;
            }
        }
    }
}
