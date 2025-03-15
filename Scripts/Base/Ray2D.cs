using Microsoft.Xna.Framework;

namespace KrofEngine
{
    public class Ray2D
    {
        public Vector2 origin;
        public Vector2 direction;
        public float maxDistance;
        public int layer;

        public Ray2D(Vector2 origin, Vector2 direction, float maxDistance, int layer)
        {
            this.origin = origin;
            this.direction = direction;
            this.maxDistance = maxDistance;
            this.layer = layer;
        }
    }
}
