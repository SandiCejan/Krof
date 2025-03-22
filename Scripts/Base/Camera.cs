using Microsoft.Xna.Framework;

namespace KrofEngine
{
    internal class Camera
    {
        public static Matrix Transform { get; internal set; }
        public static Matrix UITransform { get; internal set; }
        public static Vector2 Position { get; internal set; }
        public static void Resize()
        {
            UITransform = Matrix.CreateScale(new Vector3((float)Game1.ScreenWidth / Game1.GameWidth, (float)Game1.ScreenHeight / Game1.GameHeight, 1));
        }
        public static void Follow(Transform target)
        {
            var position = Matrix.CreateTranslation(
            -target.Position.X - (80 / 2),
            -target.Position.Y - (80 / 2),
            0);
            var offset = Matrix.CreateTranslation(
            Game1.GameWidth / 2,
            Game1.GameHeight / 2,
            0);
            Transform = position * offset * UITransform;
            Position = new Vector2(position.Translation.X, position.Translation.Y);
        }
    }
}