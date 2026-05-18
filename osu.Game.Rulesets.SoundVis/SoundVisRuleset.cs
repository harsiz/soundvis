using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.SoundVis.Beatmaps;
using osu.Game.Rulesets.SoundVis.UI;
using osu.Game.Rulesets.UI;
using osu.Framework.Input.Bindings;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.SoundVis
{
    public class SoundVisRuleset : Ruleset
    {
        public override string Description => "SoundVis";
        public override string ShortName => "soundvis";
        public override string PlayingVerb => "vibing to";

        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod>? mods = null)
            => new DrawableSoundVisRuleset(this, beatmap, mods);

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap)
            => new SoundVisBeatmapConverter(beatmap, this);

        public override DifficultyCalculator CreateDifficultyCalculator(IWorkingBeatmap beatmap)
            => new SoundVisDifficultyCalculator(RulesetInfo, beatmap);

        public override IEnumerable<Mod> GetModsFor(ModType type) => [];

        public override IEnumerable<KeyBinding> GetDefaultKeyBindings(int variant = 0) => [];

        public override ISkinTransformer? CreateSkinTransformer(ISkin skin, IBeatmap beatmap)
            => new SoundVisSkinTransformer(skin);

        public override Drawable CreateIcon() => new SoundVisIcon();
    }
}
