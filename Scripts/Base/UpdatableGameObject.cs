using Microsoft.Xna.Framework;
using System;

namespace KrofEngine
{
    public abstract class UpdatableGameObject : GameObject, IUpdate
    {

        protected UpdatableGameObject(Vector2 position = default, Vector2 scale = default, float angle = 0) : base(position, scale, angle)
        {
            EnabledChanged += delegate
            {
                if (Enabled)
                {
                    GameManager.AddUpdate(this);
                }
                else
                {
                    GameManager.updateablesToRemove.Add(this);
                }
            };
            GameManager.AddUpdate(this);
            OnActiveChanged += delegate { Enabled = Active; };
        }
        private bool enabled = true;
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (value != enabled)
                {
                    enabled = value;
                    EnabledChanged.Invoke(null, null);
                }
            }
        }

        public int UpdateOrder { get; set; }

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;

        public abstract void Update(GameTime gameTime);

        internal override void OnDestroy()
        {
            GameManager.updateablesToRemove.Add(this);
        }

        int IComparable<IUpdate>.CompareTo(IUpdate other)
        {
            if (other == null) return 1;
            return UpdateOrder.CompareTo(other.UpdateOrder);
        }
    }
}
