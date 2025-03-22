using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text;

namespace KrofEngine
{
    internal class InputField : UpdatableGameObject
    {
        private Rectangle rectangle;

        private StringBuilder inputText;
        private bool isFocused, isHovered;
        private double caretBlinkTime;
        private int caretPos = 0;
        private bool showCaret;

        private Keys[] pressedKeys;

        private Color backgroundColor;
        private Color focusColor;
        private Color hoverColor;

        public string Text => inputText.ToString();
        public bool IsEnterPressed { get; private set; }
        DrawableUI drawableUI;
        DrawableText drawableText;
        Image caret;
        int width, height;
        int maxLength;
        string placeholder;
        public InputFieldRules inputRules;

        public InputField(Sprite sprite, Vector2 position, Color backgroundColor, InputFieldRules inputFieldRules = null, int maxLength = 0, string placeholder = "", string text = "", Color textColor = default, TextAlignment textAlignment = TextAlignment.Left, SpriteFont font = null, Color focusColor = default, Color hoverColor = default) : base(position)
        {
            inputRules = inputFieldRules != null? inputFieldRules:new InputFieldRules();
            this.backgroundColor = backgroundColor;
            this.maxLength = maxLength;
            this.placeholder = placeholder;
            this.hoverColor = hoverColor;
            drawableUI = (DrawableUI)AddComponent(new DrawableUI(sprite));
            drawableText = (DrawableText)AddComponent(new DrawableText(text, textColor, textAlignment, font));
            width = sprite.texture.Width;
            height = sprite.texture.Height;
            if (textAlignment == TextAlignment.Left)
            {
                drawableText.MovePosition(new Vector2(-width / 2 + 10, -height / 4));
            }
            else if (textAlignment == TextAlignment.Right)
            {
                drawableText.MovePosition(new Vector2(width / 2 - 10, height / 4));
            }
            else
            {
                drawableText.MovePosition(new Vector2(0, 0));
            }
            if (text == "" && placeholder != "")
            {
                drawableText.Text = placeholder;
            }
            this.focusColor = focusColor;
            caret = new Image(Renderer.Sprites[1], Vector2.Zero, new Vector2(2, 22));

            rectangle = new Rectangle((int)position.X - width / 2, (int)position.Y - height / 2, width, height);
            inputText = new StringBuilder(text);
            caretPos = inputText.Length;
            OnActiveChanged += delegate { caret.Active = Active; };
        }
        internal override void OnDestroy()
        {
            caret.Destroy();
            base.OnDestroy();
        }

        public override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            Vector2 mousePos = GameManager.MousePosition;
            KeyboardState keyboardState = Keyboard.GetState();

            // Handle focus
            if (rectangle.Contains(mousePos))
            {
                isHovered = true;
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    isFocused = true;
                }
            }
            else if (mouseState.LeftButton == ButtonState.Pressed)
            {
                isFocused = false;
            }
            else
            {
                isHovered = false;
            }

            if (isFocused)
            {
                caretBlinkTime += gameTime.ElapsedGameTime.TotalSeconds;
                if (caretBlinkTime >= 0.5)
                {
                    showCaret = !showCaret;
                    caretBlinkTime = 0;
                }

                pressedKeys = keyboardState.GetPressedKeys();
                foreach (Keys key in pressedKeys)
                {
                    if (GameManager.PreviousKeyboardState.IsKeyDown(key))
                        continue;
                    if (key == Keys.Left && caretPos != 0)
                    {
                        caretPos--;
                    }
                    else if (key == Keys.Right && caretPos != inputText.Length)
                    {
                        caretPos++;
                    }
                    else if (key == Keys.Back && inputText.Length > 0)
                    {
                        inputText.Remove(caretPos - 1, 1);
                        caretPos--;
                        drawableText.Text = inputText.Length == 0 ? placeholder : inputText.ToString();
                    }
                    else if (key == Keys.Enter)
                    {
                        IsEnterPressed = true;
                    }
                    else if (key == Keys.Space)
                    {
                        if (inputText.Length == maxLength && maxLength != 0 || !inputRules.Spaces)
                        {
                            return;
                        }
                        inputText.Insert(caretPos, ' ');
                        caretPos++;
                        drawableText.Text = inputText.Length == 0 ? placeholder : inputText.ToString();
                    }
                    else if (key >= Keys.A && key <= Keys.Z)
                    {
                        if (inputText.Length == maxLength && maxLength != 0)
                        {
                            return;
                        }
                        bool shift = keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift);
                        if (shift && !inputRules.UpperCharacters || !shift && !inputRules.LowerCharacters)
                        {
                            return;
                        }
                        char c = (char)(key - Keys.A + (shift ? 'A' : 'a'));
                        inputText.Insert(caretPos, c);
                        caretPos++;
                        drawableText.Text = inputText.Length == 0 ? placeholder : inputText.ToString();
                    }
                    else if (key >= Keys.D0 && key <= Keys.D9)
                    {
                        if (inputText.Length == maxLength && maxLength != 0 || !inputRules.Numbers)
                        {
                            return;
                        }
                        inputText.Insert(caretPos, (char)(key - Keys.D0 + '0'));
                        caretPos++;
                        drawableText.Text = inputText.Length == 0 ? placeholder : inputText.ToString();
                    }
                }
            }
            else
            {
                IsEnterPressed = false;
            }
            if (isHovered)
            {
                if (isFocused)
                {
                    drawableUI.Color = focusColor;
                }
                else
                {
                    drawableUI.Color = hoverColor;
                }
            }
            else
            {
                if (isFocused)
                {
                    drawableUI.Color = focusColor;
                }
                else
                {
                    drawableUI.Color = backgroundColor;
                }
            }
            if (isFocused && showCaret)
            {
                Vector2 textSize = drawableText.Font.MeasureString(inputText.ToString().Substring(0, caretPos));
                if (textSize.Y == 0)
                {
                    textSize.Y = 28;
                }
                textSize.Y += 16;
                switch (drawableText.TextAlignment)
                {
                    case TextAlignment.Left:
                        Vector2 textPosition = new Vector2(
                            Transform.Position.X - width / 2 + 8,
                            Transform.Position.Y - (textSize.Y / 4)
                        );
                        Vector2 caretPosition = new Vector2(textPosition.X + textSize.X, textPosition.Y);
                        caret.Transform.position = caretPosition;
                        break;
                    case TextAlignment.Middle:
                        textPosition = new Vector2(
                            Transform.Position.X,
                            Transform.Position.Y - (textSize.Y / 4)
                        );
                        Vector2 textSize2 = drawableText.Font.MeasureString(inputText.ToString());
                        caretPosition = new Vector2(textPosition.X - textSize2.X / 2 + textSize.X, textPosition.Y);
                        caret.Transform.position = caretPosition;
                        break;
                    case TextAlignment.Right:
                        textPosition = new Vector2(
                            Transform.Position.X + width / 2 - 8,
                            Transform.Position.Y - (textSize.Y / 4)
                        );
                        textSize2 = drawableText.Font.MeasureString(inputText.ToString());
                        caretPosition = new Vector2(textPosition.X - textSize2.X + textSize.X, textPosition.Y);
                        caret.Transform.position = caretPosition;
                        break;
                    default:
                        break;
                }
                caret.Active = true;
            }
            else
            {
                caret.Active = false;
            }
        }
    }
}
