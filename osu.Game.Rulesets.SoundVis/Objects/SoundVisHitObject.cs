using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osuTK;

namespace osu.Game.Rulesets.SoundVis.Objects
{
    public class SoundVisHitObject : HitObject
    {
        // Normalised position [0,1] in both axes — scaled to playfield size in the drawable.
        public Vector2 Position { get; set; }

        // Distance from the previous object in normalised units.
        // Used for scoring (longer jump = more points) and difficulty.
        public float JumpDistance { get; set; }

        public override Judgement CreateJudgement() => new SoundVisJudgement();
    }
}
