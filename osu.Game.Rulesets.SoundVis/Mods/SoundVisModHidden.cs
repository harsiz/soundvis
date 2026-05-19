using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.SoundVis.Objects;

namespace osu.Game.Rulesets.SoundVis.Mods
{
    /// <summary>
    /// Approach bars fade away mid-approach and only briefly reappear near the hit window.
    /// </summary>
    public class SoundVisModHidden : Mod, IApplicableToDrawableHitObject
    {
        public override string Name => "Hidden";
        public override string Acronym => "HD";
        public override ModType Type => ModType.DifficultyIncrease;
        public override double ScoreMultiplier => 1.06;
        public override LocalisableString Description => "Approach bars disappear before reaching the logo.";
        public override IconUsage? Icon => FontAwesome.Solid.EyeSlash;

        public void ApplyToDrawableHitObject(DrawableHitObject drawable)
        {
            if (drawable is DrawableSoundVisHitObject dh)
                dh.Hidden = true;
        }
    }
}
