using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.SoundVis.Objects
{
    public partial class DrawableSoundVisHitObject : DrawableHitObject<SoundVisHitObject>
    {
        public const double APPROACH_TIME = 1000;

        private const float APPROACH_DIST = 550f;
        private const float BAR_LENGTH = 220f;
        private const float BAR_THICKNESS = 7f;

        public float ApproachSpeedMultiplier { get; set; } = 1f;
        public double HitWindow { get; set; } = 120;
        public double MissWindow { get; set; } = 300;

        /// <summary>When true (autoplay mod) the object triggers itself perfectly at StartTime.</summary>
        public bool AutoPlay { get; set; }

        /// <summary>When true (HD mod) the bar fades away mid-approach.</summary>
        public bool Hidden { get; set; }

        private Container barGroup = null!;
        private Box bar = null!;

        protected override double InitialLifetimeOffset => APPROACH_TIME + 300;

        public DrawableSoundVisHitObject(SoundVisHitObject hitObject)
            : base(hitObject)
        {
            Size = new Vector2(APPROACH_DIST * 2 + 50);
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            float angle = HitObject.ApproachAngle;
            var colour = HitObject.BarColour;

            barGroup = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                AutoSizeAxes = Axes.Both,
                Rotation = angle,
                Children = new Drawable[]
                {
                    // Soft additive glow
                    new Box
                    {
                        Width = BAR_LENGTH + 30,
                        Height = BAR_THICKNESS + 10,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Colour = colour,
                        Alpha = 0.22f,
                        Blending = BlendingParameters.Additive,
                    },
                    // Core bar
                    bar = new Box
                    {
                        Width = BAR_LENGTH,
                        Height = BAR_THICKNESS,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Colour = colour,
                    },
                },
            };

            AddInternal(barGroup);
        }

        protected override void Update()
        {
            base.Update();

            double effectiveApproach = APPROACH_TIME / ApproachSpeedMultiplier;
            double timeUntilHit = HitObject.StartTime - Time.Current;

            if (timeUntilHit > effectiveApproach)
            {
                barGroup.Alpha = 0;
                return;
            }

            double progress = Math.Clamp(
                (Time.Current - HitObject.StartTime + effectiveApproach) / effectiveApproach, 0, 1);

            float dist = (float)(APPROACH_DIST - (APPROACH_DIST - UI.SoundVisLogoDisplay.LOGO_RADIUS) * progress);
            float rad = HitObject.ApproachAngle * MathF.PI / 180f;
            barGroup.X = MathF.Sin(rad) * dist;
            barGroup.Y = -MathF.Cos(rad) * dist;

            if (Hidden)
            {
                // Visible while approaching (0-40%), fades out through mid-flight (40-65%),
                // then briefly flashes back just before the hit (85-100%) for a last-second cue.
                float p = (float)progress;
                float hiddenAlpha;
                if      (p < 0.40f) hiddenAlpha = Math.Min(p / 0.10f, 1f);   // fade in
                else if (p < 0.65f) hiddenAlpha = 1f - (p - 0.40f) / 0.25f;  // fade out
                else if (p < 0.85f) hiddenAlpha = 0f;                          // invisible
                else                hiddenAlpha = (p - 0.85f) / 0.15f;        // flash back
                barGroup.Alpha = hiddenAlpha;
            }
            else
            {
                barGroup.Alpha = (float)Math.Clamp(progress * 4, 0, 1);
            }

            // Autoplay: self-trigger at perfect timing (0ms offset → Great).
            if (AutoPlay && !Judged && Time.Current >= HitObject.StartTime)
                TriggerResult();
        }

        public void TriggerResult() => UpdateResult(true);

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (userTriggered)
            {
                ApplyResult(Math.Abs(timeOffset) <= HitWindow ? HitResult.Great : HitResult.Miss);
                return;
            }

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
