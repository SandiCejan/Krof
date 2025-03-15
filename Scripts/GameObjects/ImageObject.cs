using Microsoft.Xna.Framework;

namespace KrofEngine
{
    internal class ImageObject : GameObject
    {
        public ImageObject(Sprite sprite, Vector2 position, Vector2 scale, Color color = default) : base(position, scale)
        {
            ((DrawableItem)AddComponent(new DrawableItem(sprite))).Color = color == default?Color.White:color;
        }
    }
}
