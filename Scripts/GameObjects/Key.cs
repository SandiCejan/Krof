using KrofEngine;
using Microsoft.Xna.Framework;

namespace Krof
{
    internal class Key : GameObject
    {
        DrawableItem drawableItem;
        public static Key Instance;
        public Key(Game game, Vector2 position) : base(position, new Vector2(.5f, .5f))
        {
            drawableItem = (DrawableItem)AddComponent(new DrawableItem(Renderer.Sprites[5]));
            AddComponent(new RectangleCollider(2, new Vector2(20, 20), move:new Vector2(-35, -35)));
            collider.Trigger = true;
            Instance = this;
            Renderer.drawableRectangleColliders.Add(collider);
        }
    }
}
