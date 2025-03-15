using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace KrofEngine
{
    internal class Panel : GameObject
    {
        public List<GameObject> Children;
        private List<bool> activeStates;
        public Panel(List<GameObject> children = null) : base()
        {
            if (children != null)
            {
                Children = children;
                activeStates = new();
                foreach (var item in children)
                {
                    activeStates.Add(item.Active);
                }
            }
            else
            {
                Children = new();
                activeStates = new();
            }
            OnActiveChanged += onActiveChanged;
        }
        public void Move(Vector2 amount)
        {
            Transform.Position += amount;
            foreach (var item in Children)
            {
                item.Transform.Position += amount;
            }
        }

        public void AddChild(GameObject child)
        {
            Children.Add(child);
            activeStates.Add(child.Active);
            child.Active = Active;
        }
        private void onActiveChanged(bool active)
        {
            if (active)
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    Children[i].Active = activeStates[i];
                }
            }
            else
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    activeStates[i] = Children[i].Active;
                    Children[i].Active = false;
                }
            }
        }
    }
}
