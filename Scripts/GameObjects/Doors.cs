using KrofEngine;
using Microsoft.Xna.Framework;
using System;
using System.Threading.Tasks;

namespace Krof
{
    public class Doors : GameObject
    {
        DrawableItem drawableItem;
        internal static Doors instance;
        public Doors(Game game, Vector2 position, float angle) : base(position, new Vector2(.5f, .5f), angle)
        {
            drawableItem = (DrawableItem)AddComponent(new DrawableItem(Renderer.Sprites[4]));
            instance = this;
            Vector2 move;
            switch (angle)
            {
                case 0:
                    move = new Vector2(-10, 90);
                    break;
                case (float)Math.PI:
                    move = new Vector2(-10, -90);
                    break;
                case (float)Math.PI/2:
                    move = new Vector2(-90, -10);
                    break;
                default:
                    move = new Vector2(90, -10);
                    break;
            }
            AddComponent(new RectangleCollider(2, new Vector2(20, 20), move));
            collider.Trigger = true;
        }
        public static void UnlockDoors()
        {
            Task.Run(instance.UnlockDoorsAnim);
            instance.collider.Enabled = false;
        }
        public async Task UnlockDoorsAnim()
        {
            PlaySceneManager.hall.Enabled = true;
            SoundEngine.PlaySound(5);
            for (int i = 1; i < 20; i++)
            {
                drawableItem.RectangleIndex = i;
                await Task.Delay(100);
            }
        }
    }
}
