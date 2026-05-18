using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.SoundVis.Objects;

namespace osu.Game.Rulesets.SoundVis.Mods
{
    /// <summary>Approach bars travel 2× as fast as normal.</summary>
    public class SoundVisModHarderHardRock : Mod, IApplicableToDrawableHitObject
    {
        public override string Name => "Harder Hard Rock";
        public override string Acronym => "HHR";
        public override ModType Type => ModType.DifficultyIncrease;
        public override double ScoreMultiplier => 1.12;
        public override LocalisableString Description => "Approach bars move 2x faster. Good luck.";
        public override IconUsage? Icon => FontAwesome.Solid.AngleDoubleUp;

        public void ApplyToDrawableHitObject(DrawableHitObject drawable)
        {
            if (drawable is DrawableSoundVisHitObject dh)
                dh.ApproachSpeedMultiplier = 2f;
        }
    }
}
