using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.SoundVis
{
    public class SoundVisSkinTransformer : SkinTransformer
    {
        public SoundVisSkinTransformer(ISkin skin)
            : base(skin)
        {
        }

        public override Drawable? GetDrawableComponent(ISkinComponentLookup lookup)
        {
            // Swallow every HUD container lookup — SoundVis renders its own UI,
            // so we don't want osu!'s score/health/progress bar showing on top.
            if (lookup is GlobalSkinnableContainerLookup)
                return new Container();

            return base.GetDrawableComponent(lookup);
        }
    }
}
