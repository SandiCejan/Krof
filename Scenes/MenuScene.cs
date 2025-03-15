using KrofEngine;

namespace Krof
{
    internal class MenuScene : IScene
    {
        private int idx;
        public int index { get { return idx; } }
        public MenuScene()
        {
            idx = GameManager.scenes.Count;
        }
        public IScene GenerateScene()
        {
            new MenuManager();
            return this;
        }

        public void OnDestroy()
        {
        }
    }
}
