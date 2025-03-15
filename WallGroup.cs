using KrofEngine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Krof
{
    internal class WallGroup
    {
        public List<Wall> walls = new List<Wall>(3);
        public byte state = 0;
        float color;
        byte colorA;
        float time;
        public WallGroup()
        {
            color = 203;
            colorA = 203;
        }
        public WallGroup StartClosing()
        {
            foreach (var item in walls)
            {
                item.collider.Trigger = true;
                item.collider.Layer = 4;
                item.collider.Enabled = true;
            }
            state = 1;
            time = 0;
            return this;
        }
        public void Update(float change)
        {
            switch (state)
            {
                case 1:
                    color += change * 10;
                    if (color > 255)
                    {
                        color = 255;
                        state = 2;
                        foreach (var item in walls)
                        {
                            item.collider.Trigger = false;
                            item.collider.Layer = 0;
                            Physics.ForceCheckCollissionsStatic(item.collider);
                        }
                    }
                    colorA = (byte)color;
                    foreach (var item in walls)
                    {
                        item.drawableItem.Color.A = colorA;
                    }
                    break;
                case 2:
                    time += change;
                    if (time > 4)
                    {
                        state = 3;
                    }
                    break;
                default:
                    color -= change * 20;
                    if (color < 203)
                    {
                        color = 203;
                        state = 0;
                        PlaySceneManager.updatableWallGroupsToRemove.Add(this);
                        foreach (var item in walls)
                        {
                            item.collider.Enabled = false;
                        }
                    }
                    colorA = (byte)color;
                    foreach (var item in walls)
                    {
                        item.drawableItem.Color.A = colorA;
                    }
                    break;
            }
        }
    }
}
