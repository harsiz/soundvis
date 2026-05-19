using osu.Game.Beatmaps;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.SoundVis.Objects;

namespace osu.Game.Rulesets.SoundVis.Replays
{
    public class SoundVisAutoGenerator : AutoGenerator<SoundVisReplayFrame>
    {
        public SoundVisAutoGenerator(IBeatmap beatmap) : base(beatmap) { }

        protected override void GenerateFrames()
        {
            // base.Beatmap is IBeatmap; HitObjects are SoundVisHitObject instances
            foreach (var obj in Beatmap.HitObjects)
            {
                if (obj is not SoundVisHitObject hitObject)
                    continue;

                // Press the correct key at perfect timing
                Frames.Add(new SoundVisReplayFrame(hitObject.StartTime, hitObject.RequiredAction));
                // Release after the minimum key-up delay
                Frames.Add(new SoundVisReplayFrame(hitObject.StartTime + KEY_UP_DELAY));
            }
        }
    }
}
