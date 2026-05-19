using System.Collections.Generic;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.UI;
using osu.Game.Scoring;
using osuTK;

namespace osu.Game.Rulesets.SoundVis.Replays
{
    public class SoundVisReplayRecorder : ReplayRecorder<SoundVisAction>
    {
        public SoundVisReplayRecorder(Score target)
            : base(target)
        {
        }

        protected override ReplayFrame HandleFrame(
            Vector2 mousePosition,
            List<SoundVisAction> actions,
            ReplayFrame previousFrame)
        {
            var frame = new SoundVisReplayFrame(Time.Current);
            frame.Actions.AddRange(actions);
            return frame;
        }
    }
}
