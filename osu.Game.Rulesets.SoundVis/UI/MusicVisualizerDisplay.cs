using System;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.SoundVis.UI
{
    /// <summary>
    /// The big flashy frequency bar visualiser. Samples track amplitudes every frame
    /// and updates bar heights accordingly. Uses a mirrored layout (bars grow from centre).
    /// </summary>
    public partial class MusicVisualizerDisplay : CompositeDrawable
    {
        private const int BAR_COUNT = 200;
        private const float BAR_WIDTH = 4f;
        private const float BAR_GAP = 2f;
        private const float SMOOTHING = 0.85f;

        private Box[] bars = null!;
        private float[] smoothedAmplitudes = new float[BAR_COUNT];

        [Resolved]
        private IBindable<WorkingBeatmap> beatmap { get; set; } = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            bars = new Box[BAR_COUNT];

            var container = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
            };

            for (int i = 0; i < BAR_COUNT; i++)
            {
                float hue = (float)i / BAR_COUNT;
                var color = Color4.FromHsv(new Vector4(hue, 0.7f, 1f, 1f));

                bars[i] = new Box
                {
                    Width = BAR_WIDTH,
                    Height = 2,
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    X = i * (BAR_WIDTH + BAR_GAP),
                    Colour = color,
                    EdgeSmoothness = new Vector2(0, 1),
                };

                container.Add(bars[i]);
            }

            AddInternal(new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                Children = new Drawable[]
                {
                    // Mirror: right side is just the left flipped
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.BottomRight,
                        Child = container,
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.BottomLeft,
                        Scale = new Vector2(-1, 1),
                        Child = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomLeft,
                            Children = buildMirrorBars(),
                        },
                    },
                }
            });
        }

        private Drawable[] buildMirrorBars()
        {
            var mirrorBars = new Drawable[BAR_COUNT];
            for (int i = 0; i < BAR_COUNT; i++)
            {
                float hue = (float)i / BAR_COUNT;
                var color = Color4.FromHsv(new Vector4(hue, 0.7f, 1f, 1f));

                mirrorBars[i] = new Box
                {
                    Width = BAR_WIDTH,
                    Height = 2,
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    X = i * (BAR_WIDTH + BAR_GAP),
                    Colour = color,
                    EdgeSmoothness = new Vector2(0, 1),
                };
            }

            // Store reference so Update() can use them too (via a separate array)
            mirrorBarRefs = (Box[])mirrorBars;
            return mirrorBars;
        }

        private Box[] mirrorBarRefs = null!;

        protected override void Update()
        {
            base.Update();

            var track = beatmap.Value?.Track;
            if (track == null || !track.IsRunning)
                return;

            var amplitudes = track.CurrentAmplitudes.FrequencyAmplitudes.Span;
            float totalWidth = (BAR_WIDTH + BAR_GAP) * BAR_COUNT;
            float scaleX = DrawWidth / 2f / Math.Max(totalWidth, 1);

            for (int i = 0; i < BAR_COUNT; i++)
            {
                float raw = i < amplitudes.Length ? amplitudes[i] : 0f;

                // exponential smoothing — makes it feel fluid rather than jittery
                smoothedAmplitudes[i] = smoothedAmplitudes[i] * SMOOTHING + raw * (1f - SMOOTHING);

                float barHeight = Math.Max(2f, smoothedAmplitudes[i] * DrawHeight * 0.85f);

                bars[i].Height = barHeight;
                if (mirrorBarRefs != null && i < mirrorBarRefs.Length)
                    mirrorBarRefs[i].Height = barHeight;

                // Shift hue over time for a nice rainbow pulse effect
                float hue = ((float)i / BAR_COUNT + (float)(Clock.CurrentTime / 8000.0)) % 1f;
                var color = Color4.FromHsv(new Vector4(hue, 0.65f + smoothedAmplitudes[i] * 0.35f, 1f, 1f));
                bars[i].Colour = color;
                if (mirrorBarRefs != null && i < mirrorBarRefs.Length)
                    mirrorBarRefs[i].Colour = color;
            }
        }
    }
}
