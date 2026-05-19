using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.SoundVis.Beatmaps;
using osu.Game.Rulesets.SoundVis.Mods;
using osu.Game.Rulesets.SoundVis.UI;
using osu.Game.Rulesets.UI;
using osu.Framework.Input.Bindings;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.SoundVis
{
    public class SoundVisRuleset : Ruleset
    {
        // "soundvis" is the internal ID; "osu!vis" is the display name.
        public override string Description => "osu!vis";
        public override string ShortName => "soundvis";
        public override string PlayingVerb => "vibing to";

        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod>? mods = null)
            => new DrawableSoundVisRuleset(this, beatmap, mods);

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap)
            => new SoundVisBeatmapConverter(beatmap, this);

        public override DifficultyCalculator CreateDifficultyCalculator(IWorkingBeatmap beatmap)
            => new SoundVisDifficultyCalculator(RulesetInfo, beatmap);

        public override PerformanceCalculator CreatePerformanceCalculator()
            => new SoundVisPerformanceCalculator(this);

        public override IEnumerable<Mod> GetModsFor(ModType type) => type switch
        {
            ModType.DifficultyReduction => new Mod[] { new SoundVisModNoFail() },
            ModType.DifficultyIncrease  => new Mod[] { new SoundVisModHardRock(), new SoundVisModHarderHardRock() },
            ModType.Automation          => new Mod[] { new SoundVisModAutoplay() },
            _ => [],
        };

        public override IEnumerable<KeyBinding> GetDefaultKeyBindings(int variant = 0) => new[]
        {
            new KeyBinding(InputKey.Q, SoundVisAction.TopLeft),
            new KeyBinding(InputKey.E, SoundVisAction.TopRight),
            new KeyBinding(InputKey.Z, SoundVisAction.BottomLeft),
            new KeyBinding(InputKey.C, SoundVisAction.BottomRight),
        };

        public override ISkinTransformer? CreateSkinTransformer(ISkin skin, IBeatmap beatmap)
            => new SoundVisSkinTransformer(skin);

        public override Drawable CreateIcon() => new SoundVisIcon();
    }
}
