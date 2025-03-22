using KrofEngine;
using Microsoft.Xna.Framework;

namespace Krof
{
    internal class TutorialObject : UpdatableGameObject
    {
        Text text;
        float time = 8;
        internal TutorialObject() {
            text = new Text(new Vector2(20, 50), "TUTORIAL" +
                "\nW to move up" +
                "\nS to move down" +
                "\nA to move left" +
                "\nD to move right", Color.White, font: Renderer.Fonts[1]);
            GameManager.OnPause += OnPause;
            GameManager.OnPlay += OnPlay;
            Enabled = false;
        }
        void OnPause()
        {
            Enabled = false;
        }
        void OnPlay()
        {
            Enabled = true;
        }
        internal override void OnDestroy()
        {
            GameManager.OnPause -= OnPause;
            GameManager.OnPlay -= OnPlay;
            base.OnDestroy();
        }
        public override void Update(GameTime gameTime)
        {
            time -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (time < 0)
            {
                text.Destroy();
                Destroy();
            }
        }
    }
}
