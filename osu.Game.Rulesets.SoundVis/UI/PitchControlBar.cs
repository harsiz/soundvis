using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;

namespace osu.Game.Rulesets.SoundVis.UI
{
    /// <summary>
    /// Bottom bar for pitch / speed control. Adjusts track frequency (vinyl-speed style).
    /// Keeps a single BindableDouble registered on the track and updates it in place
    /// rather than re-adding on every change.
    /// </summary>
    public partial class PitchControlBar : CompositeDrawable
    {
        // This bindable is what actually drives the audio engine.
        private readonly BindableDouble frequencyAdjust = new BindableDouble(1.0)
        {
            MinValue = 0.5,
            MaxValue = 2.0,
            Precision = 0.01,
            Default = 1.0,
        };

        private OsuSpriteText label = null!;

        [Resolved]
        private IBindable<WorkingBeatmap> beatmap { get; set; } = null!;

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            AutoSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                new Container
                {
                    AutoSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = 8,
                    Padding = new MarginPadding { Horizontal = 20, Vertical = 10 },
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = OsuColour.Gray(0.1f),
                            Alpha = 0.88f,
                        },
                        new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Direction = FillDirection.Horizontal,
                            Spacing = new osuTK.Vector2(12, 0),
                            Children = new Drawable[]
                            {
                                new SpriteIcon
                                {
                                    Icon = FontAwesome.Solid.Music,
                                    Size = new osuTK.Vector2(16),
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Colour = colours.Pink,
                                },
                                new OsuSpriteText
                                {
                                    Text = "PITCH",
                                    Font = OsuFont.GetFont(size: 12, weight: FontWeight.Bold),
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Colour = colours.Pink,
                                },
                                new OsuSliderBar<double>
                                {
                                    Width = 260,
                                    Height = 20,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Current = frequencyAdjust,
                                },
                                label = new OsuSpriteText
                                {
                                    Font = OsuFont.GetFont(size: 12, weight: FontWeight.SemiBold),
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Width = 48,
                                },
                                new OsuClickableContainer
                                {
                                    AutoSizeAxes = Axes.Both,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Action = () => frequencyAdjust.SetDefault(),
                                    Child = new OsuSpriteText
                                    {
                                        Text = "1×",
                                        Font = OsuFont.GetFont(size: 12, weight: FontWeight.SemiBold),
                                        Colour = colours.Yellow,
                                    },
                                },
                            }
                        }
                    }
                }
            };

            // Register the adjustment once on the current track, then just update value.
            beatmap.BindValueChanged(e =>
            {
                e.OldValue?.Track.RemoveAdjustment(AdjustableProperty.Frequency, frequencyAdjust);
                e.NewValue?.Track.AddAdjustment(AdjustableProperty.Frequency, frequencyAdjust);
            }, true);

            frequencyAdjust.BindValueChanged(v =>
            {
                string sign = v.NewValue >= 1.0 ? "+" : "";
                label.Text = $"{sign}{(v.NewValue - 1.0) * 100:0}%";
            }, true);
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            beatmap.Value?.Track.RemoveAdjustment(AdjustableProperty.Frequency, frequencyAdjust);
        }
    }
}
