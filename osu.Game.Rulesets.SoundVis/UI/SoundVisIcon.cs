using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.SoundVis.UI
{
    /// <summary>
    /// The ruleset icon shown in osu! UI (song select, score results, mod overlay, etc.).
    /// Renders the osuvis-logo.png texture clipped to a circle.
    /// </summary>
    public partial class SoundVisIcon : CompositeDrawable
    {
        [BackgroundDependencyLoader]
        private void load(GameHost host)
        {
            AutoSizeAxes = Axes.Both;

            var resources = new DllResourceStore(typeof(SoundVisRuleset).Assembly);
            var byteStore = new NamespacedResourceStore<byte[]>(resources, "Resources");
            using var textureStore = new TextureStore(
                host.Renderer,
                new TextureLoaderStore(byteStore));

            var texture = textureStore.Get("Textures/osuvis-logo");

            if (texture != null)
            {
                InternalChild = new CircularContainer
                {
                    Size = new Vector2(20),
                    Masking = true,
                    Child = new Sprite
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Texture = texture,
                        FillMode = FillMode.Fit,
                        RelativeSizeAxes = Axes.Both,
                    },
                };
            }
            else
            {
                // Fallback: plain coloured circle so it never shows blank
                InternalChild = new CircularContainer
                {
                    Size = new Vector2(20),
                    Masking = true,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = new Color4(255, 102, 170, 255),
                    },
                };
            }
        }
    }
}
