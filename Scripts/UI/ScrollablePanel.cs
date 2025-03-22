using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace KrofEngine
{
    internal class ScrollablePanel : UpdatableGameObject
    {
        public List<GameObject> Children;
        Rectangle rectangle;
        public int ScrollSensitivity, MaxScroll;
        private int currentScroll = 0, rowsNum;
        private List<bool> activeStates;
        private List<bool> updateButtons;
        public ScrollablePanel(int posX, int posY, int width, int height, int scrollSensitivity, int maxScroll = 0, List<GameObject> children = null) : base()
        {
            Children = children != null ? children : new();
            OnActiveChanged += onActiveChanged;
            rectangle = new Rectangle(posX, posY, width, height);
            ScrollSensitivity = scrollSensitivity;
            MaxScroll = maxScroll;
            rowsNum = rectangle.Height / ScrollSensitivity;
            activeStates = new();
            updateButtons = new();
        }
        public void AddChild(GameObject child)
        {
            Children.Add(child);
            activeStates.Add(child.Active);
            Vector2 p = child.Transform.Position;
            if (!Active || p.Y > rectangle.Height || p.X < 0)
            {
                child.Active = false;
            }
            int Scroll = (int)child.Transform.Position.Y / ScrollSensitivity - rowsNum;
            if (MaxScroll < Scroll)
            {
                MaxScroll = Scroll;
            }
            p.Y += rectangle.Y;
            child.Transform.Position = p;
            if (child.GetType() == typeof(Button))
            {
                ((Button)child).UpdateRectangle();
                updateButtons.Add(true);
            }
            else
            {
                updateButtons.Add(false);
            }
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 mousePosition = GameManager.MousePosition;

            if (rectangle.Contains(GameManager.MousePosition))
            {
                if (GameManager.MouseState.ScrollWheelValue != GameManager.PreviousMouseState.ScrollWheelValue)
                {
                    Vector2 pos;
                    int i = 0;
                    if (GameManager.MouseState.ScrollWheelValue > GameManager.PreviousMouseState.ScrollWheelValue)
                    {
                        //scroll down
                        if (currentScroll == 0) return;
                        currentScroll--;
                        foreach (var item in Children)
                        {
                            pos = item.Transform.Position;
                            pos.Y += ScrollSensitivity;
                            item.Transform.Position = pos;
                            if (pos.Y < rectangle.Y)
                            {
                                item.Active = false;
                            }else if (pos.Y > rectangle.Y + rectangle.Height)
                            {
                                item.Active = false;
                            }
                            else
                            {
                                item.Active = true;
                            }
                            if (updateButtons[i]) ((Button)item).UpdateRectangle();
                            i++;
                        }
                    }
                    else
                    {
                        //scroll up
                        if (currentScroll == MaxScroll) return;
                        currentScroll++;
                        foreach (var item in Children)
                        {
                            pos = item.Transform.Position;
                            pos.Y -= ScrollSensitivity;
                            item.Transform.Position = pos;
                            if (pos.Y < rectangle.Y)
                            {
                                item.Active = false;
                            }
                            else if (pos.Y > rectangle.Y + rectangle.Height)
                            {
                                item.Active = false;
                            }
                            else
                            {
                                item.Active = true;
                            }
                            if (updateButtons[i]) ((Button)item).UpdateRectangle();
                            i++;
                        }
                    }
                }
            }
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
