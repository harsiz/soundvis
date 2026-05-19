using osu.Framework.Allocation;
using osu.Framework.Graphics;
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

            Children = new Drawable[]
            {
                // ── Gameplay visuals ──────────────────────────────────────────────────
                new SettingsCheckbox
                {
                    LabelText   = "Colour-coded approach bars",
                    TooltipText = "Colour each approach bar by its required key quadrant. Disable for a cleaner look.",
                    Current     = config.GetBindable<bool>(SoundVisRulesetSetting.ShowColors),
                },
                new SettingsCheckbox
                {
                    LabelText   = "Next-note colour indicator",
                    TooltipText = "Show a coloured ring around the centre logo so you can see which key is coming next at a glance.",
                    Current     = config.GetBindable<bool>(SoundVisRulesetSetting.ShowNextNoteIndicator),
                },
                new SettingsCheckbox
                {
                    LabelText   = "Logo light-up on hit",
                    TooltipText = "Flash the osu!vis logo white when you hit a note.",
                    Current     = config.GetBindable<bool>(SoundVisRulesetSetting.ShowLightUp),
                },
                // ── Audio-reactive effects ────────────────────────────────────────────
                new SettingsCheckbox
                {
                    LabelText   = "Beat-flash side panels",
                    TooltipText = "Flash glowing panels on the screen edges when the music is loud or in kiai time.",
                    Current     = config.GetBindable<bool>(SoundVisRulesetSetting.ShowBeatFlashes),
                },
                new SettingsSlider<double>
                {
                    LabelText   = "Bar glow intensity",
                    TooltipText = "Strength of the soft additive glow around approach bars (0 = solid lines only).",
                    Current     = config.GetBindable<double>(SoundVisRulesetSetting.BarGlowIntensity),
                    KeyboardStep = 0.05f,
                },
                // ── Logo behaviour ────────────────────────────────────────────────────
                new SettingsSlider<double>
                {
                    LabelText    = "Logo spin speed",
                    TooltipText  = "Multiplier on the base spin speed of the osu!vis logo (0.5× – 3.0×).",
                    Current      = config.GetBindable<double>(SoundVisRulesetSetting.SpinSpeedMultiplier),
                    KeyboardStep = 0.05f,
                },
            };
        }
    }
}
