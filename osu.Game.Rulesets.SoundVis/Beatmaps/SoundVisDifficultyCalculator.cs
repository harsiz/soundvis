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

            // Star formula — more exponential curve (exponent 1.6 vs old 1.45).
            // Fitted so that maps which previously rated ~3★ now rate ~5★:
            //
            //   0.5 BPS → ~1★ (floor)   2 BPS → ~1.5★   5 BPS → ~5★
            //   8 BPS → ~11★           13 BPS → ~23★
            //
            // Derivation: 0.38 * 5^1.6 ≈ 5.0 exactly.
            // Low-BPS maps fall below 1 and are clamped to the floor.
            //
            // clockRate^0.5  — DT(1.5x) adds ~22% stars instead of 50%.
            // modFactor^0.6  — same dampening for approach-speed mods.
            double base_stars = 0.38 * Math.Pow(bps, 1.6);
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
