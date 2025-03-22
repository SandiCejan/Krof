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
        protected float Speed = 3;
        protected float WalkSpeed = 3;
        protected float RunSpeed = 6;
        protected Vector2 actualMove;
        Vector2 prevPos;
        public void Run()
        {
            Speed = RunSpeed;
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
        }
        internal override void OnDestroy()
        {
            AIEngine.Actors.Remove(this);
        }

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
