using osu.Game.Beatmaps;
using osu.Game.Screens.Play;

namespace osu.Game.Rulesets.SoundVis.UI
{
    public partial class SoundVisPlayer : Player
    {
        public SoundVisPlayer()
            : base(new PlayerConfiguration
            {
                AllowRestart = false,
                ShowResults = false,
            })
        {
        }

        protected override GameplayClockContainer CreateGameplayClockContainer(WorkingBeatmap beatmap, double gameplayStart)
            => base.CreateGameplayClockContainer(beatmap, 0);

        protected override bool CheckModsAllowFailure() => false;
    }
}
