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

        protected override IEnumerable<SoundVisHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap, CancellationToken cancellationToken)
            => [];

        protected override Beatmap<SoundVisHitObject> CreateBeatmap() => new SoundVisBeatmap();

        public override IBeatmap Convert(IBeatmap beatmap, CancellationToken cancellationToken = default)
        {
            var converted = (SoundVisBeatmap)base.Convert(beatmap, cancellationToken);

            // osu! blocks entry to the player if there are zero hit objects.
            // Add one invisible dummy at t=0 so the preflight check passes.
            if (converted.HitObjects.Count == 0)
                converted.HitObjects.Add(new SoundVisHitObject { StartTime = 0 });

            return converted;
        }
    }
}
