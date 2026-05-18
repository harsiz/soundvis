using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;

namespace osu.Game.Rulesets.SoundVis.UI
{
    public partial class SoundVisIcon : CompositeDrawable
    {
        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            AutoSizeAxes = Axes.Both;

            InternalChild = new SpriteIcon
            {
                Icon = FontAwesome.Solid.Music,
                Size = new osuTK.Vector2(20),
                Colour = colours.Pink,
            };
        }
    }
}
