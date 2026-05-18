using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.SoundVis.Objects
{
    // We don't really have "hit objects" — this is a music visualiser, not a game mode.
    // This exists purely so osu! lazer's ruleset machinery is happy.
    public class SoundVisHitObject : HitObject
    {
    }
}
