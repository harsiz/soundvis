using System.Collections.Generic;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.UI;
using osu.Game.Scoring;
using osuTK;

namespace osu.Game.Rulesets.SoundVis.Replays
{
    public class SoundVisReplayRecorder : ReplayRecorder<SoundVisAction>
    {
        private readonly Score score;

        public SoundVisReplayRecorder(Score target)
            : base(target)
        {
            score = target;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            // Emit an initial "no actions" frame so the FramedReplayInputHandler
            // always has a valid starting state when replay playback begins.
            // Without this the handler may start mid-state on the first frame.
            score.Replay.Frames.Insert(0, new SoundVisReplayFrame(double.MinValue));
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
