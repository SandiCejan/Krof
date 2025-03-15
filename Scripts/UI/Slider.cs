using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace KrofEngine
{
    public class Slider : UpdatableGameObject
    {
        private Rectangle _backgroundRectangle;
        private Rectangle _thumbRectangle;
        private bool _isDragging;
        private float _value;
        DrawableUI drawableUI;
        Image slider;
        public float Value => _value;
        public Action<float> OnValueChanged { get; set; }
        public Action<float> OnStartDragging { get; set; }
        public Action<float> OnEndDragging { get; set; }

        public Slider(Vector2 position, int width, int height, float initialValue = 0, Action<float> onValueChanged = null, Action<float> onStartDragging = null, Action<float> onEndDragging = null) : base(position, new Vector2(width, height/2))
        {
            drawableUI = (DrawableUI)AddComponent(new DrawableUI(Renderer.Sprites[1]));
            slider = new Image(Renderer.Sprites[6], position + new Vector2(height/2, height/4), new Vector2(.75f, .75f), Color.Black);
            // Set initial positions
            _backgroundRectangle = new Rectangle((int)position.X, (int)position.Y, width, height);
            _thumbRectangle = new Rectangle((int)(position.X - height/2 + initialValue*width), (int)position.Y - height / 2, height, height);

            _value = initialValue;
            slider.Transform.position = new Vector2(_thumbRectangle.X + _thumbRectangle.Width / 2, slider.Transform.position.Y);
            _isDragging = false;
            OnValueChanged = onValueChanged;
            OnStartDragging = onStartDragging;
            OnEndDragging = onEndDragging;
            OnActiveChanged += delegate { slider.Active = Active; };
        }
        

        public override void Update(GameTime gameTime)
        {
            MouseState mouseState = GameManager.MouseState;
            MouseState previousMouseState = GameManager.PreviousMouseState;
            Vector2 mousePos = GameManager.MousePosition;
            if (mouseState.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released &&
                _thumbRectangle.Contains(mousePos))
            {
                _isDragging = true;
                OnStartDragging?.Invoke(_value);
            }

            if (_isDragging && mouseState.LeftButton == ButtonState.Pressed)
            {
                int newThumbX = Math.Clamp((int)mousePos.X, _backgroundRectangle.Left, _backgroundRectangle.Right);
                _thumbRectangle.X = newThumbX - _thumbRectangle.Width / 2;

                // Calculate the slider value (0.0 to 1.0)
                _value = (float)(_thumbRectangle.X + _thumbRectangle.Width / 2 - _backgroundRectangle.Left)/ _backgroundRectangle.Width;
                slider.Transform.position = new Vector2(_thumbRectangle.X + _thumbRectangle.Width / 2, slider.Transform.position.Y);
                OnValueChanged?.Invoke(_value);
            }

            if (mouseState.LeftButton == ButtonState.Released && _isDragging)
            {
                _isDragging = false;
                OnEndDragging?.Invoke(_value);
            }
        }
    }
}
