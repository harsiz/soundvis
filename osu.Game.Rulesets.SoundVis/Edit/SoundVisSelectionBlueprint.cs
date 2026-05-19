using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.SoundVis.Objects;
using osuTK;

namespace osu.Game.Rulesets.SoundVis.Edit
{
    /// <summary>
    /// Overlay drawn over a selected <see cref="SoundVisHitObject"/> in the editor compose view.
    /// Shows a highlighted bar in the approach direction.
    /// The selection quad is a fixed rectangle along the approach angle so the
    /// user can click the bar to select the note regardless of playback position.
    /// </summary>
    public partial class SoundVisSelectionBlueprint : HitObjectSelectionBlueprint
    {
        private readonly SoundVisHitObject soundVisObject;

        public SoundVisSelectionBlueprint(HitObject hitObject)
            : base(hitObject)
        {
            soundVisObject = (SoundVisHitObject)hitObject;

            RelativeSizeAxes = Axes.Both;

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

        // Build a screen-space quad centred in the compose area and oriented along
        // the approach angle — gives a stable, clickable area for every note.
        public override Quad SelectionQuad
        {
            get
            {
                // Fall back to the drawable if it has a meaningful size
                if (DrawableObject != null)
                {
                    var q = DrawableObject.ScreenSpaceDrawQuad;
                    if (q.Width > 4 || q.Height > 4)
                        return q;
                }

                // Construct a quad from the blueprint's centred bar Box in screen space
                // (the Box is 230 × 11 px rotated to the approach angle).
                return ToScreenSpace(
                    new RectangleF(
                        x:      DrawWidth  / 2f - 115f,
                        y:      DrawHeight / 2f - 5.5f,
                        width:  230f,
                        height: 11f));
            }
        }

        // Allow clicking anywhere in the half-plane that corresponds to this note's
        // quadrant, so the user can select a note by clicking near its bar.
        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos)
        {
            // Check the rotated bar quad first
            if (SelectionQuad.Contains(screenSpacePos))
                return true;

            // Also accept clicks in the approach-bar bounding box so the user
            // can click on the visible bar even when it's partially animated.
            if (DrawableObject != null && DrawableObject.ScreenSpaceDrawQuad.Contains(screenSpacePos))
                return true;

            return false;
        }
    }
}
