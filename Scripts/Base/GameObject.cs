using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace KrofEngine
{
    public class GameObject
    {
        internal static List<GameObject> AllObjects = new List<GameObject>();
        internal GameComponentCollection components = new();
        public Transform Transform { get; private set; }
        public bool Static
        {
            get { return Static; }
            set
            {
                if (_static != value)
                {
                    _static = value;
                    if (value) Physics.Movables.Remove(this);
                    else Physics.Movables.Add(this);
                }
            }
        }
        private bool _static = true;
        public List<Collider> colliders = new List<Collider>();
        public Collider collider;
        public Vector2 MoveAmount, ActualMove;
        public Action<bool> OnActiveChanged;
        public string Name;
        public void addCollider(Collider c)
        {
            collider = c;
            colliders.Add(c);
        }
        public void removeCollider(Collider c)
        {
            colliders.Remove(c);
            if (collider == c)
            {
                collider = (colliders.Count == 0 ?  null : colliders[colliders.Count-1]);
            }
        }
        public bool Active { get { return active; } set { if (active != value) {
                    active = value;
                    OnActiveChanged?.Invoke(value);
                    foreach (var item in components)
                    {
                        ((Component)item).Enabled = value;
                    }
                } } }
        private bool active = true;

        public GameObject(Vector2 position = default, Vector2 scale = default, float angle = 0)
        {
            Transform = new Transform(position, scale, angle);
            AllObjects.Add(this);
        }
        public override string ToString()
        {
            return GetType().Name;
        }
        public void DontDestroyOnLoad()
        {
            AllObjects.Remove(this);
        }
        internal static void DestroyAllObjects()
        {
            //AllObjects.Clear();
            //Physics.Setup();
            //Renderer.drawableObjects.Clear();
            //Renderer.drawableText.Clear();
            //GameManager.updateables.Clear();
            while (0 < AllObjects.Count)
            {
                AllObjects[0].Dest();
            }
        }
        public void Destroy()
        {
            GameManager.objectToDestroy.Add(this);
        }
        internal void Dest()
        {
            while (0 < components.Count)
            {
                ((Component)components[0]).Destroy();
            }
            OnDestroy();
            if (!_static)
            {
                Physics.Movables.Remove(this);
            }
            AllObjects.Remove(this);
            GameManager.objectToDestroy.Remove(this);
        }
        public T GetComponent<T>()
        {
            foreach (var item in components)
            {
                if (item.GetType() == typeof(T))
                {
                    return (T)item;
                }
            }
            return default(T);
        }
        public bool RemoveComponent<T>()
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].GetType() == typeof(T))
                {
                    components.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public Component AddComponent(Component component)
        {
            components.Add(component);
            component.gameObject = this;
            component.Awake();
            return component;
        }
        internal virtual void OnDestroy() { }
        public static implicit operator bool(GameObject obj)
        {
            return obj != null;
        }
        public void LookAt(GameObject go)
        {
            Vector2 dir = go.Transform.position - Transform.position;
            Transform.Angle = (float)(Math.Atan2(dir.Y, dir.X) + Math.PI/2);
        }
        //public T AddComponent<T>(params object[] args)
        //{
        //    Type type = typeof(T);
        //    var constructor = type.GetConstructors().FirstOrDefault(c => c.GetParameters().Length >= args.Length);
        //    if (constructor == null)
        //        throw new MissingMethodException($"No suitable constructor found for type {type}.");

        //    // Prepare the parameter list for the constructor
        //    var parameters = constructor.GetParameters();
        //    var finalArgs = new object[parameters.Length];
        //    for (int i = 0; i < finalArgs.Length; i++)
        //    {
        //        if (i < args.Length)
        //            finalArgs[i] = args[i]; // Use provided argument
        //        else
        //            finalArgs[i] = parameters[i].DefaultValue; // Use default value
        //    }
        //    instance = Activator.CreateInstance(type, finalArgs);
        //    c = (Component)instance;
        //    components.Add(c);
        //    c.gameObject = this;
        //    c.Awake();
        //    return (T)instance;
        //}
    }
}
