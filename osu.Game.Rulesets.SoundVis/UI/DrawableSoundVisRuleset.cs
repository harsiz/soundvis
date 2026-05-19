using System.Collections.Generic;
using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.SoundVis.Objects;
using osu.Game.Rulesets.SoundVis.Replays;
using osu.Game.Rulesets.UI;
using osu.Game.Screens.Play;
using osu.Framework.Allocation;

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

        protected override PassThroughInputManager CreateInputManager() => new SoundVisInputManager(Ruleset?.RulesetInfo);

        public override DrawableHitObject<SoundVisHitObject>? CreateDrawableRepresentation(SoundVisHitObject h)
            => new DrawableSoundVisHitObject(h);

        protected override ReplayInputHandler CreateReplayInputHandler(Replay replay)
            => new SoundVisFramedReplayInputHandler(replay);
    }
}
