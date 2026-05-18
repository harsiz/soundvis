using System;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.SoundVis.Objects
{
    public partial class DrawableSoundVisHitObject : DrawableHitObject<SoundVisHitObject>
    {
        private const float HIT_WINDOW = 200f;
        public const float LOGO_RADIUS = 80f;

        /// <summary>Fired when cursor enters/leaves this hit zone.</summary>
        public event Action<bool>? HoverStateChanged;

        private bool mouseIsOver;

        public DrawableSoundVisHitObject(SoundVisHitObject hitObject)
            : base(hitObject)
        {
            Alpha = 0;
            Size = new Vector2(LOGO_RADIUS * 2);
            Origin = Anchor.Centre;
        }

        protected override bool OnHover(HoverEvent e)
        {
            mouseIsOver = true;
            HoverStateChanged?.Invoke(true);
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            mouseIsOver = false;
            HoverStateChanged?.Invoke(false);
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (timeOffset < -HIT_WINDOW)
                return;

            if (timeOffset > HIT_WINDOW)
            {
                ApplyResult(HitResult.Miss);
                return;
            }

            if (mouseIsOver)
                ApplyResult(HitResult.Great);
        }

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos)
        {
            Vector2 local = ToLocalSpace(screenSpacePos);
            Vector2 centre = DrawSize / 2;
            return (local - centre).Length <= LOGO_RADIUS;
        }

        protected override void UpdateHitStateTransforms(ArmedState state) { }
    }
}
