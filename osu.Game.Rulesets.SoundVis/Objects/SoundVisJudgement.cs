using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.SoundVis.Objects
{
    public class SoundVisJudgement : Judgement
    {
        /// <summary>
        /// Scales all health deltas by the map's estimated difficulty, clamped to [0.15, 1.0].
        /// 1.0 = full drain (≥6★ maps). 0.15 = minimum drain (easy maps).
        /// Set by <see cref="SoundVisHitObject.HealthMultiplier"/>.
        /// </summary>
        private readonly double healthMultiplier;

        public SoundVisJudgement(double healthMultiplier = 1.0)
        {
            this.healthMultiplier = healthMultiplier;
        }

        public override HitResult MaxResult => HitResult.Perfect;

        protected override double HealthIncreaseFor(HitResult result)
        {
            double delta = result switch
            {
                HitResult.Perfect => 0.06,
                HitResult.Good    => 0.04,
                HitResult.Ok      => 0.01,
                HitResult.Meh     => 0.00,  // neutral
                HitResult.Miss    => -0.08, // ~13 consecutive misses from full = dead at full mult
                _                 => 0,
            };
            return delta * healthMultiplier;
        }
    }
}
