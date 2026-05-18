using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.SoundVis.Objects
{
    public class SoundVisJudgement : Judgement
    {
        public override HitResult MaxResult => HitResult.Great;

        protected override double HealthIncreaseFor(HitResult result)
            => result == HitResult.Great ? 0.05 : -0.1;
    }
}
