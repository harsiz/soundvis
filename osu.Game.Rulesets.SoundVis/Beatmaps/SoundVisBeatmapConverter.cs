using System;
using System.Collections.Generic;
using System.Threading;
using osu.Game.Audio;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.SoundVis.Objects;

namespace osu.Game.Rulesets.SoundVis.Beatmaps
{
    public class SoundVisBeatmapConverter : BeatmapConverter<SoundVisHitObject>
    {
        // Skip objects that are closer than this — filters true simultaneity
        // (e.g. mania chords) without losing fast streams (83ms at 180BPM 1/4)
        private const double MIN_GAP_MS = 60;

        private double lastEmittedTime = double.MinValue;

        public SoundVisBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
            : base(beatmap, ruleset)
        {
        }

        public override bool CanConvert() => true;

        protected override IEnumerable<SoundVisHitObject> ConvertHitObject(
            HitObject original, IBeatmap beatmap, CancellationToken cancellationToken)
        {
            if (original.StartTime - lastEmittedTime < MIN_GAP_MS)
                yield break;

            lastEmittedTime = original.StartTime;

            var obj = new SoundVisHitObject
            {
                StartTime = original.StartTime,
                ApproachAngle = GetApproachAngle(original, beatmap),
            };
            obj.Samples.Add(new HitSampleInfo(HitSampleInfo.HIT_NORMAL));
            yield return obj;
        }

        private static float GetApproachAngle(HitObject original, IBeatmap beatmap)
        {
            // osu! standard — angle from playfield centre to hit object position
            // (bar comes FROM the direction the object is relative to centre)
            if (original is IHasPosition pos)
            {
                float dx = pos.Position.X - 256f;
                float dy = pos.Position.Y - 192f;
                if (MathF.Abs(dx) + MathF.Abs(dy) > 25f)
                {
                    float angle = MathF.Atan2(dx, -dy) * 180f / MathF.PI;
                    return angle < 0 ? angle + 360f : angle;
                }
            }

            // mania — spread columns evenly around 360°
            if (original is IHasColumn col)
            {
                int keys = Math.Max((int)beatmap.Difficulty.CircleSize, 1);
                return (float)col.Column / keys * 360f;
            }

            // fallback (taiko, catch, etc.) — golden-ratio spread by time
            return (float)((original.StartTime * 137.508) % 360);
        }
    }
}
