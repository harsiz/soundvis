using osu.Framework.Allocation;
using osu.Framework.Localisation;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.SoundVis.Configuration;

namespace osu.Game.Rulesets.SoundVis.UI
{
    public partial class SoundVisSettingsSubsection : RulesetSettingsSubsection
    {
        protected override LocalisableString Header => "osu!vis";

        public SoundVisSettingsSubsection(SoundVisRuleset ruleset)
            : base(ruleset)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            var config = (SoundVisRulesetConfigManager)Config;

            Children = new[]
            {
                new SettingsCheckbox
                {
                    LabelText = "Colour-coded approach bars",
                    TooltipText = "Colour each bar by its required key quadrant. Disable for a cleaner look.",
                    Current = config.GetBindable<bool>(SoundVisRulesetSetting.ShowColors),
                },
                new SettingsCheckbox
                {
                    LabelText = "Logo light-up on hit",
                    TooltipText = "Flash the osu!vis logo white when you trigger a direction reversal.",
                    Current = config.GetBindable<bool>(SoundVisRulesetSetting.ShowLightUp),
                },
                new SettingsSlider<double>
                {
                    LabelText = "Logo spin speed",
                    TooltipText = "Multiplier on the base spin speed of the osu!vis logo (0.5× – 3.0×).",
                    Current = config.GetBindable<double>(SoundVisRulesetSetting.SpinSpeedMultiplier),
                    KeyboardStep = 0.05f,
                },
                new SettingsSlider<double>
                {
                    LabelText = "Bar glow intensity",
                    TooltipText = "Strength of the soft additive glow around approach bars. 0 = solid lines only.",
                    Current = config.GetBindable<double>(SoundVisRulesetSetting.BarGlowIntensity),
                    KeyboardStep = 0.05f,
                },
            };
        }
    }
}
