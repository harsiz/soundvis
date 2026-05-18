using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
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

            // Star rating scales with object density (BPS)
            double stars = Math.Round(Math.Clamp(Math.Log(1 + bps * 3) * 2.5 * clockRate, 1.0, 10.0), 2);
            return new DifficultyAttributes(mods, stars);
        }

        protected override IEnumerable<DifficultyHitObject> CreateDifficultyHitObjects(IBeatmap beatmap, double clockRate)
            => [];

        protected override Skill[] CreateSkills(IBeatmap beatmap, Mod[] mods, double clockRate)
            => [];
    }
}
