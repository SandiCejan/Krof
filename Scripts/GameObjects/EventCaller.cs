using Krof.Scenes;
using KrofEngine;
using Microsoft.Xna.Framework;

namespace Krof
{
    internal class EventCaller : GameObject
    {
        public EventCaller(Transform t, Vector2 size, Vector2 move) : base(t.position)
        {
            AddComponent(new RectangleCollider(2, size, move, onTrigger: OnTrigger));
            collider.Trigger = true;
        }
        public void OnTrigger(Collider other)
        {
            if(StoryManager.Progress())
                Destroy();
        }
    }
}
