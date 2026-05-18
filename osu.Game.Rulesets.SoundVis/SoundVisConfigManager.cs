using osu.Framework.Configuration;
using osu.Framework.Platform;
using osu.Game.Configuration;
using osu.Game.Rulesets;

namespace osu.Game.Rulesets.SoundVis
{
    public class SoundVisConfigManager : RulesetConfigManager<SoundVisSetting>
    {
        public SoundVisConfigManager(SettingsStore? settings, RulesetInfo ruleset)
            : base(settings, ruleset)
        {
        }

        protected override void InitialiseDefaults()
        {
            SetDefault(SoundVisSetting.BarCount, 200, 50, 400);
            SetDefault(SoundVisSetting.RainbowSpeed, 8000.0, 1000.0, 30000.0);
        }
    }

    public enum SoundVisSetting
    {
        BarCount,
        RainbowSpeed,
    }
}
