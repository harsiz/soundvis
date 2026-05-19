using System.Collections.Generic;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Screens.Edit.Compose.Components;
using osuTK;

namespace osu.Game.Rulesets.SoundVis.Edit
{
    /// <summary>
    /// Blueprint container for the osu!vis editor compose screen.
    /// Creates a <see cref="SoundVisSelectionBlueprint"/> for every hit object.
    /// Notes have no XY position, so blueprint dragging is not supported (use the timeline).
    /// </summary>
    public partial class SoundVisBlueprintContainer : ComposeBlueprintContainer
    {
        public SoundVisBlueprintContainer(HitObjectComposer composer)
            : base(composer)
        {
        }

        // Override the virtual (non-sealed) hook used by ComposeBlueprintContainer
        public override HitObjectSelectionBlueprint CreateHitObjectBlueprintFor(HitObject hitObject)
            => new SoundVisSelectionBlueprint(hitObject);

        // SoundVis notes don't have 2D positions — spatial drag-to-move is unsupported.
        // Note timing can still be adjusted via the editor timeline.
        protected override bool TryMoveBlueprints(
            DragEvent e,
            IList<(SelectionBlueprint<HitObject> blueprint, Vector2[] originalSnapPositions)> blueprints)
        {
            return false;
        }
    }
}
