using osu.Game.Beatmaps;
using osu.Game.Scoring;
using osu.Game.Screens.Play;
using osu.Game.Screens.Ranking;

namespace osu.Game.Rulesets.SoundVis.UI
{
    public partial class SoundVisPlayer : Player
    {
        public SoundVisPlayer()
            : base(new PlayerConfiguration
            {
                AllowRestart = true,
                ShowResults = true,
            })
        {
        }

        protected override GameplayClockContainer CreateGameplayClockContainer(WorkingBeatmap beatmap, double gameplayStart)
            => base.CreateGameplayClockContainer(beatmap, 0);

        protected override bool CheckModsAllowFailure() => false;

        protected override ResultsScreen CreateResults(ScoreInfo score) => new SoloResultsScreen(score);
    }
}
