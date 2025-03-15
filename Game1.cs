using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace KrofEngine
{
    public class Game1 : Game
    {
        // Engine parts
        Renderer _renderer;
        GameManager _gameManager;
        Input _input;
        AIEngine _AIEngine;
        Physics _physics;
        SoundEngine _soundEngine;

        // Engine essentials
        public static Game1 Instance;
        public static GraphicsDeviceManager Graphics { get; private set; }
        public static int ScreenWidth { get; internal set; }
        public static int ScreenHeight { get; internal set; }
        public const int GameWidth = 1920;
        public const int GameHeight = 1080;
        public static List<DisplayMode> supportedDisplayModes;
        static DisplayMode HighestDisplayMode;
        public Game1()
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = false,
                PreferredBackBufferWidth = GameWidth,
                PreferredBackBufferHeight = GameHeight,
                SynchronizeWithVerticalRetrace = true,
            };
            //Graphics.IsFullScreen = true;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += Resize;
            //Graphics.PreferredBackBufferWidth = ScreenWidth;
            //Graphics.PreferredBackBufferHeight = ScreenHeight;
            //Graphics.ApplyChanges();
            //IsMouseVisible = false;
            Instance = this;
            supportedDisplayModes = new();
            foreach (var item in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                supportedDisplayModes.Add(item);
            }
            HighestDisplayMode = supportedDisplayModes.Last();
        }
        public static Vector2 ScreenSize { get { return new Vector2(HighestDisplayMode.Width, HighestDisplayMode.Height); } }

        public void Resize(object sender, EventArgs e)
        {
            ScreenWidth = Window.ClientBounds.Width;
            ScreenHeight = Window.ClientBounds.Height;
            GameManager.OnResize?.Invoke();
            //Graphics.PreferredBackBufferWidth = ScreenWidth;
            //Graphics.PreferredBackBufferHeight = ScreenHeight;
            //Graphics.ApplyChanges();
            Camera.Resize();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            GameManager.OnApplicationQuit.Invoke();
            base.OnExiting(sender, args);
        }

        protected override void Initialize()
        {
            _renderer = new Renderer(GraphicsDevice);
            _gameManager = new GameManager();
            _input = new Input();
            _physics = new Physics();
            _AIEngine = new AIEngine();
            _soundEngine = new SoundEngine();
            _gameManager.Initialize();
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.F1))
                Exit();
            _input.Update(gameTime);
            _AIEngine.Update(gameTime);
            _gameManager.Update(gameTime);
            _physics.Update(gameTime);
            _soundEngine.Update(gameTime);
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            _renderer.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
