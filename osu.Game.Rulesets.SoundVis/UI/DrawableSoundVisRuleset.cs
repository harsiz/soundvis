using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.SoundVis.Objects;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.SoundVis.UI
{
    public partial class DrawableSoundVisRuleset : DrawableRuleset<SoundVisHitObject>
    {
        public DrawableSoundVisRuleset(SoundVisRuleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod>? mods = null)
            : base(ruleset, beatmap, mods)
        {
        }

        protected override Playfield CreatePlayfield() => new SoundVisPlayfield();

        protected override ResumeOverlay? CreateResumeOverlay() => null;

        protected override PassThroughInputManager CreateInputManager() => new PassThroughInputManager();

        public override DrawableHitObject<SoundVisHitObject>? CreateDrawableRepresentation(SoundVisHitObject h) => null;
    }
}
