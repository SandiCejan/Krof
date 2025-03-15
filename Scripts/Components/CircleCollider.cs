using System;

namespace KrofEngine
{
    internal class CircleCollider : Collider
    {
        internal float radius;
        public CircleCollider(float radius, int layer = 0, float mass = float.PositiveInfinity, Action<Collider> onCollision = null) : base(layer, mass, onCollision)
        {
            this.radius = radius;
        }
        internal override void OnDestroy()
        {
            base.OnDestroy();
            Renderer.drawableCircleColliders.Remove(this);
        }
    }
}
