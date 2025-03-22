using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KrofEngine
{
    public class DrawableText : Component
    {
        public string Text { get { return text; } set { text = value; setPosition(); } }
        private string text;
        public TextAlignment TextAlignment { get { return textAlignment; } set { textAlignment = value; setPosition(); } }
        private TextAlignment textAlignment;
        internal Vector2 position { get; private set; }
        public Vector2 BeginPosition;
        public SpriteFont Font { get { return font; } set { font = value; setPosition(); } }
        private SpriteFont font;
        public Color Color;
        public DrawableText(string text, Color color, TextAlignment textAlignment = TextAlignment.Left, SpriteFont font = null) : base(Game1.Instance)
        {
            this.text = text;
            this.textAlignment = textAlignment;
            Color = color;
            this.font = font != null ? font : Renderer.Fonts[0];
            Renderer.drawableText.Add(this);
        }
        public override void Awake()
        {
            position = transform.position;
            setPosition();
            transform.OnPositionChanged += delegate { setPosition(); };
        }
        public void MovePosition(Vector2 moveAmount)
        {
            BeginPosition = moveAmount;
        }

        private void setPosition()
        {
            position = transform.position + BeginPosition;
            if (textAlignment == TextAlignment.Left) return;
            if (textAlignment == TextAlignment.Middle)
            {
                position -= Font.MeasureString(text) / 2;
            }
            else
            {
                position -= Font.MeasureString(text);
            }
        }
        internal override void OnDestroy()
        {
            Renderer.drawableText.Remove(this);
        }
    }
}
