using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.Containers;

namespace osu.Game.Rulesets.SoundVis.UI
{
    /// <summary>
    /// Top-left "now playing" card showing album art + song info.
    /// </summary>
    public partial class NowPlayingPanel : CompositeDrawable
    {
        private TruncatingSpriteText titleText = null!;
        private TruncatingSpriteText artistText = null!;
        private Sprite albumArt = null!;

        [Resolved]
        private IBindable<WorkingBeatmap> beatmap { get; set; } = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            AutoSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                new Container
                {
                    AutoSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = 12,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = OsuColour.Gray(0.08f),
                            Alpha = 0.9f,
                        },
                        new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Direction = FillDirection.Horizontal,
                            Padding = new MarginPadding(14),
                            Spacing = new osuTK.Vector2(14, 0),
                            Children = new Drawable[]
                            {
                                albumArt = new Sprite
                                {
                                    Size = new osuTK.Vector2(64),
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    FillMode = FillMode.Fill,
                                },
                                new FillFlowContainer
                                {
                                    AutoSizeAxes = Axes.Both,
                                    Direction = FillDirection.Vertical,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Spacing = new osuTK.Vector2(0, 4),
                                    Children = new Drawable[]
                                    {
                                        new OsuSpriteText
                                        {
                                            Text = "NOW VIBING TO",
                                            Font = OsuFont.GetFont(size: 9, weight: FontWeight.Bold),
                                            Colour = OsuColour.Gray(0.6f),
                                        },
                                        titleText = new TruncatingSpriteText
                                        {
                                            Font = OsuFont.GetFont(size: 18, weight: FontWeight.Bold),
                                            MaxWidth = 320,
                                        },
                                        artistText = new TruncatingSpriteText
                                        {
                                            Font = OsuFont.GetFont(size: 13),
                                            Colour = OsuColour.Gray(0.75f),
                                            MaxWidth = 320,
                                        },
                                    }
                                }
                            }
                        }
                    }
                }
            };

            beatmap.BindValueChanged(updateBeatmap, true);
        }

        private void updateBeatmap(ValueChangedEvent<WorkingBeatmap> e)
        {
            var b = e.NewValue;
            if (b == null) return;

            titleText.Text = b.Metadata.TitleUnicode.Length > 0 ? b.Metadata.TitleUnicode : b.Metadata.Title;
            artistText.Text = b.Metadata.ArtistUnicode.Length > 0 ? b.Metadata.ArtistUnicode : b.Metadata.Artist;

            var tex = b.GetBackground();
            if (tex != null)
                albumArt.Texture = tex;
        }
    }
}
