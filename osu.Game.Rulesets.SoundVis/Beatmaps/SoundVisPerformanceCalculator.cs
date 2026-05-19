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

        protected override PerformanceAttributes CreatePerformanceAttributes(ScoreInfo score, DifficultyAttributes attributes)
        {
            double stars    = attributes.StarRating;
            double accuracy = score.Accuracy; // 0.0 – 1.0

            score.Statistics.TryGetValue(HitResult.Great, out int greats);
            score.Statistics.TryGetValue(HitResult.Miss,  out int misses);
            int total = greats + misses;

            // Miss penalty — each miss reduces pp, scales with how many you got
            double hitRate    = total > 0 ? (double)greats / total : 1.0;
            double missPenalty = Math.Pow(hitRate, 2.0);

            // PP formula — mirrors osu! standard's steep difficulty scaling:
            //
            //   diffPp   = stars² × 5   (quadratic: doubling stars → 4× pp, just like osu! std)
            //   accFactor = accuracy^6   (very sensitive near 100%; 95% acc ≈ 74% of max pp)
            //   missPenalty              (not miss-forgiving at all for many misses)
            //
            // The logarithmic "volume bonus" from the old formula has been removed so pp
            // doesn't start big and slow down — it grows purely with star rating.
            double diffPp    = Math.Pow(stars,    2.0) * 5.0;
            double accFactor = Math.Pow(accuracy, 6.0);

            double pp = Math.Max(0, diffPp * accFactor * missPenalty);

            return new PerformanceAttributes { Total = pp };
        }
    }
}
