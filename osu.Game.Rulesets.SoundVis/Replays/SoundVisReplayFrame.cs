using System.Collections.Generic;
using osu.Game.Replays;

namespace osu.Game.Rulesets.SoundVis.Replays
{
    public class SoundVisReplayFrame : ReplayFrame
    {
        /// <summary>All actions that are held down at this point in time.</summary>
        public List<SoundVisAction> Actions { get; } = new List<SoundVisAction>();

        public SoundVisReplayFrame(double time) : base(time) { }

        public SoundVisReplayFrame(double time, SoundVisAction action) : base(time)
        {
            Actions.Add(action);
        }
    }
}
