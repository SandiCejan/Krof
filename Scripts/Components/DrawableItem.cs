using Microsoft.Xna.Framework;
using System;
namespace KrofEngine
{
    public class DrawableItem : Component
    {
        public int RectangleIndex;
        public Sprite Sprite;
        public Color Color = Color.White;
        internal bool GlobalDraw;

        public DrawableItem(Sprite sprite) : base(Game1.Instance)
        {
            Renderer.drawableObjects.Add(this);
            Sprite = sprite;
        }
        public override void Awake()
        {
            GlobalDraw = Math.Max(transform.Scale.X, transform.Scale.Y) * Math.Max(Sprite.texture.Width, Sprite.texture.Height) > Renderer.GlobalObjectSize;
        }
        internal override void OnDestroy()
        {
            Renderer.drawableObjects.Remove(this);
        }
    }

}