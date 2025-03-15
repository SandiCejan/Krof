using Microsoft.Xna.Framework;
using System;

namespace KrofEngine
{
    public class Transform
    {
        public Action OnPositionChanged;
        public Vector2 Position { get { return position; } set { position = value; OnPositionChanged?.Invoke(); } }
        internal Vector2 position;
        public float Angle;
        public Vector2 Scale = Vector2.One;
        public Transform(Vector2 Position = default, Vector2 Scale = default, float Angle = 0) {
            this.Position = Position;
            if (Scale != default)
                this.Scale = Scale;
            this.Angle = Angle;
        }
    }
}
