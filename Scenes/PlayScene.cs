using KrofEngine;

namespace Krof
{
    internal class PlayScene : IScene
    {
        private int idx;
        public int index { get { return idx; } }
        public PlayScene() {
            idx = GameManager.scenes.Count;
        }
        public IScene GenerateScene()
        {
            new PlaySceneManager();
            return this;
        }

        public void OnDestroy()
        {
        }
    }
}
