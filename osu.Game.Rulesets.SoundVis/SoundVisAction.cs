using osuTK.Graphics;

namespace osu.Game.Rulesets.SoundVis
{
    public enum SoundVisAction
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
    }

    public static class SoundVisActionHelper
    {
        // D / J / F / K default binds
        public static string GetKeyLabel(SoundVisAction action) => action switch
        {
            SoundVisAction.TopLeft     => "D",
            SoundVisAction.TopRight    => "J",
            SoundVisAction.BottomLeft  => "F",
            SoundVisAction.BottomRight => "K",
            _                          => "?",
        };

        public static Color4 GetColour(SoundVisAction action) => action switch
        {
            SoundVisAction.TopLeft     => new Color4(255, 80,  80,  255),  // red
            SoundVisAction.TopRight    => new Color4(80,  160, 255, 255),  // blue
            SoundVisAction.BottomLeft  => new Color4(80,  220, 80,  255),  // green
            SoundVisAction.BottomRight => new Color4(255, 200, 60,  255),  // yellow
            _                          => Color4.White,
        };

        /// <summary>Maps a 0°-360° approach angle (0=top, clockwise) to the required action.</summary>
        public static SoundVisAction FromAngle(float angleDeg)
        {
            float a = ((angleDeg % 360) + 360) % 360;
            if (a < 90)  return SoundVisAction.TopRight;
            if (a < 180) return SoundVisAction.BottomRight;
            if (a < 270) return SoundVisAction.BottomLeft;
            return SoundVisAction.TopLeft;
        }
    }
}
