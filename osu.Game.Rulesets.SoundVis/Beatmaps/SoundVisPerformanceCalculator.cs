using System;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;

namespace osu.Game.Rulesets.SoundVis.Beatmaps
{
    public class SoundVisPerformanceCalculator : PerformanceCalculator
    {
        public SoundVisPerformanceCalculator(Ruleset ruleset) : base(ruleset) { }

        // The abstract method osu! lazer actually exposes is CreatePerformanceAttributes.
        protected override PerformanceAttributes CreatePerformanceAttributes(ScoreInfo score, DifficultyAttributes attributes)
        {
            double stars    = attributes.StarRating;
            double accuracy = score.Accuracy; // 0.0 – 1.0

            // Count greats and misses for a volume-based bonus
            score.Statistics.TryGetValue(HitResult.Great, out int greats);
            score.Statistics.TryGetValue(HitResult.Miss,  out int misses);
            int total = greats + misses;

            // Hit-rate factor (penalises heavy-miss plays)
            double hitRate = total > 0 ? (double)greats / total : 1.0;

            // Core formula:
            //   base   = stars^1.5  (non-linear difficulty scaling)
            //   acc    = accuracy^4 (punishing near-100%)
            //   volume = log(1+greats)*0.4+1 (more objects → more pp, log-damped)
            //   hitRate^1.5 discounts plays with many misses
            double basePp     = Math.Pow(stars,    1.5) * 8.0;
            double accBonus   = Math.Pow(accuracy, 4);
            double volBonus   = Math.Log(1 + greats) * 0.4 + 1.0;
            double hitPenalty = Math.Pow(hitRate,  1.5);

            double pp = Math.Max(0, basePp * accBonus * volBonus * hitPenalty);

            return new PerformanceAttributes { Total = pp };
        }
    }
}
