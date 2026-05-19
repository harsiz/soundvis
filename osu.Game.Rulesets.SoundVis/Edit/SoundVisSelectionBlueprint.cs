using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.SoundVis.Objects;

namespace osu.Game.Rulesets.SoundVis.Edit
{
    /// <summary>
    /// Overlay drawn over a selected <see cref="SoundVisHitObject"/> in the editor compose view.
    /// Shows the bar in the approach direction with a coloured highlight.
    /// </summary>
    public partial class SoundVisSelectionBlueprint : HitObjectSelectionBlueprint
    {
        private readonly SoundVisHitObject soundVisObject;

        public SoundVisSelectionBlueprint(HitObject hitObject)
            : base(hitObject)
        {
            soundVisObject = (SoundVisHitObject)hitObject;

            AddInternal(new Box
            {
                Width    = 230,
                Height   = 11,
                Anchor   = Anchor.Centre,
                Origin   = Anchor.Centre,
                Rotation = soundVisObject.ApproachAngle,
                Colour   = SoundVisActionHelper.GetColour(soundVisObject.RequiredAction),
                Alpha    = 0.4f,
            });
        }

        // Use the live drawable's quad when present; fall back to the blueprint's own screen quad
        public override Quad SelectionQuad =>
            DrawableObject != null
                ? DrawableObject.ScreenSpaceDrawQuad
                : ScreenSpaceDrawQuad;
    }
}
