using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using osu.Game.Beatmaps;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Screens.Play;

namespace osu.Game.Rulesets.SoundVis.UI
{
    /// <summary>
    /// Overrides the default Player to strip away the HUD/score/fail stuff
    /// and just show our visualiser full-screen with a blurred beatmap background.
    /// </summary>
    public partial class SoundVisPlayer : Player
    {
        [Resolved]
        private IBindable<WorkingBeatmap> beatmap { get; set; } = null!;

        public SoundVisPlayer()
            : base(new PlayerConfiguration
            {
                AllowRestart = false,
                ShowResults = false,
            })
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
        }

        protected override GameplayClockContainer CreateGameplayClockContainer(WorkingBeatmap beatmap, double gameplayStart)
        {
            // Start from the very beginning of the track
            return base.CreateGameplayClockContainer(beatmap, 0);
        }

        protected override bool CheckModsAllowFailure() => false;

        // Make the background nice and blurry — music client vibes
        protected override float BackgroundOpacityWhenPlaying => 0.3f;
    }
}
