namespace osu.Game.Rulesets.SoundVis
{
    public enum SoundVisRulesetSetting
    {
        /// <summary>Colour-code approach bars by quadrant. Off = all white.</summary>
        ShowColors,

        /// <summary>Flash the logo white when a direction-reversal is triggered.</summary>
        ShowLightUp,

        /// <summary>Multiplier applied to the logo's base spin speed (0.5 – 3.0).</summary>
        SpinSpeedMultiplier,

        /// <summary>Opacity of the soft additive glow around each approach bar (0 – 1).</summary>
        BarGlowIntensity,

        /// <summary>Show a coloured ring around the logo indicating which key to press next.</summary>
        ShowNextNoteIndicator,

        /// <summary>Flash the screen edges on loud / kiai beats.</summary>
        ShowBeatFlashes,
    }
}
