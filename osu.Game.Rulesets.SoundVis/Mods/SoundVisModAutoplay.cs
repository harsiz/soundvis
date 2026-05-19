using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.SoundVis.Objects;

namespace osu.Game.Rulesets.SoundVis.Mods
{
    /// <summary>
    /// Autoplay: sets a flag on every DrawableSoundVisHitObject so it self-triggers
    /// at the perfect moment inside its own Update loop — no replay machinery required.
    /// CreateReplayData still needs a concrete implementation to satisfy ModAutoplay.
    /// </summary>
    public class SoundVisModAutoplay : ModAutoplay, IApplicableToDrawableHitObject
    {
        public void ApplyToDrawableHitObject(DrawableHitObject drawable)
        {
            if (drawable is DrawableSoundVisHitObject dh)
                dh.AutoPlay = true;
        }

        // ModAutoplay is abstract; return empty replay data so osu! doesn't error.
        public override ModReplayData CreateReplayData(IBeatmap beatmap, IReadOnlyList<Mod> mods)
            => new ModReplayData(new Replay(), new ModCreatedUser("osu!vis"));
    }
}
