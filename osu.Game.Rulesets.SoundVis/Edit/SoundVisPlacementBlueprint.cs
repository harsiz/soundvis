using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.SoundVis.Objects;
using osuTK;
using osuTK.Input;

namespace osu.Game.Rulesets.SoundVis.Edit
{
    /// <summary>
    /// Ghost bar shown while the user is about to place a note.
    /// The action (quadrant) is fixed by the tool; only StartTime changes.
    /// One click = one note placed.
    /// </summary>
    public partial class SoundVisPlacementBlueprint : HitObjectPlacementBlueprint
    {
        public new SoundVisHitObject HitObject => (SoundVisHitObject)base.HitObject;

        public SoundVisPlacementBlueprint(SoundVisAction action)
            : base(new SoundVisHitObject
            {
                RequiredAction = action,
                ApproachAngle  = AngleForAction(action),
            })
        {
            // Ghost bar: same size as a real approach bar, semi-transparent
            InternalChild = new Box
            {
                Width    = 220,
                Height   = 7,
                Anchor   = Anchor.Centre,
                Origin   = Anchor.Centre,
                Colour   = SoundVisActionHelper.GetColour(action),
                Rotation = AngleForAction(action),
                Alpha    = 0.55f,
            };
        }

        public override SnapResult UpdateTimeAndPosition(Vector2 screenSpacePosition, double time)
        {
            // Only StartTime matters — the quadrant is fixed by the tool
            HitObject.StartTime = time;
            return new SnapResult(screenSpacePosition, time);
        }

        // Single-click placement: commit the note immediately on left mouse down.
        // Override OnMouseDown directly (same pattern as osu!std circle placement)
        // rather than BeginPlacement, so the framework routes the event reliably.
        protected override bool OnMouseDown(MouseDownEvent e)
        {
            if (e.Button != MouseButton.Left)
                return false;

            BeginPlacement(true);  // sets PlacementActive = Active
            EndPlacement(true);    // commits the note, sets PlacementActive = Finished
            return true;
        }

        /// <summary>Centre angle (degrees, 0=up, clockwise) for each quadrant.</summary>
        private static float AngleForAction(SoundVisAction action) => action switch
        {
            SoundVisAction.TopLeft     => 315f,
            SoundVisAction.TopRight    => 45f,
            SoundVisAction.BottomRight => 135f,
            SoundVisAction.BottomLeft  => 225f,
            _                          => 0f,
        };
    }
}
