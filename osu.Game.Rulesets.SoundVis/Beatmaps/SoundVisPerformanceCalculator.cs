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
            double accuracy = score.Accuracy; // 0.0 – 1.0, already weighted by osu! per grade tier

            // Collect per-grade counts
            score.Statistics.TryGetValue(HitResult.Perfect, out int perfects);
            score.Statistics.TryGetValue(HitResult.Good,    out int goods);
            score.Statistics.TryGetValue(HitResult.Ok,      out int oks);
            score.Statistics.TryGetValue(HitResult.Meh,     out int mehs);
            score.Statistics.TryGetValue(HitResult.Miss,    out int misses);
            int total = perfects + goods + oks + mehs + misses;

            // ── Miss / hit-rate penalty ─────────────────────────────────────────────
            // hitRate^2: losing 10% of notes → 81% pp; losing 50% → 25% pp.
            double hitRate     = total > 0 ? (double)(total - misses) / total : 1.0;
            double missPenalty = Math.Pow(hitRate, 2.0);

            // ── Accuracy factor ─────────────────────────────────────────────────────
            // score.Accuracy already encodes grade mix (Perfect=1.0, Good~0.64, Ok~0.32, Meh~0.16).
            // ^6 keeps it punishing near 100%:
            //   100% → ×1.00   95% → ×0.74   90% → ×0.53
            double accFactor = Math.Pow(accuracy, 6.0);

            // ── Grade quality bonus ─────────────────────────────────────────────────
            // Rewards plays dominated by Perfect/Good over Ok/Meh at the same accuracy.
            // perfectRatio = fraction of hits that were Perfect-tier.
            // Bonus: +0 for all-Meh, up to +20% for all-Perfect.
            double topHits    = total > 0 ? (double)perfects / total : 0;
            double gradeFactor = 0.80 + 0.20 * topHits;

            // ── Difficulty component ────────────────────────────────────────────────
            // stars^1.5 × 8 peaks at ~2 830pp for a perfect 50★ play (≈ 3 000 cap).
            double diffPp = Math.Pow(stars, 1.5) * 8.0;

            // ── Length / volume factor ──────────────────────────────────────────────
            // Shorter maps give less pp than long ones at the same star rating.
            //   50 notes → ×0.32   200 → ×0.52   500 → ×0.80   1000+ → ×1.00
            double lengthFactor = Math.Min(1.0, Math.Sqrt(total / 1000.0) + 0.10);

            double pp = Math.Max(0, diffPp * accFactor * missPenalty * gradeFactor * lengthFactor);

            return new PerformanceAttributes { Total = pp };
        }
    }
}
