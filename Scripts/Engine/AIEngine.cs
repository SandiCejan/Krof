using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace KrofEngine
{
    public class AIEngine
    {
        public static List<AIActor> Actors = new List<AIActor>();
        public static bool Active = true;
        public void Update(GameTime gameTime)
        {
            if (Active)
            {
                foreach (var item in Actors)
                {
                    if (item.Enabled)
                    {
                        item.Update(gameTime);
                    }
                }
            }
        }
    }
}
