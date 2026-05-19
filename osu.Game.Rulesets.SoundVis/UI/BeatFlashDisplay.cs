using System;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics.Containers;
using osu.Game.Rulesets.SoundVis.Configuration;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.SoundVis.UI
{
    /// <summary>
    /// Glowing side panels and a subtle screen pulse that react to the beat.
    /// Flashes are brighter during kiai sections and when the music is louder than the recent average.
    /// </summary>
    public partial class BeatFlashDisplay : BeatSyncedContainer
    {
        // Rolling amplitude history for "louder-than-average" detection
        private const int HISTORY_LEN = 24;
        private readonly float[] ampHistory = new float[HISTORY_LEN];
        private int historyIdx;

        // Slow hue drift — gives each beat a slightly different colour
        private float hue;

        // Panel references
        private Box leftPanel  = null!;
        private Box rightPanel = null!;
        private Box screenPulse = null!;

        private readonly BindableBool showBeatFlashes = new BindableBool(true);

        [Resolved(CanBeNull = true)]
        private SoundVisRulesetConfigManager? rulesetConfig { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;

            // ── Full-screen pulse (very subtle, sits behind everything) ───────────
            AddInternal(screenPulse = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour           = Color4.White,
                Blending         = BlendingParameters.Additive,
                Alpha            = 0,
            });

            // ── Left side panel ───────────────────────────────────────────────────
            AddInternal(new Container
            {
                RelativeSizeAxes = Axes.Y,
                Width            = 180,
                Anchor           = Anchor.CentreLeft,
                Origin           = Anchor.CentreLeft,
                Children = new Drawable[]
                {
                    leftPanel = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Blending         = BlendingParameters.Additive,
                        Alpha            = 0,
                    },
                },
            });

            // ── Right side panel ──────────────────────────────────────────────────
            AddInternal(new Container
            {
                RelativeSizeAxes = Axes.Y,
                Width            = 180,
                Anchor           = Anchor.CentreRight,
                Origin           = Anchor.CentreRight,
                Children = new Drawable[]
                {
                    rightPanel = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Blending         = BlendingParameters.Additive,
                        Alpha            = 0,
                    },
                },
            });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            rulesetConfig?.BindWith(SoundVisRulesetSetting.ShowBeatFlashes, showBeatFlashes);
        }

        protected override void OnNewBeat(
            int beatIndex,
            TimingControlPoint timingPoint,
            EffectControlPoint effectPoint,
            ChannelAmplitudes amplitudes)
        {
            base.OnNewBeat(beatIndex, timingPoint, effectPoint, amplitudes);

            if (!showBeatFlashes.Value) return;

            // ── Amplitude history & ratio ─────────────────────────────────────────
            float amp = amplitudes.Maximum;
            ampHistory[historyIdx % HISTORY_LEN] = amp;
            historyIdx++;

            float avg = 0;
            for (int i = 0; i < HISTORY_LEN; i++) avg += ampHistory[i];
            avg /= HISTORY_LEN;

            // ratio > 1  →  louder than recent average  →  stronger flash
            float ratio = avg > 0.005f ? amp / avg : 1f;

            // ── Base strength: kiai + above-average loudness ───────────────────────
            // kiai alone → 0.35 base; additionally ramp up if ratio > 0.9
            float kiaiBoost    = effectPoint.KiaiMode ? 0.35f : 0f;
            float loudnessBoost = Math.Clamp((ratio - 0.9f) * 1.1f, 0f, 0.55f);
            float strength     = Math.Clamp(kiaiBoost + loudnessBoost, 0f, 0.80f);

            if (strength < 0.06f) return; // skip very quiet non-kiai beats

            // ── Colour: slow hue drift with higher saturation during kiai ─────────
            hue = (hue + 0.055f) % 1f;
            float saturation = effectPoint.KiaiMode ? 0.90f : 0.70f;
            var   colour     = Color4.FromHsv(new Vector4(hue, saturation, 1f, 1f));

            double beatLen = timingPoint.BeatLength;

            // Side panels: sharp hit, long tail
            leftPanel.Colour  = colour;
            rightPanel.Colour = colour;

            leftPanel
                .FadeTo(strength * 0.75f, 35, Easing.OutQuint)
                .Then()
                .FadeOut(beatLen * 0.85, Easing.OutQuint);

            rightPanel
                .FadeTo(strength * 0.75f, 35, Easing.OutQuint)
                .Then()
                .FadeOut(beatLen * 0.85, Easing.OutQuint);

            // Screen pulse: much softer, kiai-only or very loud beats
            if (effectPoint.KiaiMode || ratio > 1.35f)
            {
                float pulseStrength = effectPoint.KiaiMode ? strength * 0.10f : strength * 0.06f;
                screenPulse.Colour = colour;
                screenPulse
                    .FadeTo(pulseStrength, 30, Easing.OutQuint)
                    .Then()
                    .FadeOut(beatLen * 0.6, Easing.OutQuint);
            }
        }
    }
}
