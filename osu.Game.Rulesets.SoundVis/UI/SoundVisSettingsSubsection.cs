using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.SoundVis.Configuration;

namespace osu.Game.Rulesets.SoundVis.UI
{
    /// <summary>
    /// Ruleset settings for osu!vis.
    /// <para>
    /// Uses the <see cref="SettingsItemV2"/> + <c>Form*</c> control family, which is the
    /// current osu! settings style (see <c>ManiaSettingsSubsection</c>). The older
    /// <c>SettingsCheckbox</c>/<c>SettingsSlider</c> controls still compile but render
    /// in the pre-redesign flat style, which looks out of place next to the built-in
    /// rulesets.
    /// </para>
    /// </summary>
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
                new SettingsItemV2(new FormCheckBox
                {
                    Caption  = "Colour-coded approach bars",
                    HintText = "Colour each approach bar by its required key quadrant. Disable for a cleaner look.",
                    Current  = config.GetBindable<bool>(SoundVisRulesetSetting.ShowColors),
                })
                {
                    Keywords = new[] { "color", "colour" },
                },
                new SettingsItemV2(new FormCheckBox
                {
                    Caption  = "Next-note colour indicator",
                    HintText = "Show a coloured ring around the centre logo so you can see which key is coming next at a glance.",
                    Current  = config.GetBindable<bool>(SoundVisRulesetSetting.ShowNextNoteIndicator),
                }),
                new SettingsItemV2(new FormCheckBox
                {
                    Caption  = "Logo light-up on hit",
                    HintText = "Flash the osu!vis logo white when you hit a note.",
                    Current  = config.GetBindable<bool>(SoundVisRulesetSetting.ShowLightUp),
                }),

                // ── Input ─────────────────────────────────────────────────────────────
                new SettingsItemV2(new FormCheckBox
                {
                    Caption  = "Touch controls",
                    HintText = "Split the screen into four quadrant tap zones. Works with both touchscreen taps and mouse clicks. The zones stay hidden until first used, so keyboard play is unaffected.",
                    Current  = config.GetBindable<bool>(SoundVisRulesetSetting.TouchControls),
                })
                {
                    Keywords = new[] { "touch", "mouse", "tap", "mobile" },
                },

                // ── Audio-reactive effects ────────────────────────────────────────────
                new SettingsItemV2(new FormCheckBox
                {
                    Caption  = "Beat-flash side panels",
                    HintText = "Flash glowing panels on the screen edges when the music is loud or in kiai time.",
                    Current  = config.GetBindable<bool>(SoundVisRulesetSetting.ShowBeatFlashes),
                }),
                new SettingsItemV2(new FormSliderBar<double>
                {
                    Caption      = "Bar glow intensity",
                    Current      = config.GetBindable<double>(SoundVisRulesetSetting.BarGlowIntensity),
                    KeyboardStep = 0.05f,
                    LabelFormat  = v => v <= 0 ? "Off (solid lines)" : $"{v:P0}",
                }),

                // ── Logo behaviour ────────────────────────────────────────────────────
                new SettingsItemV2(new FormSliderBar<double>
                {
                    Caption      = "Logo spin speed",
                    Current      = config.GetBindable<double>(SoundVisRulesetSetting.SpinSpeedMultiplier),
                    KeyboardStep = 0.05f,
                    LabelFormat  = v => $"{v:0.00}x",
                }),
            };
        }
    }
}
