using Krof.Scenes;
using KrofEngine;
using Microsoft.Xna.Framework;

namespace Krof
{
    internal class Hider : UpdatableGameObject
    {
        Image i;
        float colorA;
        internal Hider()
        {
            i = new Image(Renderer.Sprites[1], Vector2.Zero, new Vector2(Game1.GameWidth, Game1.GameHeight), Color.Black);
            i.image.Color.A = 0;
        }
        public override void Update(GameTime gameTime)
        {
            colorA += (60 * (float)gameTime.ElapsedGameTime.TotalSeconds);
            if (colorA > 255)
            {
                i.image.Color.A = 255;
                Destroy();
                StoryManager.Progress();
                StoryManager.PlayerText.color = Color.White;
            }
            else
            {
                i.image.Color.A = (byte)colorA;
            }
        }
    }
}
