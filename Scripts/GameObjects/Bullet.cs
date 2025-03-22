using KrofEngine;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace Krof
{
    public class Bullet : GameObject
    {
        public Bullet(Game game, Vector2 position, Vector2 velocity) : base(position, new Vector2(.2f, .2f))
        {
            AddComponent(new DrawableItem(Renderer.Sprites[6]));
            Static = false;
            MoveAmount = velocity;
            Task.Run(wait);
            AddComponent(new RectangleCollider(5, new Vector2(10, 10), new Vector2(-5, -5), true, onCollision: OnCollision));
        }
        async Task wait()
        {
            await Task.Delay(2000);
            Destroy();
        }
        public void OnCollision(Collider collisionObject)
        {
            if (collisionObject.gameObject.GetType() == typeof(Player))
            {
                Player.player.Damage(1);
            }
            Destroy();
        }
    }
}
