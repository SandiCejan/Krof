using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace KrofEngine
{
    public class Quadtree
    {
        private int MAX_OBJECTS = 10;
        private int MAX_LEVELS = 50;
        private int level;
        private List<Collider> objects;
        private Rectangle bounds;
        private Quadtree[] nodes;
        private Quadtree parent;
        /* 
       * Constructor 
       */
        public Quadtree(int pLevel, Rectangle pBounds)
        {
            level = pLevel;
            objects = new();
            bounds = pBounds;
            nodes = new Quadtree[4];
            parent = this;
        }
        private Quadtree(int pLevel, Rectangle pBounds, Quadtree parent)
        {
            level = pLevel;
            objects = new();
            bounds = pBounds;
            nodes = new Quadtree[4];
            this.parent = parent;
        }
        public void clear()
        {
            objects.Clear();
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] != null)
                {
                    nodes[i].clear();
                    nodes[i] = null;
                }
            }
        }
        private void split()
        {
            int subWidth = bounds.Width / 2;
            int subHeight = bounds.Height / 2;
            int x = bounds.X;
            int y = bounds.Y;
            nodes[0] = new Quadtree(level + 1, new Rectangle(x + subWidth, y, subWidth, subHeight), this);
            nodes[1] = new Quadtree(level + 1, new Rectangle(x, y, subWidth, subHeight), this);
            nodes[2] = new Quadtree(level + 1, new Rectangle(x, y + subHeight, subWidth, subHeight), this);
            nodes[3] = new Quadtree(level + 1, new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight), this);
        }
        private int getIndex(Collider pRect)
        {
            int index = -1;
            double verticalMidpoint = bounds.X + (bounds.Width / 2);
            double horizontalMidpoint = bounds.Y + (bounds.Height / 2);
            // Object can completely fit within the top quadrants 
            bool topQuadrant = (pRect.Position.Y < horizontalMidpoint && pRect.Position.Y + pRect.Height < horizontalMidpoint);
            // Object can completely fit within the bottom quadrants 
            bool bottomQuadrant = (pRect.Position.Y > horizontalMidpoint);
            // Object can completely fit within the left quadrants 
            if (pRect.Position.X < verticalMidpoint && pRect.Position.X + pRect.Width < verticalMidpoint)
            {
                if (topQuadrant)
                {
                    index = 1;
                }
                else if (bottomQuadrant)
                {
                    index = 2;
                }
            }
            // Object can completely fit within the right quadrants 
            else if (pRect.Position.X > verticalMidpoint)
            {
                if (topQuadrant)
                {
                    index = 0;
                }
                else if (bottomQuadrant)
                {
                    index = 3;
                }
            }
            return index;
        }
        public void insert(Collider pRect)
        {
            if (nodes[0] != null)
            {
                int index = getIndex(pRect);
                if (index != -1)
                {
                    nodes[index].insert(pRect);
                    return;
                }
            }
            objects.Add(pRect);
            if (objects.Count > MAX_OBJECTS && level < MAX_LEVELS)
            {
                if (nodes[0] == null)
                {
                    split();
                }
                int i = 0;
                while (i < objects.Count)
                {
                    int index = getIndex(objects[i]);
                    if (index != -1)
                    {
                        nodes[index].insert(objects[i]);
                        objects.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }
        public List<Collider> retrieve(List<Collider> returnObjects, Collider pRect)
        {
            int index = getIndex(pRect);
            if (index != -1 && nodes[0] != null)
            {
                nodes[index].retrieve(returnObjects, pRect);
            }
            else
            {

            }
            returnObjects.AddRange(objects);
            return returnObjects;
        }
    } 
}