using System;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.SoundVis.Mods
{
    public class SoundVisModNightcore : ModNightcore
    {
        public override Type[] IncompatibleMods => new[] { typeof(SoundVisModDoubleTime) };
    }
}
