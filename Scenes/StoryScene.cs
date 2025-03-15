using Krof.Scenes;
using KrofEngine;

namespace Krof
{
    internal class StoryScene : IScene
    {
        private int idx;
        public int index { get { return idx; } }
        public StoryScene()
        {
            idx = GameManager.scenes.Count;
        }
        StoryManager sm;
        public IScene GenerateScene()
        {
            sm = new StoryManager();
            new PlaySceneManager();
            sm.OnGenerated();
            return this;
        }

        public void OnDestroy()
        {
            sm.SaveState();
            sm = null;
        }
    }
}
