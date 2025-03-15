using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace KrofEngine
{
    public class Physics
    {
        public static bool[,] LayerCollision;
        public static int NumOfLayers;
        //public static List<List<Collider>> Colliders;
        public static List<List<List<Collider>>> Colliders;
        public static List<Collider> GlobalColliders;
        public static List<GameObject> Movables;
        public static int ChunkSize;
        private static int moveIterations = 5;
        //Quadtree quad;
        //public static List<Collider> Colliders2;

        //Temps
        static RectangleCollider rect1, rect2;
        static CircleCollider circ1, circ2;
        static float tempf1, tempf2, tempf3, tempf4;
        static int tempi1, tempi2, tempi3, tempi4, tempi5;
        static Collider tempCollider, tempCollider2, tempCollider3, tempCollider4;
        static Vector2 tempVec, tempVec2;
        static bool tempb1;
        static int debugNum;
        public static bool Active = true;
        public Physics()
        {
            // 0 = wall
            // 1 = player
            // 2 = collideOnlyPlayer
            // 3 = monster
            // 4 = collideOnlyMonster
            // 5 = bullet (player, wall)
            LayerCollision = new bool[,] {
                //  0     1     2      3      4     5
                { false, true, false, true, false, true }, // 0
                { true,  false, true, true, true, true  }, // 1
                { false,  true, false, false, false, false }, // 2
                { true,  true, false, true, true, false }, // 3
                { false, true, false, true, false, false},// 4
                { true, true, false, false, false, false} };// 5
            NumOfLayers = LayerCollision.GetLength(0);
            //Colliders = new List<List<Collider>>();
            //Colliders2 = new();
            Movables = new();
            //for (int i = 0; i < NumOfLayers; i++)
            //{
            //    Colliders.Add(new List<Collider>());
            //}
        }
        public static void Setup(int SizeX = 0, int SizeY = 0, int chunkSize = 200)
        {
            Colliders = new();
            GlobalColliders = new();
            ChunkSize = chunkSize;
            if (SizeX == 0 || SizeY == 0) return;
            int XNum = SizeX / ChunkSize;
            int YNum = SizeY / ChunkSize;
            for (int i = 0; i <= XNum; i++)
            {
                Colliders.Add(new());
                for (int j = 0; j <= YNum; j++)
                {
                    Colliders[i].Add(new());
                }
            }
        }
        public void Update(GameTime gameTime)
        {
            if(!Active) return;
            foreach (GameObject item1 in Movables)
            {
                foreach (Collider item in item1.colliders)
                {
                    if (item.Trigger)
                    {
                        tempCollider2 = ForceCheckCollissions(item);
                        if (tempCollider2)
                        {
                            ((ICollider)item).TriggeredWith(tempCollider2);
                            ((ICollider)tempCollider2).TriggeredWith(item);
                        }
                    }
                    else
                    {
                        tempVec2 = item1.Transform.Position;
                        if (item1.MoveAmount != Vector2.Zero)
                        {
                            tempi4 = item.x;
                            tempi5 = item.y;
                            item1.Transform.Position += item1.MoveAmount;
                            if (!item.Enabled) continue;
                            tempf3 = item1.Transform.Position.X / ChunkSize;
                            tempi1 = (int)tempf3;
                            tempf4 = item1.Transform.Position.Y / ChunkSize;
                            tempi2 = (int)tempf4;
                            try
                            {
                                checkMove(item1, Colliders[tempi1][tempi2]);
                                try
                                {
                                    checkMove(item1, Colliders[tempi1 + (tempf3 % 1 > .5f ? 1 : -1)][tempi2]);
                                }
                                catch (Exception) { }
                                try
                                {
                                    checkMove(item1, Colliders[tempi1][tempi2 + (tempf4 % 1 > .5f ? 1 : -1)]);
                                }
                                catch (Exception) { }
                                try
                                {
                                    checkMove(item1, Colliders[tempi1 + (tempf3 % 1 > .5f ? 1 : -1)][tempi2 + (tempf4 % 1 > .5f ? 1 : -1)]);
                                }
                                catch (Exception) { }
                                checkMove(item1, GlobalColliders);
                                item.x = (int)item1.Transform.Position.X / ChunkSize;
                                item.y = (int)item1.Transform.Position.Y / ChunkSize;
                                if (tempi4 != item.x || tempi5 != item.y)
                                {
                                    Colliders[tempi4][tempi5].Remove(item);
                                    Colliders[item.x][item.y].Add(item);
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        item1.ActualMove = item1.Transform.Position - tempVec2;
                    }

                }
            }
            //Debug.WriteLine("Checks: " + debugNum);
            //debugNum = 0;
        }
        private void checkMove(GameObject gameObject, List<Collider> colliders)
        {
            tempCollider4 = gameObject.collider;
            for (int i = 0; i < colliders.Count; i++)
            {
                if (checkCollissions(tempCollider4, colliders[i]))
                {
                    if (colliders[i].Trigger)
                    {
                        ((ICollider)tempCollider4).TriggeredWith(colliders[i]);
                        ((ICollider)colliders[i]).TriggeredWith(tempCollider4);
                    }
                    else
                    {
                        ((ICollider)tempCollider4).CollidedWith(colliders[i]);
                        ((ICollider)colliders[i]).CollidedWith(tempCollider4);
                        tempVec = gameObject.MoveAmount / 2;
                        gameObject.Transform.Position -= tempVec;
                        tempi3 = 0;
                        for (int j = 0; j < moveIterations; j++)
                        {
                            tempVec /= 2;
                            if (checkCollissions(tempCollider4, colliders[i]))
                            {
                                tempi3++;
                                gameObject.Transform.Position -= tempVec;
                            }
                            else
                            {
                                gameObject.Transform.Position += tempVec;
                            }
                        }
                        if (tempi3 == moveIterations)
                        {
                            gameObject.Transform.Position = new Vector2(MathF.Round(gameObject.Transform.Position.X), MathF.Round(gameObject.Transform.Position.Y));
                        }
                    }
                }
            }
        }
        public static void ForceCheckCollissionsStatic(Collider collider)
        {
            for (int i = 0; i < Movables.Count; i++)
            {
                tempCollider3 = Movables[i].collider;
                if (checkCollissions(collider, tempCollider3))
                {
                    if (collider.Trigger)
                    {
                        ((ICollider)tempCollider3).TriggeredWith(collider);
                        ((ICollider)collider).TriggeredWith(tempCollider3);
                    }
                    else
                    {
                        ((ICollider)tempCollider3).CollidedWith(collider);
                        ((ICollider)collider).CollidedWith(tempCollider3);
                    }
                }
            }
        }
        private static Collider CheckCollissions(Collider collider, List<Collider> colliders)
        {
            for (int i = 0; i < colliders.Count; i++)
            {
                if (checkCollissions(collider, colliders[i])) return colliders[i];
            }
            //for (int i = 0; i < Movables.Count; i++)
            //{
            //    if (checkCollissions(collider, Movables[i].collider)) return Movables[i].collider;
            //}
            for (int i = 0; i < GlobalColliders.Count; i++)
            {
                if (checkCollissions(collider, GlobalColliders[i])) return GlobalColliders[i];
            }
            return null;
        }
        public static Collider ForceCheckCollissions(Collider collider)
        {
            tempf3 = collider.transform.Position.X / ChunkSize;
            tempi1 = (int)tempf3;
            tempf4 = collider.transform.Position.Y / ChunkSize;
            tempi2 = (int)tempf4;
            try
            {
                tempCollider = CheckCollissions(collider, Colliders[tempi1][tempi2]);
                if (tempCollider) return tempCollider;
            }
            catch (Exception) { }
            try
            {
                tempCollider = CheckCollissions(collider, Colliders[tempi1 + (tempf3 % 1 > .5f ? 1 : -1)][tempi2]);
                if (tempCollider) return tempCollider;
            }
            catch (Exception) { }
            try
            {
                tempCollider = CheckCollissions(collider, Colliders[tempi1][tempi2 + (tempf4 % 1 > .5f ? 1 : -1)]);
                if (tempCollider) return tempCollider;
            }
            catch (Exception) { }
            try
            {
                tempCollider = CheckCollissions(collider, Colliders[tempi1 + (tempf3 % 1 > .5f ? 1 : -1)][tempi2 + (tempf4 % 1 > .5f ? 1 : -1)]);
                return tempCollider;
            }
            catch (Exception) { }
            return null;
        }
        private static bool checkCollissions(Collider collider1, Collider collider2)
        {
            //debugNum++;
            if (LayerCollision[collider1.Layer, collider2.Layer] && collider1.Enabled && collider2.Enabled && collider1 != collider2)
            {
                if (collider1.GetType() == typeof(RectangleCollider) && collider2.GetType() == typeof(RectangleCollider))
                {
                    rect1 = (RectangleCollider)collider1;
                    rect2 = (RectangleCollider)collider2;
                    return rect1.Position.X < rect2.Position.X + rect2.Width && rect1.Position.X + rect1.Width > rect2.Position.X && rect1.Position.Y < rect2.Position.Y + rect2.Height && rect1.Position.Y + rect1.Height > rect2.Position.Y;
                }
                else if (collider1.GetType() == typeof(CircleCollider) && collider2.GetType() == typeof(CircleCollider))
                {
                    circ1 = (CircleCollider)collider1;
                    circ2 = (CircleCollider)collider2;
                    return (circ1.Position - circ2.Position).Length() < circ1.radius + circ2.radius;
                }
                else if ((collider1.GetType() == typeof(RectangleCollider) && collider1.GetType() == typeof(CircleCollider)) || (collider2.GetType() == typeof(RectangleCollider) && collider2.GetType() == typeof(CircleCollider)))
                {
                    if (collider1.GetType() == typeof(RectangleCollider))
                    {
                        rect1 = (RectangleCollider)collider1;
                        circ1 = (CircleCollider)collider2;
                    }
                    else
                    {
                        rect1 = (RectangleCollider)collider2;
                        circ1 = (CircleCollider)collider1;
                    }
                    tempf1 = Math.Clamp(circ1.Position.X, rect1.Position.X, rect1.Position.X + rect1.Width);
                    tempf2 = Math.Clamp(circ1.Position.Y, rect1.Position.Y, rect1.Position.Y + rect1.Height);
                    tempf1 = circ1.Position.X - tempf1;
                    tempf2 = circ1.Position.Y - tempf2;
                    tempf1 = tempf1 * tempf1 + tempf2 * tempf2;
                    return tempf1 <= (circ1.radius * circ1.radius);
                }
            }
            return false;
        }
        public static float CollisionAmount(Collider collider1, Collider collider2)
        {
            if (collider1.GetType() == typeof(RectangleCollider) && collider2.GetType() == typeof(RectangleCollider))
            {
                RectangleCollider c1 = (RectangleCollider)collider1;
                RectangleCollider c2 = (RectangleCollider)collider2;
                float intersectX = Math.Max(c1.Position.X, c2.Position.X);
                float intersectY = Math.Max(c1.Position.Y, c2.Position.Y);
                float intersectWidth = Math.Min(c1.Position.X + c1.Width, c2.Position.X + c2.Width) - intersectX;
                float intersectHeight = Math.Min(c1.Position.Y + c1.Height, c2.Position.Y + c2.Height) - intersectY;

                // Check if there's a valid intersection
                if (intersectWidth > 0 && intersectHeight > 0)
                {
                    // Calculate areas
                    float intersectionArea = intersectWidth * intersectHeight;
                    float rect1Area = c1.Width * c1.Height;

                    // Calculate percentage of overlap relative to each rectangle
                    float overlapPercentageRect1 = intersectionArea / rect1Area * 100;
                    return overlapPercentageRect1;
                }
            }
            return 0;
        }
        public static bool RaycastCollidesTarget(Collider origin, Collider destination, float maxDistance, int layer, bool includeTriggers)
        {
            float rayLength = (destination.transform.Position - origin.transform.Position).Length();
            if (rayLength > maxDistance) return false;
            Vector2 direction = Vector2.Normalize(destination.transform.Position - origin.transform.Position);
            return Raycast(new Ray2D(origin.transform.Position, direction, rayLength, layer), includeTriggers) == destination;
        }
        public static Collider Raycast(Vector2 origin, Vector2 destination, int layer, bool includeTriggers)
        {
            Vector2 direction = Vector2.Normalize(destination - origin);
            float rayLength = (destination - origin).Length();
            return Raycast(new Ray2D(origin, direction, rayLength, layer), includeTriggers);
        }
        public static Collider Raycast(Ray2D ray, bool includeTriggers)
        {
            try
            {
                int areaX = (int)ray.origin.X / ChunkSize;
                int areaY = (int)ray.origin.Y / ChunkSize;
                List<Collider> area = Colliders[areaX][areaY];
                Collider closestCollider = null;
                float closestDistance = ray.maxDistance;
                int iterations = 10;
                int prevArea = -1;
                do
                {
                    iterations--;
                    foreach (var item in area)
                    {
                        if (RayIntersectsCollider(ray, item, includeTriggers, out float distance))
                        {
                            if (distance < closestDistance)
                            {
                                closestCollider = item;
                                closestDistance = distance;
                            }
                        }
                    }
                    if (closestCollider)
                    {
                        return closestCollider;
                    }
                    if (prevArea != 1 && RayIntersectsRectangle(ray, new Rectangle((areaX) * ChunkSize, (areaY - 1) * ChunkSize, ChunkSize, ChunkSize)))
                    {
                        areaY--;
                        prevArea = 0;
                    }
                    else if (prevArea != 0 && RayIntersectsRectangle(ray, new Rectangle((areaX) * ChunkSize, (areaY + 1) * ChunkSize, ChunkSize, ChunkSize)))
                    {
                        areaY++;
                        prevArea = 1;
                    }
                    else if (prevArea != 3 && RayIntersectsRectangle(ray, new Rectangle((areaX - 1) * ChunkSize, (areaY) * ChunkSize, ChunkSize, ChunkSize)))
                    {
                        areaX--;
                        prevArea = 2;
                    }
                    else if (prevArea != 2 && RayIntersectsRectangle(ray, new Rectangle((areaX + 1) * ChunkSize, (areaY) * ChunkSize, ChunkSize, ChunkSize)))
                    {
                        areaX++;
                        prevArea = 3;
                    }
                    else
                    {
                        return null;
                    }
                    area = Colliders[areaX][areaY];
                }
                while (iterations > 0);
            }
            catch (Exception e)
            {
                //Debug.WriteLine(e);
            }
            return null;
        }
        //private static Collider RayIntersectsInsideArea(Ray2D ray, List<Collider> colliders)
        //{

        //}
        private static bool RayIntersectsRectangle(Ray2D ray, Rectangle rect)
        {
            float rectMinX = rect.X;
            float rectMaxX = rect.X + rect.Width;
            float rectMinY = rect.Y;
            float rectMaxY = rect.Y + rect.Height;

            float tMin = 0, tMax = ray.maxDistance;

            // Test against X slabs
            if (ray.direction.X != 0)
            {
                float tx1 = (rectMinX - ray.origin.X) / ray.direction.X;
                float tx2 = (rectMaxX - ray.origin.X) / ray.direction.X;

                tMin = Math.Max(tMin, Math.Min(tx1, tx2));
                tMax = Math.Min(tMax, Math.Max(tx1, tx2));
            }

            // Test against Y slabs
            if (ray.direction.Y != 0)
            {
                float ty1 = (rectMinY - ray.origin.Y) / ray.direction.Y;
                float ty2 = (rectMaxY - ray.origin.Y) / ray.direction.Y;

                tMin = Math.Max(tMin, Math.Min(ty1, ty2));
                tMax = Math.Min(tMax, Math.Max(ty1, ty2));
            }

            return tMin <= tMax && tMin <= ray.maxDistance && tMax >= 0;
        }
        private static bool RayIntersectsCollider(Ray2D ray, Collider rect, bool includeTriggers, out float distance)
        {
            distance = float.MaxValue;
            if (LayerCollision[ray.layer, rect.Layer] && rect.Enabled && (!rect.Trigger || includeTriggers))
            {
                // Define rectangle bounds
                float rectMinX = rect.Position.X;
                float rectMaxX = rect.Position.X + rect.Width;
                float rectMinY = rect.Position.Y;
                float rectMaxY = rect.Position.Y + rect.Height;

                float tMin = 0, tMax = float.MaxValue;

                // Test against X slabs
                if (ray.direction.X != 0)
                {
                    float tx1 = (rectMinX - ray.origin.X) / ray.direction.X;
                    float tx2 = (rectMaxX - ray.origin.X) / ray.direction.X;

                    tMin = Math.Max(tMin, Math.Min(tx1, tx2));
                    tMax = Math.Min(tMax, Math.Max(tx1, tx2));
                }

                // Test against Y slabs
                if (ray.direction.Y != 0)
                {
                    float ty1 = (rectMinY - ray.origin.Y) / ray.direction.Y;
                    float ty2 = (rectMaxY - ray.origin.Y) / ray.direction.Y;

                    tMin = Math.Max(tMin, Math.Min(ty1, ty2));
                    tMax = Math.Min(tMax, Math.Max(ty1, ty2));
                }

                // Check if the ray intersects
                if (tMin <= tMax && tMax >= 0)
                {
                    distance = tMin >= 0 ? tMin : tMax; // Use the nearest positive intersection
                    return true;
                }
            }
            return false;
        }
    }
}
