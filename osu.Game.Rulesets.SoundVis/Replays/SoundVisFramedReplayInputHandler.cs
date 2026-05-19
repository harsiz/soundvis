using System.Collections.Generic;
using osu.Framework.Input.StateChanges;
using osu.Game.Replays;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.SoundVis.Replays
{
    public class SoundVisFramedReplayInputHandler : FramedReplayInputHandler<SoundVisReplayFrame>
    {
        public SoundVisFramedReplayInputHandler(Replay replay) : base(replay) { }

        // Both key-press AND key-release frames must be important so the
        // FrameStabilityContainer doesn't advance the clock past them.
        // Without this, the clock can drift forward on "empty" frames and
        // hit objects time out as misses before the press event fires.
        protected override bool IsImportant(SoundVisReplayFrame frame) => true;

        protected override void CollectReplayInputs(List<IInput> inputs)
        {
            inputs.Add(new ReplayState<SoundVisAction>
            {
                PressedActions = CurrentFrame?.Actions ?? new List<SoundVisAction>(),
            });
        }
    }
}
