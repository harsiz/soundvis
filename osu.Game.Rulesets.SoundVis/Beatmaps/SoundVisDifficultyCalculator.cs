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
                return new DifficultyAttributes(mods, 0);

            double totalDuration = (objects[^1].StartTime - objects[0].StartTime) / 1000.0;
            double objectsPerSecond = totalDuration > 0 ? objects.Count / totalDuration : 1.0;
            double avgJumpDistance = objects.Skip(1).Average(o => o.JumpDistance);

            // star rating: scales with jump distance and density
            double stars = Math.Log(1 + (avgJumpDistance / 0.5) * Math.Sqrt(objectsPerSecond) * 2.0) * 4.0;
            stars = Math.Round(Math.Clamp(stars, 0.5, 10.0) * clockRate, 2);

            return new DifficultyAttributes(mods, stars);
        }

        protected override IEnumerable<DifficultyHitObject> CreateDifficultyHitObjects(IBeatmap beatmap, double clockRate)
            => [];

        protected override Skill[] CreateSkills(IBeatmap beatmap, Mod[] mods, double clockRate)
            => [];
    }
}
