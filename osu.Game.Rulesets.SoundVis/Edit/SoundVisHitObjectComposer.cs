using System.Collections.Generic;
using osu.Framework.Bindables;
using osu.Game.Graphics.UserInterface;
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
    public partial class SoundVisHitObjectComposer : HitObjectComposer<SoundVisHitObject, SoundVisAction>
    {
        public SoundVisHitObjectComposer(SoundVisRuleset ruleset)
            : base(ruleset)
        {
        }

        // ── Placement tools (left toolbar) ────────────────────────────────────────

        protected override IReadOnlyList<CompositionTool<SoundVisAction>> CompositionTools => new CompositionTool<SoundVisAction>[]
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

        // ── New-combo state ───────────────────────────────────────────────────────
        // osu! 2026.819 added this abstract member to HitObjectComposer. osu!vis has no
        // combo concept, so expose a fixed state that the editor's toolbox can bind to
        // harmlessly.
        public override Bindable<TernaryState> SelectionNewComboState { get; } =
            new Bindable<TernaryState>(TernaryState.False);

        // ── Blueprint container (hit-object selection overlays) ───────────────────

        protected override ComposeBlueprintContainer CreateBlueprintContainer()
            => new SoundVisBlueprintContainer(this);
    }
}
