using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace KrofEngine
{
    public class Button : UpdatableGameObject
    {
        public static Action<Button> OnClickAction;
        private Rectangle rectangle;

        public Color backgroundColor;
        public Color hoverColor;

        private MouseState currentMouseState;
        private MouseState previousMouseState;

        public bool IsHovered { get; private set; }
        public bool IsClicked { get; private set; }
        public event EventHandler MouseDown;
        public event EventHandler MouseUp;

        DrawableUI drawableUI;
        public Button() { Enabled = false; }
        public Button(Sprite sprite, Vector2 position, Color backgroundColor, Color hoverColor, string text = null, Color textColor = default, TextAlignment textAlignment = TextAlignment.Left, SpriteFont font = null, EventHandler mouseUp = null, EventHandler mouseDown = null) : base(position)
        {
            this.backgroundColor = backgroundColor;
            this.hoverColor = hoverColor;
            drawableUI = (DrawableUI)AddComponent(new DrawableUI(sprite));
            if (text != null)
            {
                AddComponent(new DrawableText(text, textColor, textAlignment, font));
            }
            int width = sprite.texture.Width;
            int height = sprite.texture.Height;
            rectangle = new Rectangle((int)position.X - width/2, (int)position.Y - height / 2, width, height);
            //Renderer.drawableRectangles.Add(rectangle);
            MouseDown += mouseDown;
            MouseUp += mouseUp;
        }
        public void UpdateRectangle()
        {
            rectangle = new Rectangle((int)Transform.Position.X - rectangle.Width / 2, (int)Transform.Position.Y - rectangle.Height / 2, rectangle.Width, rectangle.Height);
        }
        public Button(Sprite sprite, Vector2 position, Vector2 size, Color backgroundColor, Color hoverColor, string text = null, Color textColor = default, TextAlignment textAlignment = TextAlignment.Left, SpriteFont font = null, EventHandler mouseUp = null, EventHandler mouseDown = null) : base(position, size)
        {
            this.backgroundColor = backgroundColor;
            this.hoverColor = hoverColor;
            drawableUI = (DrawableUI)AddComponent(new DrawableUI(sprite));
            if (text != null)
            {
                AddComponent(new DrawableText(text, textColor, textAlignment, font));
            }
            int width = (int)(sprite.texture.Width * size.X);
            int height = (int)(sprite.texture.Height * size.Y);
            rectangle = new Rectangle((int)position.X - width / 2, (int)position.Y - height / 2, width, height);
            //Renderer.drawableRectangles.Add(rectangle);
            MouseDown += mouseDown;
            MouseUp += mouseUp;
        }

        public override void Update(GameTime gameTime)
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            Vector2 mousePosition = GameManager.MousePosition;

            if (IsHovered)
            {
                if (currentMouseState.LeftButton == ButtonState.Pressed)
                {
                    if (previousMouseState.LeftButton == ButtonState.Released)
                    {
                        IsClicked = true;
                        MouseDown?.Invoke(this, new EventArgs());
                    }
                }
                else
                {
                    if (previousMouseState.LeftButton == ButtonState.Pressed)
                    {
                        IsClicked = false;
                        MouseUp?.Invoke(this, new EventArgs());
                        OnClickAction?.Invoke(this);
                    }
                }
            }
            else
            {
                if (IsClicked)
                {
                    IsClicked = false;
                    MouseUp.Invoke(this, new EventArgs());
                }
            }
            IsHovered = rectangle.Contains(mousePosition);
            drawableUI.Color = IsHovered?hoverColor : backgroundColor;
        }
        internal override void OnDestroy()
        {
            //Renderer.drawableRectangles.Remove(rectangle);
            base.OnDestroy();
        }
    }
}
