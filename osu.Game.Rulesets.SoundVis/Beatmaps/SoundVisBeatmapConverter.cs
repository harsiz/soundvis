using System;
using System.Collections.Generic;
using System.Threading;
using osu.Game.Audio;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.SoundVis.Objects;

namespace osu.Game.Rulesets.SoundVis.Beatmaps
{
    public class SoundVisBeatmapConverter : BeatmapConverter<SoundVisHitObject>
    {
        private static readonly float[] Angles = { 0f, 90f, 180f, 270f };

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

            double time = startTime;
            int angleIndex = 0;

            while (time <= endTime)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var timingPoint = beatmap.ControlPointInfo.TimingPointAt(time);
                double beatLength = timingPoint.BeatLength;
                double bpm = 60_000.0 / beatLength;

                // Denser pattern at higher BPM so harder maps feel faster
                double interval = bpm >= 160 ? beatLength          // every beat
                                : bpm >= 120 ? beatLength * 1.5    // every 1.5 beats
                                :              beatLength * 2;      // every 2 beats

                // Alternate sides, never the same twice in a row
                int next;
                do { next = rng.Next(Angles.Length); } while (next == angleIndex && Angles.Length > 1);
                angleIndex = next;

                var obj = new SoundVisHitObject
                {
                    StartTime = time,
                    ApproachAngle = Angles[angleIndex],
                };

                // Skin hitsound — plays automatically on hit via DrawableHitObject.PlaySamples()
                obj.Samples.Add(new HitSampleInfo(HitSampleInfo.HIT_NORMAL));

                yield return obj;
                time += interval;
            }
        }
    }
}
