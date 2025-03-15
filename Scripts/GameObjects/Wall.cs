using KrofEngine;
using Microsoft.Xna.Framework;

namespace Krof
{
    public class Wall : GameObject
    {
        public DrawableItem drawableItem;
        byte changePerFrame;
        public bool Active;
        public Wall(Game game, Vector2 position, bool active) : base(position, new Vector2(100, 100))
        {
            drawableItem = (DrawableItem)AddComponent(new DrawableItem(Renderer.Sprites[1]));
            drawableItem.Color = Color.Black;
            if (!active)
            {
                drawableItem.Color.A = 203;
            }
            Active = active;
            AddComponent(new RectangleCollider(0));
            collider.Enabled = active;
        }
        public Wall(Game game, Vector2 position, Vector2 scale) : base(position, scale)
        {
            drawableItem = (DrawableItem)AddComponent(new DrawableItem(Renderer.Sprites[1]));
            drawableItem.Color = Color.Black;
            Active = true;
            AddComponent(new RectangleCollider());
        }
        public void SetFull(bool Active)
        {
            if (Active)
            {
                drawableItem.Color.A = 255;
            }
            else
            {
                drawableItem.Color.A = 203;
            }
            this.Active = Active;
            collider.Enabled = Active;
        }
    }
}
