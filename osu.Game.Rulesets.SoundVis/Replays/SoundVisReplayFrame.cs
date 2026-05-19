using System.Collections.Generic;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.SoundVis.Replays
{
    // ReplayFrame must inherit from osu.Game.Rulesets.Replays.ReplayFrame
    // so it satisfies the FramedReplayInputHandler<TFrame> constraint.
    public class SoundVisReplayFrame : ReplayFrame
    {
        public List<SoundVisAction> Actions { get; } = new List<SoundVisAction>();

        public SoundVisReplayFrame(double time) : base(time) { }

        public SoundVisReplayFrame(double time, SoundVisAction action) : base(time)
        {
            Actions.Add(action);
        }
    }
}
