using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KrofEngine
{
    public class Sprite
    {
        public Texture2D texture;
        public Rectangle[] rectangles;
        public Vector2 origin;
        public Sprite(Texture2D texture, Rectangle[] rectangles, Vector2 origin)
        {
            this.texture = texture;
            this.rectangles = rectangles;
            this.origin = origin;
        }
    }
}
