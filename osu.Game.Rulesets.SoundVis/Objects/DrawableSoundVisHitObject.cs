using System;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.SoundVis.Objects
{
    public partial class DrawableSoundVisHitObject : DrawableHitObject<SoundVisHitObject>
    {
        private const float HIT_WINDOW = 200f;
        public const float LOGO_RADIUS = 80f;

        public DrawableSoundVisHitObject(SoundVisHitObject hitObject)
            : base(hitObject)
        {
            // Invisible — the logo drawable is the visual. This handles hit detection only.
            Alpha = 0;
            Size = new Vector2(LOGO_RADIUS * 2);
            Origin = Anchor.Centre;
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (timeOffset < -HIT_WINDOW)
                return;

            if (timeOffset > HIT_WINDOW)
            {
                ApplyResult(HitResult.Miss);
                return;
            }

            if (IsHovered)
                ApplyResult(HitResult.Great);
        }

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos)
        {
            Vector2 local = ToLocalSpace(screenSpacePos);
            Vector2 centre = DrawSize / 2;
            return (local - centre).Length <= LOGO_RADIUS;
        }

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            // no visual transition needed
        }
    }
}
