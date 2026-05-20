using System;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.SoundVis.Objects
{
    /// <summary>
    /// A single SoundVis hit object.
    /// <para>
    /// <see cref="IHasPosition"/> is implemented so the <c>LegacyBeatmapEncoder</c> can
    /// write this ruleset's beatmaps to disk. The X/Y position encodes
    /// <see cref="ApproachAngle"/> as a point on a radius-100 circle around the
    /// osu!standard playfield centre (256, 192):
    /// <code>
    ///   X = 256 + sin(ApproachAngle°) * 100
    ///   Y = 192 − cos(ApproachAngle°) * 100
    /// </code>
    /// The <see cref="Beatmaps.SoundVisBeatmapConverter"/> recovers the angle from
    /// <c>IHasPosition</c> via <c>Atan2(dx, −dy)</c>, completing the round-trip.
    /// </para>
    /// </summary>
    public class SoundVisHitObject : HitObject, IHasPosition
    {
        // ── Approach angle ────────────────────────────────────────────────────────

        private float _approachAngle;

        public float ApproachAngle
        {
            get => _approachAngle;
            set
            {
                _approachAngle = value;
                // Keep the encoded position in sync
                float rad = value * MathF.PI / 180f;
                _position = new Vector2(256f + MathF.Sin(rad) * 100f,
                                       192f - MathF.Cos(rad) * 100f);
            }
        }

        public SoundVisAction RequiredAction { get; set; }

        // Derived from RequiredAction — no need to store separately
        public Color4 BarColour => SoundVisActionHelper.GetColour(RequiredAction);

        /// <summary>
        /// Scales health deltas for this object; set by the beatmap converter based on estimated BPS.
        /// Clamped to [0.15, 1.0] — 1.0 at ≥6★, 0.15 on easy maps.
        /// </summary>
        public double HealthMultiplier { get; set; } = 1.0;

        public override Judgement CreateJudgement() => new SoundVisJudgement(HealthMultiplier);

        // ── IHasPosition — encodes ApproachAngle for LegacyBeatmapEncoder ────────

        private Vector2 _position;

        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                // Recover angle from position so both stay in sync when loaded
                float dx = value.X - 256f;
                float dy = value.Y - 192f;
                if (MathF.Abs(dx) + MathF.Abs(dy) > 25f)
                {
                    float angle = MathF.Atan2(dx, -dy) * 180f / MathF.PI;
                    _approachAngle = angle < 0 ? angle + 360f : angle;
                    RequiredAction = SoundVisActionHelper.FromAngle(_approachAngle);
                }
            }
        }

        // IHasXPosition / IHasYPosition require settable X and Y.
        // Route through Position so everything stays in sync.
        public float X
        {
            get => _position.X;
            set => Position = new Vector2(value, _position.Y);
        }

        public float Y
        {
            get => _position.Y;
            set => Position = new Vector2(_position.X, value);
        }
    }
}
