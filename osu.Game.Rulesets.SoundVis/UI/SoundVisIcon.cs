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
    public partial class SoundVisIcon : CompositeDrawable
    {
        // Kept as a field so the store (and its GPU textures) are not disposed
        // while the Sprite still references them.
        private TextureStore? textureStore;

        [BackgroundDependencyLoader(permitNulls: true)]
        private void load(GameHost? host)
        {
            AutoSizeAxes = Axes.Both;

            Texture? texture = null;

            if (host != null)
            {
                var resources = new DllResourceStore(typeof(SoundVisRuleset).Assembly);
                var byteStore = new NamespacedResourceStore<byte[]>(resources, "Resources");
                textureStore = new TextureStore(host.Renderer, new TextureLoaderStore(byteStore));
                texture = textureStore.Get("Textures/osuvis-logo");
            }

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
                // Fallback: solid pink circle so there is always something visible.
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

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            textureStore?.Dispose();
        }
    }
}
