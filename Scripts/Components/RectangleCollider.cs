using Microsoft.Xna.Framework;
using System;

namespace KrofEngine
{
    internal class RectangleCollider : Collider, IUpdateable
    {
        public new Vector2 Position;
        private Vector2 movePos;
        private bool update;
        public RectangleCollider(int layer = 0, Vector2 size = default, Vector2 move = default, bool update = false, float mass = float.PositiveInfinity, Action<Collider> onCollision = null, Action<Collider> onTrigger = null) : base(layer, mass, onCollision, onTrigger)
        {
            if (size != default)
            {
                Width = size.X;
                Height = size.Y;
            }
            if (move != default)
            {
                movePos = move;
            }
            this.update = update;
        }
        private void UpdatePosition()
        {
            Position = transform.Position + movePos;
        }
        //public override void Update(GameTime gameTime)
        //{
        //    Position = transform.Position + movePos;
        //}
        public override void Awake()
        {
            if (update)
            {
                //GameManager.updateables.Add(this);
                UpdatePosition();
                transform.OnPositionChanged += UpdatePosition;
            }
            else
            {
                Position = transform.Position + movePos;
                //tempPos = Position;
            }
            if (Width == 0)
            {
                Rectangle r = GetComponent<DrawableItem>().Sprite.texture.Bounds;
                Width = r.Width * transform.Scale.X;
                Height = r.Height * transform.Scale.Y;
            }
            base.Awake();
        }
        internal override void OnDestroy()
        {
            base.OnDestroy();
            if(update) transform.OnPositionChanged -= UpdatePosition;
        }
    }
}
