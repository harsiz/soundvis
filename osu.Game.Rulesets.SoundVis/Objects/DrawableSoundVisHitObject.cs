using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.SoundVis.Configuration;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.SoundVis.Objects
{
    public partial class DrawableSoundVisHitObject : DrawableHitObject<SoundVisHitObject>
    {
        public const double APPROACH_TIME = 1000;

        private const float APPROACH_DIST  = 550f;
        private const float BAR_LENGTH     = 220f;
        private const float BAR_THICKNESS  = 7f;

        // Mod-adjustable properties
        public float ApproachSpeedMultiplier { get; set; } = 1f;

        /// <summary>
        /// Outer timing boundary (Meh window). All sub-windows are derived proportionally.
        ///   default 150 ms → Perfect ≤ 20ms  Good ≤ 60ms  Ok ≤ 100ms  Meh ≤ 150ms
        ///   HHR     50  ms → Perfect ≤ 6.7ms Good ≤ 20ms  Ok ≤ 33ms   Meh ≤ 50ms
        /// </summary>
        public double HitWindow  { get; set; } = 150;
        public double MissWindow { get; set; } = 300;

        /// <summary>When true (Autoplay) the object self-triggers at StartTime.</summary>
        public bool AutoPlay { get; set; }

        /// <summary>When true (HD) the bar fades away mid-approach.</summary>
        public bool Hidden { get; set; }

        // Config bindables
        private readonly BindableBool   showColors    = new BindableBool(true);
        private readonly BindableDouble glowIntensity = new BindableDouble(0.5);

        private Container barGroup = null!;
        private Box       glowBox  = null!;
        private Box       bar      = null!;

        protected override double InitialLifetimeOffset => APPROACH_TIME + 300;

        public DrawableSoundVisHitObject(SoundVisHitObject hitObject)
            : base(hitObject)
        {
            Size   = new Vector2(APPROACH_DIST * 2 + 50);
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }

        [Resolved(CanBeNull = true)]
        private SoundVisRulesetConfigManager? rulesetConfig { get; set; }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            float    angle  = HitObject.ApproachAngle;
            Color4   colour = HitObject.BarColour;

            barGroup = new Container
            {
                Anchor        = Anchor.Centre,
                Origin        = Anchor.Centre,
                AutoSizeAxes  = Axes.Both,
                Rotation      = angle,
                Children = new Drawable[]
                {
                    // Soft additive glow
                    glowBox = new Box
                    {
                        Width   = BAR_LENGTH + 30,
                        Height  = BAR_THICKNESS + 10,
                        Anchor  = Anchor.Centre,
                        Origin  = Anchor.Centre,
                        Colour  = colour,
                        Alpha   = 0.22f,
                        Blending = BlendingParameters.Additive,
                    },
                    // Core bar
                    bar = new Box
                    {
                        Width  = BAR_LENGTH,
                        Height = BAR_THICKNESS,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Colour = colour,
                    },
                },
            };

            AddInternal(barGroup);

            // Wire up config settings
            if (rulesetConfig != null)
            {
                rulesetConfig.BindWith(SoundVisRulesetSetting.ShowColors,       showColors);
                rulesetConfig.BindWith(SoundVisRulesetSetting.BarGlowIntensity, glowIntensity);
            }

            showColors.BindValueChanged(_ => applyColour(), true);
            glowIntensity.BindValueChanged(_ => applyGlow(), true);
        }

        private void applyColour()
        {
            var col = showColors.Value ? HitObject.BarColour : Color4.White;
            bar.Colour     = col;
            glowBox.Colour = col;
        }

        private void applyGlow()
        {
            glowBox.Alpha = (float)(0.22 * glowIntensity.Value * 2); // 0 intensity = no glow
        }

        protected override void Update()
        {
            base.Update();

            double effectiveApproach = APPROACH_TIME / ApproachSpeedMultiplier;
            double timeUntilHit      = HitObject.StartTime - Time.Current;

            if (timeUntilHit > effectiveApproach)
            {
                barGroup.Alpha = 0;
                return;
            }

            double progress = Math.Clamp(
                (Time.Current - HitObject.StartTime + effectiveApproach) / effectiveApproach, 0, 1);

            float dist = (float)(APPROACH_DIST - (APPROACH_DIST - UI.SoundVisLogoDisplay.LOGO_RADIUS) * progress);
            float rad  = HitObject.ApproachAngle * MathF.PI / 180f;
            barGroup.X = MathF.Sin(rad) * dist;
            barGroup.Y = -MathF.Cos(rad) * dist;

            // Alpha — Hidden mod gets its own fade curve
            if (Hidden)
            {
                float p = (float)progress;
                float hiddenAlpha;
                if      (p < 0.40f) hiddenAlpha = Math.Min(p / 0.10f, 1f);
                else if (p < 0.65f) hiddenAlpha = 1f - (p - 0.40f) / 0.25f;
                else if (p < 0.85f) hiddenAlpha = 0f;
                else                hiddenAlpha = (p - 0.85f) / 0.15f;
                barGroup.Alpha = hiddenAlpha;
            }
            else
            {
                barGroup.Alpha = (float)Math.Clamp(progress * 4, 0, 1);
            }

            // Autoplay: self-trigger the moment we reach StartTime
            if (AutoPlay && !Judged && Time.Current >= HitObject.StartTime)
                TriggerResult();
        }

        public void TriggerResult() => UpdateResult(true);

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (userTriggered)
            {
                double abs = Math.Abs(timeOffset);

                // Sub-windows are proportional to the outer Meh boundary (HitWindow).
                // Default (150 ms): Perfect ≤ 20ms  Good ≤ 60ms  Ok ≤ 100ms  Meh ≤ 150ms
                HitResult result;
                if      (abs <= HitWindow * (20.0  / 150.0)) result = HitResult.Perfect;
                else if (abs <= HitWindow * (60.0  / 150.0)) result = HitResult.Good;
                else if (abs <= HitWindow * (100.0 / 150.0)) result = HitResult.Ok;
                else if (abs <= HitWindow)                   result = HitResult.Meh;
                else                                         result = HitResult.Miss;

                ApplyResult(result);
                return;
            }

            // Auto-miss when the note passes the window entirely
            if (timeOffset > MissWindow)
                ApplyResult(HitResult.Miss);
        }

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            switch (state)
            {
                case ArmedState.Hit:
                    this.FadeOut(150, Easing.OutQuint);
                    break;
                case ArmedState.Miss:
                    bar.FadeColour(Color4.Red, 60).Then().FadeOut(300);
                    break;
            }
        }
    }
}
