using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.SoundVis.Replays;

namespace osu.Game.Rulesets.SoundVis.Mods
{
    public class SoundVisModAutoplay : ModAutoplay
    {
        public override ModReplayData CreateReplayData(IBeatmap beatmap, IReadOnlyList<Mod> mods)
        {
            var replay = new SoundVisAutoGenerator(beatmap).Generate();
            return new ModReplayData(replay, new ModCreatedUser { Username = "osu!vis" });
        }
    }
}
