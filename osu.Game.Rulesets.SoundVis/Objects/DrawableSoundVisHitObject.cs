using System;
using osu.Framework.Graphics;
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

        private const float APPROACH_DIST = 550f; // pixels from centre to start
        private const float LOGO_RADIUS = 80f;
        private const float BAR_LENGTH = 200f;
        private const float BAR_THICKNESS = 7f;

        private Box bar = null!;

        protected override double InitialLifetimeOffset => APPROACH_TIME + 300;

        public DrawableSoundVisHitObject(SoundVisHitObject hitObject)
            : base(hitObject)
        {
            // Sized to cover the full approach path; centred on the playfield
            Size = new Vector2(APPROACH_DIST * 2 + 50);
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Alpha = 1;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            float angle = HitObject.ApproachAngle;

            bar = new Box
            {
                Width = BAR_LENGTH,
                Height = BAR_THICKNESS,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Rotation = angle,
                Colour = Color4.White,
            };
            AddInternal(bar);
        }

        protected override void Update()
        {
            base.Update();

            double progress = Math.Clamp(
                (Time.Current - HitObject.StartTime + APPROACH_TIME) / APPROACH_TIME, 0, 1);

            // Move along the approach angle from APPROACH_DIST down to LOGO_RADIUS
            float dist = (float)(APPROACH_DIST - (APPROACH_DIST - LOGO_RADIUS) * progress);
            float rad = HitObject.ApproachAngle * MathF.PI / 180f;
            bar.X = MathF.Sin(rad) * dist;
            bar.Y = -MathF.Cos(rad) * dist;

            // Fade in quickly, then stay visible
            bar.Alpha = (float)Math.Clamp(progress * 4, 0, 1);
        }

        /// <summary>Called by the playfield when the player presses the hit key.</summary>
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
                    bar.FadeColour(Color4.Red, 80)
                       .Then()
                       .FadeOut(250);
                    break;
            }
        }
    }
}
