using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.SoundVis.Objects;

namespace osu.Game.Rulesets.SoundVis.Beatmaps
{
    public class SoundVisBeatmap : Beatmap<SoundVisHitObject>
    {
        public override IEnumerable<BeatmapStatistic> GetStatistics() => [];
    }
}
