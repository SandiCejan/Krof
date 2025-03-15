using Microsoft.Xna.Framework;
using System;

namespace KrofEngine
{
    public class AIActor : GameObject, IUpdateable
    {
        public AIActor(Vector2 position, Vector2 scale) : base(position, scale)
        {
            AIEngine.Actors.Add(this);
            Static = false;
        }

        public bool Enabled { get { return Active; } }

        public int UpdateOrder { get { return UpdateOrder; } set { updateOrder = value; } }
        public int updateOrder = 0;
        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
        protected int moveDir = 0;
        //public Vector2 destination = default;
        protected float Speed = 3;
        protected float WalkSpeed = 3;
        protected float RunSpeed = 6;
        protected Vector2 actualMove;
        Vector2 prevPos;
        public void Run()
        {
            Speed = RunSpeed;
            //MoveAmount *= 1.5f;
            switch (moveDir)
            {
                case 0:
                    MoveAmount = new Vector2(0, -Speed);
                    break;
                case 1:
                    MoveAmount = new Vector2(Speed, 0);
                    break;
                case 2:
                    MoveAmount = new Vector2(0, Speed);
                    break;
                default:
                    MoveAmount = new Vector2(-Speed, 0);
                    break;
            }
        }
        public void Walk()
        {
            Speed = WalkSpeed;
            //MoveAmount *= .5f;
            switch (moveDir)
            {
                case 0:
                    MoveAmount = new Vector2(0, -Speed);
                    break;
                case 1:
                    MoveAmount = new Vector2(Speed, 0);
                    break;
                case 2:
                    MoveAmount = new Vector2(0, Speed);
                    break;
                default:
                    MoveAmount = new Vector2(-Speed, 0);
                    break;
            }
        }
        public virtual void Update(GameTime gameTime)
        {
            //actualMove = Transform.Position - prevPos;
            //prevPos = Transform.Position;

            //MoveAmount = destination - Transform.Position;
            //float length = MoveAmount.Length();
            //if (length < 3f)
            //{
            //    destination = default;
            //}
            //MoveAmount.Normalize();
            //MoveAmount *= Speed;
        }
        internal override void OnDestroy()
        {
            AIEngine.Actors.Remove(this);
        }


        //protected List<Vector2> _waypoints = new();
        //private List<SearchNode> _fringe = new();
        //protected Dictionary<Point, int> _expandedPoints = new();

        //public List<Vector2> Waypoints => _waypoints;


        //public void GoTo(Vector2 theTarget)
        //{
        //    _waypoints.Clear();
        //    ArrayList gridCoordinatesToGoal = FindPathTo(AIEngine.GetCoordinates(theTarget));
        //    foreach (Point gridCoordinate in gridCoordinatesToGoal)
        //    {
        //        _waypoints.Add(new Vector2(gridCoordinate.X * 100 + 50f, gridCoordinate.Y * 100 + 50f));
        //    }

        //    destination = default;
        //}
        //public ArrayList FindPathTo(Point goal)
        //{
        //    ArrayList result = new ArrayList();
        //    _fringe.Clear();
        //    _expandedPoints.Clear();

        //    // Check if goal is Obstacle
        //    //foreach (var itemAtGoal in _scene.GetItemsAt(goal))
        //    //{
        //    //    if (itemAtGoal is Obstacle)
        //    //    {
        //    //        return result;
        //    //    }
        //    //}

        //    // Add initial node from which the search starts.
        //    SearchNode initialNode = SearchNode.Node();
        //    initialNode.Point = AIEngine.GetCoordinates(Transform.Position);
        //    _fringe.Add(initialNode);
        //    while (true)
        //    {
        //        // If fringe is empty we're in a dead end with nowhere to go. Fail!
        //        if (_fringe.Count == 0)
        //        {
        //            return result;
        //        }

        //        // Get the node with smallest cost.
        //        _fringe.Sort((x, y) => -x.TotalCost.CompareTo(y.TotalCost)); // descending or ascending
        //        int lastIdx = _fringe.Count - 1;
        //        SearchNode node = _fringe[lastIdx];
        //        _fringe.RemoveAt(lastIdx);

        //        // If this is the goal state return with success.
        //        if (node.Point == goal)
        //        {
        //            while (true)
        //            {
        //                if (node.Parent is null)
        //                {
        //                    return result;
        //                }
        //                else
        //                {
        //                    result.Add(node.Point);
        //                    node = node.Parent;
        //                }
        //            }
        //        }

        //        // Not a goal state - expand the node and add the successors to the list.
        //        _fringe.AddRange(ExpandGoal(node, goal));
        //    }
        //}

        //public List<SearchNode> ExpandGoal(SearchNode node, Point goal)
        //{
        //    List<SearchNode> successors = new();
        //    foreach (Point point in NeighboursOf(node.Point))
        //    {
        //        int newCost = node.RealCost + 1;

        //        if (!_expandedPoints.ContainsKey(point) || newCost < _expandedPoints[point])
        //        {
        //            _expandedPoints[point] = newCost;
        //            SearchNode newNode = SearchNode.Node();
        //            newNode.Point = point;
        //            newNode.Parent = node;
        //            newNode.RealCost = newCost;
        //            newNode.HeuristicCost = Math.Abs(goal.X - point.X) + Math.Abs(goal.Y - point.Y);
        //            successors.Add(newNode);
        //        }
        //    }

        //    return successors;
        //}

        //public ArrayList NeighboursOf(Point point)
        //{
        //    ArrayList neighbours = new ArrayList();
        //    AddPointIfClearTo(new Point(point.X - 1, point.Y), neighbours);
        //    AddPointIfClearTo(new Point(point.X + 1, point.Y), neighbours);
        //    AddPointIfClearTo(new Point(point.X, point.Y - 1), neighbours);
        //    AddPointIfClearTo(new Point(point.X, point.Y + 1), neighbours);
        //    return neighbours;
        //}

        //public void AddPointIfClearTo(Point point, ArrayList array)
        //{
        //    if (!AIEngine.Grid[point.X, point.Y])
        //    {
        //        array.Add(point);
        //    }
        //}


        #region GlobalMovement
        protected void MoveGloballyUp()
        {
            SetMoveDir();
            if (moveDir != 0)
            {
                switch (moveDir)
                {
                    case 1:
                        MoveAmount.Y = -MoveAmount.X;
                        MoveAmount.X = 0;
                        break;
                    case 2:
                        MoveAmount.Y *= -1;
                        break;
                    case 3:
                        MoveAmount.Y = MoveAmount.X;
                        MoveAmount.X = 0;
                        break;
                    default:
                        break;
                }
                Transform.Angle = (float)Math.Atan2(MoveAmount.X, -MoveAmount.Y);
                moveDir = 0;
            }
        }
        protected void MoveGloballyRight()
        {
            SetMoveDir();
            if (moveDir != 1)
            {
                switch (moveDir)
                {
                    case 0:
                        MoveAmount.X = -MoveAmount.Y;
                        MoveAmount.Y = 0;
                        break;
                    case 2:
                        MoveAmount.X = MoveAmount.Y;
                        MoveAmount.Y = 0;
                        break;
                    case 3:
                        MoveAmount.X *= -1;
                        break;
                    default:
                        break;
                }
                Transform.Angle = (float)Math.Atan2(MoveAmount.X, -MoveAmount.Y);
                moveDir = 1;
            }
        }
        protected void MoveGloballyDown()
        {
            SetMoveDir();
            if (moveDir != 2)
            {
                switch (moveDir)
                {
                    case 0:
                        MoveAmount.Y *= -1;
                        break;
                    case 1:
                        MoveAmount.Y = MoveAmount.X;
                        MoveAmount.X = 0;
                        break;
                    case 3:
                        MoveAmount.Y = -MoveAmount.X;
                        MoveAmount.X = 0;
                        break;
                    default:
                        break;
                }
                Transform.Angle = (float)Math.Atan2(MoveAmount.X, -MoveAmount.Y);
                moveDir = 2;
            }
        }
        protected void MoveGloballyLeft()
        {
            SetMoveDir();
            if (moveDir != 3)
            {
                switch (moveDir)
                {
                    case 0:
                        MoveAmount.X = MoveAmount.Y;
                        MoveAmount.Y = 0;
                        break;
                    case 1:
                        MoveAmount.X *= -1;
                        break;
                    case 2:
                        MoveAmount.X = -MoveAmount.Y;
                        MoveAmount.Y = 0;
                        break;
                    default:
                        break;
                }
                Transform.Angle = (float)Math.Atan2(MoveAmount.X, -MoveAmount.Y);
                moveDir = 3;
            }
        }
        private void SetMoveDir()
        {
            if (Math.Abs(MoveAmount.X) > Math.Abs(MoveAmount.Y))
            {
                if (MoveAmount.X > 0)
                {
                    moveDir = 1;
                }
                else
                {
                    moveDir = 3;
                }
            }
            else
            {
                if (MoveAmount.Y > 0)
                {
                    moveDir = 2;
                }
                else
                {
                    moveDir = 0;
                }
            }
        }
        #endregion
        #region Movement
        protected void MoveLeft()
        {
            if (MoveAmount.X == 0)
            {
                MoveAmount.X = MoveAmount.Y;
                MoveAmount.Y = 0;
            }
            else
            {
                MoveAmount.Y = MoveAmount.X;
                MoveAmount.X = 0;
            }
            Transform.Angle = (float)Math.Atan2(MoveAmount.X, -MoveAmount.Y);
            moveDir = (moveDir + 3) % 4;
        }
        protected void MoveRight()
        {
            if (MoveAmount.X == 0)
            {
                MoveAmount.X = MoveAmount.Y * -1;
                MoveAmount.Y = 0;
            }
            else
            {
                MoveAmount.Y = MoveAmount.X * -1;
                MoveAmount.X = 0;
            }
            Transform.Angle = (float)Math.Atan2(MoveAmount.X, -MoveAmount.Y);
            moveDir = (moveDir + 1) % 4;
        }
        protected void MoveBack()
        {
            MoveAmount *= -1;
            Transform.Angle = (float)Math.Atan2(MoveAmount.X, -MoveAmount.Y);
            moveDir = (moveDir + 2) % 4;
        }
        #endregion
    }
}
