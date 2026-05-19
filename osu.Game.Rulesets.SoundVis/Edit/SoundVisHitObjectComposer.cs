using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.SoundVis.Objects;
using osu.Game.Rulesets.SoundVis.UI;
using osu.Game.Rulesets.UI;
using osu.Game.Screens.Edit.Compose.Components;

namespace osu.Game.Rulesets.SoundVis.Edit
{
    /// <summary>
    /// Editor compose-mode host for osu!vis.
    /// Provides four placement tools — one per quadrant — and a custom blueprint container.
    /// </summary>
    public partial class SoundVisHitObjectComposer : HitObjectComposer<SoundVisHitObject>
    {
        public SoundVisHitObjectComposer(SoundVisRuleset ruleset)
            : base(ruleset)
        {
        }

        // ── Placement tools (left toolbar) ────────────────────────────────────────

        protected override IReadOnlyList<CompositionTool> CompositionTools => new CompositionTool[]
        {
            new SoundVisPlaceTool(SoundVisAction.TopLeft),
            new SoundVisPlaceTool(SoundVisAction.TopRight),
            new SoundVisPlaceTool(SoundVisAction.BottomLeft),
            new SoundVisPlaceTool(SoundVisAction.BottomRight),
        };

        // ── Drawable ruleset (game view inside the editor) ────────────────────────

        protected override DrawableRuleset<SoundVisHitObject> CreateDrawableRuleset(
            Ruleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod>? mods = null)
            => new DrawableSoundVisRuleset((SoundVisRuleset)ruleset, beatmap, mods);

        // ── Blueprint container (hit-object selection overlays) ───────────────────

        protected override ComposeBlueprintContainer CreateBlueprintContainer()
            => new SoundVisBlueprintContainer(this);
    }
}
