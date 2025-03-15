using System;

namespace KrofEngine
{
    public partial interface ICollider
    {
        public int Layer { get; set; }
        public float Mass { get; set; }
        public Action<Collider> OnCollision { get; set; }
        public void CollidedWith(Collider item)
        {
            OnCollision?.Invoke(item);
        }
        public Action<Collider> OnTrigger { get; set; }
        public void TriggeredWith(Collider item)
        {
            OnTrigger?.Invoke(item);
        }
    }
}
