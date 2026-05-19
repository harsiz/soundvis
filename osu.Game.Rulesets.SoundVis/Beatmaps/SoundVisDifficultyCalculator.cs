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

            // clockRate already encodes DT/HT/NC speed (passed in by the framework).
            // Approach-speed mods add extra difficulty on top.
            double modMultiplier = 1.0;
            foreach (var mod in mods)
            {
                if (mod is SoundVisModHarderHardRock) { modMultiplier = 1.9; break; }
                if (mod is SoundVisModHardRock)        { modMultiplier = 1.4; break; }
            }

            // Power-law formula so density differences matter at all levels.
            //   density = notes/sec adjusted for playback rate
            //   stars   = 1.8 * density^1.1  (sub-linear growth at the low end,
            //             steeper above ~4 BPS so hard maps get meaningfully higher stars)
            // Cap raised to 50 — theoretical max with min-gap=60ms is ~16 BPS base,
            // which gives ~38★; DT(1.5x) could push that to ~57★ → capped at 50.
            double density = bps * clockRate * modMultiplier;
            double stars = Math.Round(Math.Clamp(1.8 * Math.Pow(density, 1.1), 1.0, 50.0), 2);

            return new DifficultyAttributes(mods, stars);
        }

        protected override IEnumerable<DifficultyHitObject> CreateDifficultyHitObjects(IBeatmap beatmap, double clockRate)
            => [];

        protected override Skill[] CreateSkills(IBeatmap beatmap, Mod[] mods, double clockRate)
            => [];
    }
}
