using System;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.SoundVis.Objects
{
    public class SoundVisJudgement : Judgement
    {
        private readonly float jumpDistance;

        public SoundVisJudgement(float jumpDistance)
        {
            this.jumpDistance = jumpDistance;
        }

        public override HitResult MaxResult => HitResult.Great;

        protected override double HealthIncreaseFor(HitResult result) => result == HitResult.Great ? 0.05 : -0.1;

        public override int NumericResultFor(HitResult result)
            => result == HitResult.Great ? Math.Max(100, (int)(jumpDistance * 3f)) : 0;
    }
}
