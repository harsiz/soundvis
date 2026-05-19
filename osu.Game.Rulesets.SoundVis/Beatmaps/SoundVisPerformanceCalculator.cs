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

            // ── Difficulty component ────────────────────────────────────────────────
            // stars^1.5 × 8 peaks at ~2 830 pp for a 50★ map (≈ 3 000 max).
            // stars^2 was the old value; it scaled too fast.
            double diffPp = Math.Pow(stars, 1.5) * 8.0;

            // ── Accuracy factor ─────────────────────────────────────────────────────
            // ^6 makes this very punishing near 100%:
            //   100% → ×1.00   95% → ×0.74   90% → ×0.53   85% → ×0.38
            double accFactor = Math.Pow(accuracy, 6.0);

            // ── Miss penalty ────────────────────────────────────────────────────────
            // hitRate^2: losing 10% of notes → ~81% pp; losing 50% → ~25% pp.
            double hitRate    = total > 0 ? (double)greats / total : 1.0;
            double missPenalty = Math.Pow(hitRate, 2.0);

            // ── Length / volume factor ──────────────────────────────────────────────
            // Shorter maps give meaningfully less pp than long ones at equal difficulty.
            // Scales from ~0.3 at 50 notes to 1.0 at 1 000+ notes (smooth square-root).
            //   50 notes  → ×0.32   200 notes → ×0.58   500 notes → ×0.82
            //   800 notes → ×0.95   1000+ notes → ×1.00
            double lengthFactor = Math.Min(1.0, Math.Sqrt(total / 1000.0) + 0.10);

            double pp = Math.Max(0, diffPp * accFactor * missPenalty * lengthFactor);

            return new PerformanceAttributes { Total = pp };
        }
    }
}
