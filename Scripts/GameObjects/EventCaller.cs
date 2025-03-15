using Krof.Scenes;
using KrofEngine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krof
{
    internal class EventCaller : GameObject
    {
        public EventCaller(Transform t, Vector2 size, Vector2 move) : base(t.position)
        {
            AddComponent(new RectangleCollider(2, size, move, onTrigger: OnTrigger));
            collider.Trigger = true;
            //Renderer.drawableRectangleColliders.Add(collider);
        }
        public void OnTrigger(Collider other)
        {
            if(StoryManager.Progress())
                Destroy();
        }
    }
}
