using System;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.SoundVis.Objects;

namespace osu.Game.Rulesets.SoundVis.Mods
{
    /// <summary>Approach bars travel 1.5× faster, leaving less reaction time.</summary>
    public class SoundVisModHardRock : Mod, IApplicableToDrawableHitObject
    {
        public override string Name => "Hard Rock";
        public override string Acronym => "HR";
        public override ModType Type => ModType.DifficultyIncrease;
        public override double ScoreMultiplier => 1.06;
        public override Type[] IncompatibleMods => new[] { typeof(SoundVisModHarderHardRock) };
        public override LocalisableString Description => "Approach bars move 1.5x faster.";
        public override IconUsage? Icon => FontAwesome.Solid.AngleDoubleUp;

        public void ApplyToDrawableHitObject(DrawableHitObject drawable)
        {
            if (drawable is DrawableSoundVisHitObject dh)
                dh.ApproachSpeedMultiplier = 1.5f;
        }
    }
}
