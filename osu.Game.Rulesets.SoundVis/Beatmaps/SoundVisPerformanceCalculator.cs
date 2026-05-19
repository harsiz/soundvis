using System;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;

namespace osu.Game.Rulesets.SoundVis.Beatmaps
{
    public class SoundVisPerformanceCalculator : PerformanceCalculator
    {
        public override PerformanceAttributes Calculate(ScoreInfo score, DifficultyAttributes attributes)
        {
            double stars = attributes.StarRating;
            double accuracy = score.Accuracy; // 0.0 – 1.0

            // Count greats and misses for a volume-based bonus
            score.Statistics.TryGetValue(HitResult.Great, out int greats);
            score.Statistics.TryGetValue(HitResult.Miss, out int misses);
            int total = greats + misses;

            // Hit-rate factor (penalises maps where most objects were missed)
            double hitRate = total > 0 ? (double)greats / total : 1.0;

            // Core formula:
            //   base   = stars^1.5  (scales difficulty non-linearly)
            //   acc    = accuracy^4 (very sensitive near 100%)
            //   volume = log(1+greats)*0.4+1 (more objects → more pp, log-damped)
            //   hitRate factor discounts heavy-miss plays
            double basePp    = Math.Pow(stars, 1.5) * 150.0;
            double accBonus  = Math.Pow(accuracy, 4);
            double volBonus  = Math.Log(1 + greats) * 0.4 + 1.0;
            double hitPenalty = Math.Pow(hitRate, 1.5);

            double pp = basePp * accBonus * volBonus * hitPenalty;

            // Clamp to something sane (stars are already capped at 10 by difficulty calc)
            pp = Math.Max(0, pp);

            return new PerformanceAttributes { Total = pp };
        }
    }
}
