using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osuTK.Graphics;

namespace osu.Game.Rulesets.SoundVis.Objects
{
    public class SoundVisHitObject : HitObject
    {
        public float ApproachAngle { get; set; }
        public SoundVisAction RequiredAction { get; set; }

        // Derived from RequiredAction — no need to store separately
        public Color4 BarColour => SoundVisActionHelper.GetColour(RequiredAction);

        public override Judgement CreateJudgement() => new SoundVisJudgement();
    }
}
