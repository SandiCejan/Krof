namespace KrofEngine
{
    internal interface IScene
    {
        public IScene GenerateScene();
        public int index { get; }
        public void OnDestroy();
    }
}
