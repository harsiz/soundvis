using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.SoundVis.Objects;
using osu.Game.Rulesets.SoundVis.Replays;

namespace osu.Game.Rulesets.SoundVis.Mods
{
    public class SoundVisModAutoplay : ModAutoplay
    {
        public override ModReplayData CreateReplayData(IBeatmap beatmap, IReadOnlyList<Mod> mods)
        {
            var frames = new List<ReplayFrame>
            {
                // Initial empty frame so the input handler has something to start with.
                new SoundVisReplayFrame(0),
            };

            foreach (var obj in beatmap.HitObjects)
            {
                if (obj is not SoundVisHitObject hit)
                    continue;

                // Press the required key exactly on time (0ms offset = perfect Great).
                frames.Add(new SoundVisReplayFrame(hit.StartTime, hit.RequiredAction));

                // Release 50ms later so rapid consecutive notes on the same key still fire.
                frames.Add(new SoundVisReplayFrame(hit.StartTime + 50));
            }

            return new ModReplayData(
                new Replay { Frames = frames },
                new ModCreatedUser("osu!vis"));
        }
    }
}
