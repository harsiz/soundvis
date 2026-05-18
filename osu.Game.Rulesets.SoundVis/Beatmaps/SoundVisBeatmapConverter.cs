using System;
using System.Collections.Generic;
using System.Threading;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.SoundVis.Objects;
using osuTK;

namespace osu.Game.Rulesets.SoundVis.Beatmaps
{
    public class SoundVisBeatmapConverter : BeatmapConverter<SoundVisHitObject>
    {
        public SoundVisBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
            : base(beatmap, ruleset)
        {
        }

        public override bool CanConvert() => true;

        // All objects are generated on the first call; subsequent calls yield nothing.
        private bool objectsEmitted;

        protected override IEnumerable<SoundVisHitObject> ConvertHitObject(
            HitObject original, IBeatmap beatmap, CancellationToken cancellationToken)
        {
            if (objectsEmitted) yield break;
            objectsEmitted = true;

            int seed = (int)(beatmap.BeatmapInfo.OnlineID ^ (beatmap.BeatmapInfo.OnlineID >> 16));
            if (seed == 0) seed = beatmap.BeatmapInfo.GetHashCode();
            var rng = new Random(seed);

            double startTime = beatmap.ControlPointInfo.TimingPoints.Count > 0
                ? beatmap.ControlPointInfo.TimingPoints[0].Time
                : 0;

            double endTime = beatmap.HitObjects.Count > 0
                ? beatmap.HitObjects[^1].GetEndTime()
                : startTime + 30_000;

            if (endTime <= startTime)
                endTime = startTime + 30_000;

            var prevPos = new Vector2(0.5f, 0.5f);
            bool first = true;
            double time = startTime;

            while (time <= endTime)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var timingPoint = beatmap.ControlPointInfo.TimingPointAt(time);
                double beatLength = timingPoint.BeatLength;

                float x = (float)(rng.NextDouble() * 0.7 + 0.15);
                float y = (float)(rng.NextDouble() * 0.7 + 0.15);
                var pos = new Vector2(x, y);

                float jumpDistance = first ? 0f : (pos - prevPos).Length;
                first = false;

                yield return new SoundVisHitObject
                {
                    StartTime = time,
                    Position = pos,
                    JumpDistance = jumpDistance,
                };

                prevPos = pos;
                time += beatLength * 2;
            }
        }
    }
}
