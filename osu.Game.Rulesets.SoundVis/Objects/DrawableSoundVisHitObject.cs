using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.SoundVis.Objects
{
    public partial class DrawableSoundVisHitObject : DrawableHitObject<SoundVisHitObject>
    {
        private const float HIT_WINDOW = 200f;

        // Set by the playfield every frame based on actual logo visual position.
        public bool MouseIsOverLogo { get; set; }

        // Short lead time so the logo arrives just before the hit window opens.
        protected override double InitialLifetimeOffset => 700;

        public DrawableSoundVisHitObject(SoundVisHitObject hitObject)
            : base(hitObject)
        {
            Alpha = 0;
            Size = Vector2.Zero; // no visual, no input area — detection is done in Playfield
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

            if (MouseIsOverLogo)
                ApplyResult(HitResult.Great);
        }

        protected override void UpdateHitStateTransforms(ArmedState state) { }
    }
}
