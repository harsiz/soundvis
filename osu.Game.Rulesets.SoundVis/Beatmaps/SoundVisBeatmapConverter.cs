using System.Collections.Generic;
using System.Threading;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.SoundVis.Objects;

namespace osu.Game.Rulesets.SoundVis.Beatmaps
{
    public class SoundVisBeatmapConverter : BeatmapConverter<SoundVisHitObject>
    {
        public SoundVisBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
            : base(beatmap, ruleset)
        {
        }

        public override bool CanConvert() => true;

        // Emit one invisible dummy on the very first hit object, drop the rest.
        // osu! refuses to enter the player with zero hit objects, so we need at least one.
        private bool dummyEmitted;

        protected override IEnumerable<SoundVisHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap, CancellationToken cancellationToken)
        {
            if (!dummyEmitted)
            {
                dummyEmitted = true;
                yield return new SoundVisHitObject { StartTime = 0 };
            }
        }
    }
}
