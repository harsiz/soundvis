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
        private const float MIN_JUMP = 0.35f; // minimum normalised distance between objects
        private const float MARGIN = 0.07f;   // keep logo away from screen edges

        public SoundVisBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
            : base(beatmap, ruleset)
        {
        }

        public override bool CanConvert() => true;

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

                // Retry until we get a position that is far enough from the previous one.
                Vector2 pos;
                int tries = 0;
                do
                {
                    float x = MARGIN + (float)(rng.NextDouble() * (1f - MARGIN * 2));
                    float y = MARGIN + (float)(rng.NextDouble() * (1f - MARGIN * 2));
                    pos = new Vector2(x, y);
                    tries++;
                } while (!first && (pos - prevPos).Length < MIN_JUMP && tries < 20);

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
