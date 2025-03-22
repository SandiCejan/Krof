using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Collections.Generic;

namespace KrofEngine
{
    internal class Renderer
    {
        public static ArrayList drawableObjects;
        public static ArrayList drawableUI;
        public static ArrayList drawableText;
        public static ArrayList drawableRectangleColliders = new();
        public static ArrayList drawableRectangles = new();
        public static ArrayList drawableCircleColliders = new();
        public static SpriteBatch spriteBatch;
        public static List<Sprite> Sprites;
        public static List<SpriteFont> Fonts;
        public static GraphicsDeviceManager Graphics;
        internal SpriteBatch _spriteBatch;
        public static Color backgroundColor;
        public static int FOV = 1300;
        public static int GlobalObjectSize = 200;
        public static int numOfObjectsRendered {  get; private set; }
        GraphicsDevice graphicsDevice;
        Game game1;
        public static Vector2 dest;
        public Renderer(GraphicsDevice graphicsDevice)
        {
            game1 = Game1.Instance;
            drawableObjects = new();
            drawableText = new();
            drawableUI = new();
            Sprites = new();
            Fonts = new();
            this.graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(graphicsDevice);
            Rectangle[] rectangles = new Rectangle[20];
            for (int i = 0; i < 20; i++)
            {
                rectangles[i] = new Rectangle(200*i, 0, 200, 200);
            }
            Sprites.Add(new Sprite(game1.Content.Load<Texture2D>("PlayerSprite"), new Rectangle[] { new Rectangle(0, 0, 200, 200), new Rectangle(200, 0, 200, 200), new Rectangle(200, 200, 200, 200), new Rectangle(0, 200, 200, 200) }, new Vector2(100, 100)));
            Sprites.Add(new Sprite(game1.Content.Load<Texture2D>("Wall"), new Rectangle[] { new Rectangle(0, 0, 1, 1) }, Vector2.Zero));
            Sprites.Add(new Sprite(game1.Content.Load<Texture2D>("MonsterNormalSprite"), new Rectangle[] { new Rectangle(0, 0, 200, 200), new Rectangle(200, 0, 200, 200), new Rectangle(0, 200, 200, 200), new Rectangle(200, 200, 200, 200) }, new Vector2(100, 100)));
            Sprites.Add(new Sprite(game1.Content.Load<Texture2D>("MonsterShootSprite"), new Rectangle[] { new Rectangle(0, 0, 200, 200), new Rectangle(200, 0, 200, 200), new Rectangle(200, 200, 200, 200), new Rectangle(0, 200, 200, 200) }, new Vector2(100, 100)));
            Sprites.Add(new Sprite(game1.Content.Load<Texture2D>("Doors"), rectangles, new Vector2(100, 100)));
            Sprites.Add(new Sprite(game1.Content.Load<Texture2D>("EscapeCard"), new Rectangle[] { new Rectangle(0, 0, 200, 200) }, new Vector2(100, 100)));
            Sprites.Add(new Sprite(game1.Content.Load<Texture2D>("Bullet"), new Rectangle[] { new Rectangle(0, 0, 40, 40) }, new Vector2(20, 20)));
            Sprites.Add(new Sprite(game1.Content.Load<Texture2D>("ButtonBackground"), new Rectangle[] { new Rectangle(0, 0, 390, 50) }, new Vector2(390 / 2, 25))); // 7
            Sprites.Add(new Sprite(game1.Content.Load<Texture2D>("edit"), new Rectangle[] { new Rectangle(0, 0, 512, 512) }, new Vector2(256, 256)));
            Sprites.Add(new Sprite(game1.Content.Load<Texture2D>("MonsterBossSprite"), new Rectangle[] { new Rectangle(0, 0, 200, 200), new Rectangle(200, 0, 200, 200), new Rectangle(0, 200, 200, 200), new Rectangle(200, 200, 200, 200) }, new Vector2(100, 100)));
            Sprites.Add(new Sprite(game1.Content.Load<Texture2D>("Explosive"), new Rectangle[] { new Rectangle(0, 0, 100, 100), new Rectangle(100, 0, 100, 100), new Rectangle(200, 0, 100, 100), new Rectangle(0, 100, 100, 100), new Rectangle(100, 100, 100, 100), new Rectangle(200, 100, 100, 100) }, new Vector2(50, 50))); // 10
            Sprites.Add(new Sprite(game1.Content.Load<Texture2D>("ButtonBackgroundSquare"), new Rectangle[] { new Rectangle(0, 0, 30, 30) }, new Vector2(15, 15)));
            Sprites.Add(new Sprite(game1.Content.Load<Texture2D>("Background"), new Rectangle[] { new Rectangle(0, 0, 1920,1080) }, new Vector2(0, 0)));
            Sprites.Add(new Sprite(game1.Content.Load<Texture2D>("WhiteBackgroundRounded"), new Rectangle[] { new Rectangle(0, 0, 800,390) }, new Vector2(0, 0)));
            Sprites.Add(new Sprite(game1.Content.Load<Texture2D>("WhiteBackgroundRounded2"), new Rectangle[] { new Rectangle(0, 0, 700,210) }, new Vector2(0, 0)));
            Sprites.Add(new Sprite(game1.Content.Load<Texture2D>("WhiteBackgroundRounded3"), new Rectangle[] { new Rectangle(0, 0, 460,300) }, new Vector2(0, 0)));
            Sprites.Add(new Sprite(game1.Content.Load<Texture2D>("WhiteBackgroundRounded4"), new Rectangle[] { new Rectangle(0, 0, 1000,500) }, new Vector2(0, 0)));
            Fonts.Add(game1.Content.Load<SpriteFont>("NormalFont"));
            Fonts.Add(game1.Content.Load<SpriteFont>("SmallFont"));
            Fonts.Add(game1.Content.Load<SpriteFont>("SmallFontInput"));
            backgroundColor = new Color(203, 203, 203);
        }
        public void Draw(GameTime gameTime)
        {
            graphicsDevice.Clear(backgroundColor);
            _spriteBatch.Begin(transformMatrix: Camera.Transform);
            //DEV TOOLS
            //_spriteBatch.Begin();
            //if (Game1.playerTransform != null)
            //{
            //    _spriteBatch.Draw(Sprites[1].texture, Game1.playerTransform.Position - new Vector2(25, 25), new Rectangle(0, 0, 1, 1), Color.White, 0, Vector2.Zero, new Vector2(50, 50), SpriteEffects.None, 0);
            //}
            Vector2 cameraPos = new Vector2(-Camera.Position.X, -Camera.Position.Y);
            //numOfObjectsRendered = 0;
            foreach (DrawableItem item in drawableObjects)
            {
                if (item.Enabled && (item.GlobalDraw || Vector2.Distance(cameraPos, item.transform.Position) < FOV))
                {
                    _spriteBatch.Draw(item.Sprite.texture, item.transform.Position, item.Sprite.rectangles[item.RectangleIndex], item.Color, item.transform.Angle, item.Sprite.origin, item.transform.Scale, SpriteEffects.None, 0);
                    //numOfObjectsRendered++;
                }
            }
            //if (dest != default)
            //{
            //    _spriteBatch.Draw(Sprites[1].texture, new Rectangle((int)dest.X, (int)dest.Y, 10, 10), Color.White);
            //}

            //foreach (RectangleCollider item in drawableRectangleColliders)
            //{
            //    if (item.Enabled)
            //    {
            //        _spriteBatch.Draw(Sprites[1].texture, item.Position, new Rectangle(0, 0, 1, 1), Color.White, 0, Vector2.Zero, new Vector2(item.Width, item.Height), SpriteEffects.None, 0);
            //    }
            //}
            //foreach (CircleCollider item in drawableCircleColliders)
            //{
            //    if (item.Enabled)
            //    {
            //        _spriteBatch.Draw(Sprites[1].texture, item.Position, new Rectangle(0, 0, 1, 1), Color.White, 0, Vector2.Zero, new Vector2(item.Width, item.Height), SpriteEffects.None, 0);
            //    }
            //}
            _spriteBatch.End();
            _spriteBatch.Begin(transformMatrix: Camera.UITransform);
            foreach (DrawableUI item in drawableUI)
            {
                if (item.Enabled)
                {
                    _spriteBatch.Draw(item.Sprite.texture, item.transform.Position, item.Sprite.rectangles[item.RectangleIndex], item.Color, item.transform.Angle, item.Sprite.origin, item.transform.Scale, SpriteEffects.None, 0);
                }
            }
            foreach (DrawableText item in drawableText)
            {
                if (item.Enabled)
                {
                    _spriteBatch.DrawString(item.Font, item.Text, item.position, item.Color);
                }
            }
            foreach (Rectangle item in drawableRectangles)
            {
                _spriteBatch.Draw(Sprites[1].texture, item, Color.White);
            }
            _spriteBatch.End();
        }
    }
}
