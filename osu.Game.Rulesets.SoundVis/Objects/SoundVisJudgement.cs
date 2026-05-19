using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.SoundVis.Objects
{
    public class SoundVisJudgement : Judgement
    {
        // Perfect is the top tier — gives access to all sub-results.
        public override HitResult MaxResult => HitResult.Perfect;

        protected override double HealthIncreaseFor(HitResult result) => result switch
        {
            // Normal fixed increments — no weird halving behaviour.
            HitResult.Perfect => 0.06,
            HitResult.Good    => 0.04,
            HitResult.Ok      => 0.01,
            HitResult.Meh     => 0.00,  // neutral — doesn't kill you, doesn't heal you
            HitResult.Miss    => -0.08, // ~13 consecutive misses from full health = dead
            _                 => 0,
        };
    }
}
