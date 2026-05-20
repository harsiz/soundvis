using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.SoundVis.Mods;
using osu.Game.Rulesets.SoundVis.Objects;

namespace osu.Game.Rulesets.SoundVis.Beatmaps
{
    public class SoundVisDifficultyCalculator : DifficultyCalculator
    {
        public SoundVisDifficultyCalculator(IRulesetInfo ruleset, IWorkingBeatmap beatmap)
            : base(ruleset, beatmap)
        {
        }

        protected override DifficultyAttributes CreateDifficultyAttributes(IBeatmap beatmap, Mod[] mods, Skill[] skills, double clockRate)
        {
            var objects = beatmap.HitObjects.OfType<SoundVisHitObject>().ToList();

            if (objects.Count < 2)
                return new DifficultyAttributes(mods, 1.0);

            double totalDuration = (objects[^1].StartTime - objects[0].StartTime) / 1000.0;
            double bps = totalDuration > 0 ? objects.Count / totalDuration : 1.0;

            // Approach-speed mod factors — dampened exponent so HR/HHR don't
            // inflate stars out of proportion (same reasoning as clockRate below).
            double modFactor = 1.0;
            foreach (var mod in mods)
            {
                if (mod is SoundVisModHarderHardRock) { modFactor = 1.9; break; }
                if (mod is SoundVisModHardRock)        { modFactor = 1.4; break; }
            }

            // Star formula — quadratic shifted so the steep section lives at 5→7★.
            // Wide 1★ floor: all maps with BPS ≤ 2 rate exactly 1★.
            // Growth is gentle at low BPS and accelerates at high BPS:
            //
            //   BPS=2  → 1★ (floor)   BPS=3  → 1.44★   BPS=5  → 5★
            //   BPS=8  → ~17★         BPS=13 → ~50★ (capped)
            //
            // Derivation: 4/9 * (5−2)² + 1 = 4 + 1 = 5.0 exactly at 5 BPS.
            //
            // clockRate^0.5  — DT(1.5x) adds ~22% stars instead of 50%.
            // modFactor^0.6  — same dampening for approach-speed mods.
            double shifted    = Math.Max(0.0, bps - 2.0);
            double base_stars = shifted * shifted * (4.0 / 9.0) + 1.0;
            double stars = base_stars
                           * Math.Pow(clockRate, 0.5)
                           * Math.Pow(modFactor, 0.6);

            stars = Math.Round(Math.Clamp(stars, 1.0, 50.0), 2);

            return new DifficultyAttributes(mods, stars);
        }

        protected override IEnumerable<DifficultyHitObject> CreateDifficultyHitObjects(IBeatmap beatmap, double clockRate)
            => [];

        protected override Skill[] CreateSkills(IBeatmap beatmap, Mod[] mods, double clockRate)
            => [];
    }
}
