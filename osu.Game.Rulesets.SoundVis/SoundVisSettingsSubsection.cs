using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets;

namespace osu.Game.Rulesets.SoundVis
{
    public partial class SoundVisSettingsSubsection : RulesetSettingsSubsection
    {
        protected override string Header => "SoundVis";

        public SoundVisSettingsSubsection(Ruleset ruleset)
            : base(ruleset)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            var config = (SoundVisConfigManager)Config;

            Children = new Drawable[]
            {
                new SettingsSlider<int>
                {
                    LabelText = "Bar count",
                    Current = config.GetBindable<int>(SoundVisSetting.BarCount),
                    KeyboardStep = 10,
                },
                new SettingsSlider<double>
                {
                    LabelText = "Rainbow speed (ms per cycle)",
                    Current = config.GetBindable<double>(SoundVisSetting.RainbowSpeed),
                    KeyboardStep = 500,
                },
            };
        }
    }
}
