using Microsoft.Xna.Framework;
namespace KrofEngine
{
    public class DrawableUI : Component
    {
        public int RectangleIndex;
        public Sprite Sprite;
        public Color Color = Color.White;

        public DrawableUI(Sprite sprite) : base(Game1.Instance)
        {
            Renderer.drawableUI.Add(this);
            Sprite = sprite;
        }
        internal override void OnDestroy()
        {
            Renderer.drawableUI.Remove(this);
        }
    }

}