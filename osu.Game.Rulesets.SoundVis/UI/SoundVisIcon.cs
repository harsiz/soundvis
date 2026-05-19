using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.SoundVis.UI
{
    public partial class SoundVisIcon : CompositeDrawable
    {
        // Kept as a field so textures aren't freed while the Sprite still references them.
        private TextureStore? textureStore;

        public SoundVisIcon()
        {
            // Explicit size so the icon is always 20x20 even if children fail to load.
            Size = new Vector2(20);
        }

        [BackgroundDependencyLoader]
        private void load(IRenderer renderer)
        {
            var resources = new DllResourceStore(typeof(SoundVisRuleset).Assembly);
            var byteStore  = new NamespacedResourceStore<byte[]>(resources, "Resources");
            textureStore   = new TextureStore(renderer, new TextureLoaderStore(byteStore));

            var texture = textureStore.Get("Textures/osuvis-logo");

            InternalChild = new CircularContainer
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                Child = texture != null
                    ? (Drawable)new Sprite
                    {
                        RelativeSizeAxes = Axes.Both,
                        FillMode         = FillMode.Fit,
                        Anchor           = Anchor.Centre,
                        Origin           = Anchor.Centre,
                        Texture          = texture,
                    }
                    : new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour           = new Color4(255, 102, 170, 255),
                    },
            };
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            textureStore?.Dispose();
        }
    }
}
