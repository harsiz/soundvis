using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.SoundVis.Edit
{
    /// <summary>
    /// One of the four quadrant placement tools shown in the editor's left toolbar.
    /// </summary>
    public class SoundVisPlaceTool : CompositionTool
    {
        private readonly SoundVisAction action;

        public SoundVisPlaceTool(SoundVisAction action)
            : base(action.ToString())
        {
            this.action  = action;
            TooltipText  = $"Place {SoundVisActionHelper.GetKeyLabel(action)} note";
        }

        public override Drawable CreateIcon() => new SoundVisToolIcon(action);

        public override PlacementBlueprint CreatePlacementBlueprint()
            => new SoundVisPlacementBlueprint(action);

        // ── Coloured icon shown in the toolbar ───────────────────────────────────

        private partial class SoundVisToolIcon : CircularContainer
        {
            public SoundVisToolIcon(SoundVisAction action)
            {
                Size    = new Vector2(26);
                Masking = true;

                Color4 colour = SoundVisActionHelper.GetColour(action);
                string key    = SoundVisActionHelper.GetKeyLabel(action);

                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour           = colour,
                        Alpha            = 0.85f,
                    },
                    new SpriteText
                    {
                        Text   = key,
                        Font   = OsuFont.GetFont(size: 14, weight: FontWeight.Bold),
                        Colour = Color4.White,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                    },
                };
            }
        }
    }
}
