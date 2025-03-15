using Microsoft.Xna.Framework;

namespace KrofEngine
{
    internal class Image : GameObject
    {
        public DrawableUI image;
        public Image(Sprite sprite, Vector2 position, Vector2 scale, Color color = default) : base(position, scale)
        {
            image = ((DrawableUI)AddComponent(new DrawableUI(sprite)));
            image.Color = color == default?Color.White:color;
        }
    }
}
