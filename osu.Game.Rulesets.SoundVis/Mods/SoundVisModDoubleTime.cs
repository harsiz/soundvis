using System;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.SoundVis.Mods
{
    public class SoundVisModDoubleTime : ModDoubleTime
    {
        // NC is mutually exclusive with DT (NC IS DT + pitch).
        public override Type[] IncompatibleMods => new[] { typeof(SoundVisModNightcore) };
    }
}
