using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KrofEngine
{
    internal class Text : GameObject
    {
        public string text { get { return drawableText.Text; } set { drawableText.Text = value; } }
        public Color color { get { return drawableText.Color; } set { drawableText.Color = value; } }
        DrawableText drawableText;
        public Text(Vector2 position, string text, Color color, TextAlignment textAlignment = TextAlignment.Left, SpriteFont font = null) : base(position, new Vector2(.3f, .3f))
        {
            drawableText = new DrawableText(text, color, textAlignment, font);
            AddComponent(drawableText);
        }
    }
}
