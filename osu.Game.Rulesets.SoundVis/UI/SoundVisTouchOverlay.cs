using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osu.Framework.Input.StateChanges;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.SoundVis.Configuration;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;

namespace osu.Game.Rulesets.SoundVis.UI
{
    /// <summary>
    /// Touchscreen input for osu!vis: the screen is split into four quadrant tap
    /// zones matching the four <see cref="SoundVisAction"/> directions.
    /// <para>
    /// Presses are fed through the ruleset's <c>KeyBindingContainer</c> rather than
    /// invoking playfield hit logic directly. That keeps touch on exactly the same
    /// path as keyboard input, so the <see cref="Replays.SoundVisReplayRecorder"/>
    /// (an <c>IKeyBindingHandler</c>) records touch plays correctly.
    /// </para>
    /// <para>
    /// The overlay stays invisible until the first touch arrives, so keyboard players
    /// never see it. It is <c>AlwaysPresent</c> so it can still receive that first touch
    /// while fully transparent.
    /// </para>
    /// </summary>
    public partial class SoundVisTouchOverlay : CompositeDrawable
    {
        private readonly SoundVisInputManager inputManager;

        /// <summary>Which action each active finger is currently holding.</summary>
        private readonly Dictionary<TouchSource, SoundVisAction> activeTouches = new();

        /// <summary>
        /// How many fingers are holding each action. The input manager runs in
        /// <c>SimultaneousBindingMode.Unique</c>, so an action must be pressed once on
        /// 0→1 and released once on 1→0 — otherwise a second finger landing in the same
        /// quadrant would release the action early when it lifts.
        /// </summary>
        private readonly Dictionary<SoundVisAction, int> pressCounts = new();

        private readonly Dictionary<SoundVisAction, TouchQuadrant> zones = new();

        /// <summary>Which action the mouse is currently holding, if any.</summary>
        private SoundVisAction? mouseAction;

        private readonly BindableBool touchControls = new BindableBool(true);

        private bool revealed;

        [Resolved(CanBeNull = true)]
        private SoundVisRulesetConfigManager? rulesetConfig { get; set; }

        public SoundVisTouchOverlay(SoundVisInputManager inputManager)
        {
            this.inputManager = inputManager;

            RelativeSizeAxes = Axes.Both;

            // Transparent until the first touch, but must still accept input.
            Alpha         = 0;
            AlwaysPresent = true;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            var container = new Container { RelativeSizeAxes = Axes.Both };

            foreach (SoundVisAction action in Enum.GetValues<SoundVisAction>())
            {
                var zone = new TouchQuadrant(action);
                zones[action] = zone;
                container.Add(zone);
            }

            InternalChild = container;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            rulesetConfig?.BindWith(SoundVisRulesetSetting.TouchControls, touchControls);

            touchControls.BindValueChanged(v =>
            {
                if (!v.NewValue)
                {
                    releaseAll();
                    this.FadeOut(150, Easing.OutQuint);
                    revealed = false;
                }
            }, true);
        }

        // ── Touch handling ────────────────────────────────────────────────────────

        protected override bool OnTouchDown(TouchDownEvent e)
        {
            if (!touchControls.Value)
                return false;

            var action = actionForPosition(ToLocalSpace(e.ScreenSpaceTouch.Position));

            activeTouches[e.Touch.Source] = action;
            pressAction(action);

            zones[action].Flash();
            reveal();

            return true;
        }

        protected override void OnTouchUp(TouchUpEvent e)
        {
            if (!activeTouches.Remove(e.Touch.Source, out var action))
                return;

            releaseAction(action);
            zones[action].Unflash();
        }

        /// <summary>
        /// A finger sliding between quadrants deliberately does not re-trigger — the
        /// action stays bound to the quadrant the finger first landed in, which avoids
        /// spurious hits from thumb roll-off during fast play.
        /// </summary>
        protected override void OnTouchMove(TouchMoveEvent e)
        {
        }

        // ── Mouse handling ────────────────────────────────────────────────────────
        // Left-click drives the same quadrant zones, so the overlay is usable (and
        // testable) on a desktop without a touchscreen.

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            if (!touchControls.Value || e.Button != MouseButton.Left)
                return false;

            // osu!framework synthesises mouse events from touches. Without this guard a
            // single tap on a real touchscreen would arrive twice — once via OnTouchDown
            // and again as a synthesised click.
            if (e.CurrentState.Mouse.LastSource is ISourcedFromTouch)
                return false;

            var action = actionForPosition(ToLocalSpace(e.ScreenSpaceMousePosition));

            mouseAction = action;
            pressAction(action);

            zones[action].Flash();
            reveal();

            return true;
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            if (e.Button != MouseButton.Left || mouseAction is not SoundVisAction action)
                return;

            mouseAction = null;
            releaseAction(action);
            zones[action].Unflash();
        }

        private void pressAction(SoundVisAction action)
        {
            pressCounts.TryGetValue(action, out int count);
            pressCounts[action] = count + 1;

            if (count == 0)
                inputManager.Bindings.TriggerPressed(action);
        }

        private void releaseAction(SoundVisAction action)
        {
            if (!pressCounts.TryGetValue(action, out int count) || count == 0)
                return;

            pressCounts[action] = count - 1;

            if (count - 1 == 0)
                inputManager.Bindings.TriggerReleased(action);
        }

        /// <summary>Drops every held action — used when touch input is switched off mid-play.</summary>
        private void releaseAll()
        {
            foreach (var action in activeTouches.Values)
                releaseAction(action);

            activeTouches.Clear();

            if (mouseAction is SoundVisAction held)
            {
                mouseAction = null;
                releaseAction(held);
            }

            foreach (var zone in zones.Values)
                zone.Unflash();
        }

        private void reveal()
        {
            if (revealed)
                return;

            revealed = true;
            this.FadeIn(250, Easing.OutQuint);
        }

        private SoundVisAction actionForPosition(Vector2 local)
        {
            bool right  = local.X >= DrawWidth  / 2f;
            bool bottom = local.Y >= DrawHeight / 2f;

            return (right, bottom) switch
            {
                (false, false) => SoundVisAction.TopLeft,
                (true,  false) => SoundVisAction.TopRight,
                (false, true)  => SoundVisAction.BottomLeft,
                (true,  true)  => SoundVisAction.BottomRight,
            };
        }

        // ── Inner drawable: one quadrant tap zone ─────────────────────────────────

        private partial class TouchQuadrant : CompositeDrawable
        {
            private readonly SoundVisAction action;

            private Box       fill   = null!;
            private Container border = null!;

            private const float IDLE_ALPHA = 0.10f;

            public TouchQuadrant(SoundVisAction action)
            {
                this.action = action;

                RelativeSizeAxes = Axes.Both;
                Size             = new Vector2(0.5f);

                var anchor = CornerAnchor(action);
                Anchor = anchor;
                Origin = anchor;
            }

            [BackgroundDependencyLoader]
            private void load()
            {
                var colour = SoundVisActionHelper.GetColour(action);

                InternalChildren = new Drawable[]
                {
                    fill = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour           = colour,
                        Alpha            = IDLE_ALPHA,
                    },
                    border = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Masking          = true,
                        BorderThickness  = 3f,
                        BorderColour     = colour,
                        Child = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Alpha            = 0,
                            AlwaysPresent    = true,
                        },
                    },
                    new OsuSpriteText
                    {
                        Text   = SoundVisActionHelper.GetKeyLabel(action),
                        Font   = OsuFont.GetFont(size: 28, weight: FontWeight.Bold),
                        Colour = colour,
                        Alpha  = 0.5f,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                    },
                };
            }

            public void Flash()
            {
                fill.FadeTo(0.42f, 30, Easing.OutQuint);
                border.FadeTo(1f, 30, Easing.OutQuint);
            }

            public void Unflash()
            {
                fill.FadeTo(IDLE_ALPHA, 220, Easing.OutQuint);
                border.FadeTo(1f, 220, Easing.OutQuint);
            }

            private static Anchor CornerAnchor(SoundVisAction action) => action switch
            {
                SoundVisAction.TopLeft     => Anchor.TopLeft,
                SoundVisAction.TopRight    => Anchor.TopRight,
                SoundVisAction.BottomLeft  => Anchor.BottomLeft,
                SoundVisAction.BottomRight => Anchor.BottomRight,
                _                          => Anchor.Centre,
            };
        }
    }
}
