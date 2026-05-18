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

        public override double NumericResultFor(JudgementResult result)
            => result.Type == HitResult.Great ? Math.Max(100, (int)(jumpDistance * 3f)) : 0;

        protected override double HealthIncreaseFor(JudgementResult result)
            => result.Type == HitResult.Great ? 0.05 : -0.1;
    }
}
