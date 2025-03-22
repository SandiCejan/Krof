using Microsoft.Xna.Framework;
using System;

namespace KrofEngine
{
    public class Collider : Component, ICollider
    {
        float mass;
        Action<Collider> onCollision;
        Action<Collider> onTrigger;
        public Collider(int layer, float mass = float.PositiveInfinity, Action<Collider> onCollision = null, Action<Collider> onTrigger = null) : base(Game1.Instance)
        {
            this.mass = mass;
            this.onCollision = onCollision;
            this.onTrigger = onTrigger;
            if (Physics.NumOfLayers > layer)
            {
                this.layer = layer;
            }
        }
        internal float Width;
        internal float Height;
        internal int x, y;
        //Readonly!
        public ref Vector2 Position => ref transform.position;

        int layer;
        public int Layer { get { return layer; } set { SetLayer(value); } }

        public float Mass { get { return mass; } set { mass = value; } }

        public Action<Collider> OnCollision { get { return onCollision; } set { onCollision = value; } }

        public Action<Collider> OnTrigger { get { return onTrigger; } set { onTrigger = value; } }

        public override void Awake()
        {
            gameObject.addCollider(this);
            x = (int)Position.X / Physics.ChunkSize;
            y = (int)Position.Y / Physics.ChunkSize;
            if (Width > Physics.ChunkSize || Height > Physics.ChunkSize) Physics.GlobalColliders.Add(this);
            else Physics.Colliders[x][y].Add(this);
        }

        public bool Trigger;
        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            if(Enabled) Physics.ForceCheckCollissionsStatic(this);
            base.OnEnabledChanged(sender, args);
        }
        private void SetLayer(int layer)
        {
            if (Physics.LayerCollision.GetLength(0) > layer)
            {
                this.layer = layer;
            }
        }
        internal override void OnDestroy()
        {
            if (Width > Physics.ChunkSize || Height > Physics.ChunkSize) Physics.GlobalColliders.Remove(this);
            else Physics.Colliders[x][y].Remove(this);
            gameObject.removeCollider(this);
        }
    }
}
