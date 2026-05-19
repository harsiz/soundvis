using System.Collections.Generic;
using osu.Framework.Input.StateChanges;
using osu.Game.Replays;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.SoundVis.Replays
{
    public class SoundVisFramedReplayInputHandler : FramedReplayInputHandler<SoundVisReplayFrame>
    {
        public SoundVisFramedReplayInputHandler(Replay replay) : base(replay) { }

        protected override bool IsImportant(SoundVisReplayFrame frame) => frame.Actions.Count > 0;

        protected override void CollectReplayInputs(List<IInput> inputs)
        {
            inputs.Add(new ReplayState<SoundVisAction>
            {
                PressedActions = CurrentFrame?.Actions ?? new List<SoundVisAction>(),
            });
        }
    }
}
