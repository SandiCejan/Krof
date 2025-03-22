using Microsoft.Xna.Framework;

namespace KrofEngine
{
    public class Component : GameComponent
    {
        public GameObject gameObject { get; internal set; }
        public Transform transform { get { return gameObject.Transform; } }
        Game game;
        public Component(Game game) : base(game)
        {
        }
        public T GetComponent<T>()
        {
            return gameObject.GetComponent<T>();
        }
        public Component AddComponent(Component component)
        {
            return gameObject.AddComponent(component);
        }
        public void Destroy()
        {
            OnDestroy();
            gameObject.components.Remove(this);
            gameObject = null;
        }
        internal virtual void OnDestroy() { }
        public virtual void Awake() { }
        public static implicit operator bool(Component obj)
        {
            return obj != null;
        }
    }
}
