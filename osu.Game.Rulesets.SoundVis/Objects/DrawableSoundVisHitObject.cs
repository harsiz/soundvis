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
        public const double HIT_WINDOW = 50;
        public const double MISS_WINDOW = 150;

        private const float APPROACH_DIST = 550f;
        private const float LOGO_RADIUS = 80f;
        private const float BAR_LENGTH = 200f;
        private const float BAR_THICKNESS = 7f;

        // Container that holds glow + bar; we move this each frame
        private Container barGroup = null!;
        private Box bar = null!;

        /// <summary>Set by HR mod — 1.5 means bars travel at 1.5× speed.</summary>
        public float ApproachSpeedMultiplier { get; set; } = 1f;

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

            barGroup = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                AutoSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    // Outer glow
                    new Box
                    {
                        Width = BAR_LENGTH + 30,
                        Height = BAR_THICKNESS + 10,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Colour = Color4.White,
                        Alpha = 0.18f,
                        Blending = BlendingParameters.Additive,
                    },
                    // Core bar
                    bar = new Box
                    {
                        Width = BAR_LENGTH,
                        Height = BAR_THICKNESS,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Colour = Color4.White,
                    },
                },
                Rotation = angle,
            };

            AddInternal(barGroup);
        }

        protected override void Update()
        {
            base.Update();

            double effectiveApproach = APPROACH_TIME / ApproachSpeedMultiplier;
            double timeUntilHit = HitObject.StartTime - Time.Current;

            // Don't show the bar until it's within the effective approach window
            if (timeUntilHit > effectiveApproach)
            {
                barGroup.Alpha = 0;
                return;
            }

            double progress = Math.Clamp(
                (Time.Current - HitObject.StartTime + effectiveApproach) / effectiveApproach, 0, 1);

            float dist = (float)(APPROACH_DIST - (APPROACH_DIST - LOGO_RADIUS) * progress);
            float rad = HitObject.ApproachAngle * MathF.PI / 180f;
            barGroup.X = MathF.Sin(rad) * dist;
            barGroup.Y = -MathF.Cos(rad) * dist;

            barGroup.Alpha = (float)Math.Clamp(progress * 4, 0, 1);
        }

        public void TriggerResult() => UpdateResult(true);

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (userTriggered)
            {
                ApplyResult(Math.Abs(timeOffset) <= HIT_WINDOW ? HitResult.Great : HitResult.Miss);
                return;
            }

            if (timeOffset > MISS_WINDOW)
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
                    bar.FadeColour(Color4.Red, 60)
                       .Then()
                       .FadeOut(300);
                    break;
            }
        }
    }
}
