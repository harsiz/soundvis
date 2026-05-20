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

        // Cached health multiplier — computed once per Convert() call.
        private double _cachedHealthMultiplier = -1;

        public SoundVisBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
            : base(beatmap, ruleset)
        {
        }

        public override bool CanConvert() => true;

        /// <summary>
        /// Estimates a health drain multiplier from the source beatmap's BPS using the same
        /// quadratic formula as the star rating calculator.  Clamped to [0.15, 1.0] so
        /// easy maps drain slowly and 6★+ maps drain at full speed.
        /// </summary>
        private double GetHealthMultiplier(IBeatmap beatmap)
        {
            if (_cachedHealthMultiplier >= 0)
                return _cachedHealthMultiplier;

            var objs = beatmap.HitObjects;
            if (objs.Count < 2)
                return _cachedHealthMultiplier = 0.15;

            double duration = (objs[^1].StartTime - objs[0].StartTime) / 1000.0;
            double bps      = duration > 0 ? objs.Count / duration : 1.0;

            // Mirrors the star formula: 4/9 * max(0, bps-2)^2 + 1
            double shifted  = Math.Max(0.0, bps - 2.0);
            double estStars = shifted * shifted * (4.0 / 9.0) + 1.0;

            return _cachedHealthMultiplier = Math.Clamp(estStars / 6.0, 0.15, 1.0);
        }

        protected override IEnumerable<SoundVisHitObject> ConvertHitObject(
            HitObject original, IBeatmap beatmap, CancellationToken cancellationToken)
        {
            if (original.StartTime - lastEmittedTime < MIN_GAP_MS)
                yield break;

            lastEmittedTime = original.StartTime;

            float angle = GetApproachAngle(original, beatmap);
            var obj = new SoundVisHitObject
            {
                StartTime        = original.StartTime,
                ApproachAngle    = angle,
                RequiredAction   = SoundVisActionHelper.FromAngle(angle),
                HealthMultiplier = GetHealthMultiplier(beatmap),
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
