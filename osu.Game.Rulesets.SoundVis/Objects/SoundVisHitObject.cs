using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.SoundVis.Objects;

namespace osu.Game.Rulesets.SoundVis.Objects
{
    public class SoundVisHitObject : HitObject
    {
        // Which side the approach bar comes from (degrees: 0=top, 90=right, 180=bottom, 270=left)
        public float ApproachAngle { get; set; }

        public override Judgement CreateJudgement() => new SoundVisJudgement();
    }
}
