using osu.Game.Configuration;
using osu.Game.Rulesets;

namespace osu.Game.Rulesets.SoundVis.Configuration
{
    public class SoundVisRulesetConfigManager : RulesetConfigManager<SoundVisRulesetSetting>
    {
        public SoundVisRulesetConfigManager(SettingsStore? store, RulesetInfo ruleset, int? variant = null)
            : base(store, ruleset, variant)
        {
        }

        protected override void InitialiseDefaults()
        {
            SetDefault(SoundVisRulesetSetting.ShowColors,          true);
            SetDefault(SoundVisRulesetSetting.ShowLightUp,         true);
            SetDefault(SoundVisRulesetSetting.SpinSpeedMultiplier, 1.0, 0.5, 3.0, 0.05);
            SetDefault(SoundVisRulesetSetting.BarGlowIntensity,    0.5, 0.0, 1.0, 0.05);
        }
    }
}
