using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.SoundVis.Mods
{
    public class SoundVisModNoFail : ModNoFail
    {
        // Standard NF is 0.5x; SoundVis makes it harsher at 0.25x to discourage abuse.
        public override double ScoreMultiplier => 0.25;
    }
}
