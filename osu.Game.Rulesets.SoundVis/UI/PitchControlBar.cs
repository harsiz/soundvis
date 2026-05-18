using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;

namespace osu.Game.Rulesets.SoundVis.UI
{
    /// <summary>
    /// Bottom bar that lets you crank the pitch up or down like a DJ.
    /// Adjusts track frequency — changes both pitch + tempo (vinyl speed style).
    /// </summary>
    public partial class PitchControlBar : CompositeDrawable
    {
        private readonly BindableDouble pitchRate = new BindableDouble(1.0)
        {
            MinValue = 0.5,
            MaxValue = 2.0,
            Precision = 0.01,
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
                    Padding = new MarginPadding { Horizontal = 20, Vertical = 10 },
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = OsuColour.Gray(0.1f),
                            Alpha = 0.85f,
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
                                    Current = pitchRate,
                                },
                                label = new OsuSpriteText
                                {
                                    Font = OsuFont.GetFont(size: 12, weight: FontWeight.SemiBold),
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Width = 45,
                                },
                                new ResetButton(pitchRate)
                                {
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                },
                            }
                        }
                    }
                }
            };

            pitchRate.BindValueChanged(v =>
            {
                string indicator = v.NewValue > 1.0 ? "+" : "";
                label.Text = $"{indicator}{(v.NewValue - 1.0) * 100:0}%";
                applyPitch(v.NewValue);
            }, true);
        }

        private void applyPitch(double rate)
        {
            var track = beatmap.Value?.Track;
            if (track == null) return;

            track.ResetSpeedAdjustments();
            track.AddAdjustment(AdjustableProperty.Frequency, new BindableDouble(rate));
        }

        // Reset button — double click the slider label to get back to 1x
        private partial class ResetButton : OsuAnimatedButton
        {
            private readonly BindableDouble target;

            public ResetButton(BindableDouble target)
            {
                this.target = target;
                Size = new osuTK.Vector2(40, 20);
            }

            [BackgroundDependencyLoader]
            private void load(OsuColour colours)
            {
                Add(new OsuSpriteText
                {
                    Text = "1x",
                    Font = OsuFont.GetFont(size: 11, weight: FontWeight.SemiBold),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Colour = colours.Yellow,
                });

                Action = () => target.SetDefault();
            }
        }
    }
}
