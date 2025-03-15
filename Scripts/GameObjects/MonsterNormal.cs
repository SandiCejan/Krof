using KrofEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;

namespace Krof
{
    //public enum MonsterState
    //{
    //    NORMAL,
    //    RUNNING
    //}
    internal class MonsterNormal : AIActor
    {
        DrawableItem drawableItem;
        int AnimFrame;
        Collider[] colliders = new Collider[4];
        int[] collidersState;
        int colliderStateID = 0;
        int runningTime = 0;
        public Vector2 destination = default;
        bool lockMovement = false;
        bool caughtInWall = false;


        public MonsterNormal(Game game, Vector2 position) : base(position, new Vector2(.5f, .5f))
        {
            WalkSpeed = 2;
            RunSpeed = 3;
            collidersState = new int[] { 0, 0, 0, 0 };
            drawableItem = (DrawableItem)AddComponent(new DrawableItem(Renderer.Sprites[2]));
            MoveAmount = new Vector2(0, -2f);
            //upleft
            ((RectangleCollider)AddComponent(new RectangleCollider(3, new Vector2(3, 3), new Vector2(-49, -49), true, onTrigger: OnTriggerUpLeft))).Trigger = true;
            Renderer.drawableRectangleColliders.Add(collider);
            //upright
            ((RectangleCollider)AddComponent(new RectangleCollider(3, new Vector2(3, 3), new Vector2(46, -49), true, onTrigger: OnTriggerUpRight))).Trigger = true;
            Renderer.drawableRectangleColliders.Add(collider);
            //downleft
            ((RectangleCollider)AddComponent(new RectangleCollider(3, new Vector2(3, 3), new Vector2(-49, 46), true, onTrigger: OnTriggerDownLeft))).Trigger = true;
            Renderer.drawableRectangleColliders.Add(collider);
            //downright
            ((RectangleCollider)AddComponent(new RectangleCollider(3, new Vector2(3, 3), new Vector2(46, 46), true, onTrigger: OnTriggerDownRight))).Trigger = true;
            Renderer.drawableRectangleColliders.Add(collider);

            //up
            ((RectangleCollider)AddComponent(new RectangleCollider(3, new Vector2(92, 6), new Vector2(-46, -52), true, onTrigger: OnTriggerUp))).Trigger = true;
            Renderer.drawableRectangleColliders.Add(collider);
            colliders[0] = collider;
            //right
            ((RectangleCollider)AddComponent(new RectangleCollider(3, new Vector2(6, 92), new Vector2(46, -46), true, onTrigger: OnTriggerRight))).Trigger = true;
            colliders[1] = collider;
            Renderer.drawableRectangleColliders.Add(collider);
            //down
            ((RectangleCollider)AddComponent(new RectangleCollider(3, new Vector2(92, 6), new Vector2(-46, 46), true, onTrigger: OnTriggerDown))).Trigger = true;
            colliders[2] = collider;
            Renderer.drawableRectangleColliders.Add(collider);
            //left
            ((RectangleCollider)AddComponent(new RectangleCollider(3, new Vector2(6, 92), new Vector2(-52, -46), true, onTrigger: OnTriggerLeft))).Trigger = true;
            colliders[3] = collider;
            Renderer.drawableRectangleColliders.Add(collider);
            //main
            AddComponent(new RectangleCollider(3, new Vector2(92, 92), new Vector2(-46, -46), true, 80, OnCollision));
            //SoundEngine.SoundEffectsInstances[9].IsLooped = true;
            soundEffectWalk = SoundEngine.CreateAudioEmitter(1, Transform);
            soundEffectWalk.IsLooped = true;
            soundEffectWalk.Play();
            soundEffectRun = SoundEngine.CreateAudioEmitter(2, Transform);
            soundEffectRun.IsLooped = true;
            GameManager.OnPause += OnStop;
            GameManager.OnPlay += OnPlay;
            //audioEmitter = SoundEngine.CreateAudioEmitter(Transform.Position);
            //Renderer.drawableRectangleColliders.Add(collider);
        }
        SoundEffectInstance soundEffectWalk;
        SoundEffectInstance soundEffectRun;
        internal override void OnDestroy()
        {
            if (soundEffectWalk != null)
            {
                SoundEngine.Emitters.Remove(soundEffectWalk);
                soundEffectWalk.Stop();
                soundEffectWalk = null;
                SoundEngine.Emitters.Remove(soundEffectRun);
                soundEffectRun.Stop();
                soundEffectRun = null;
            }
            GameManager.OnPause -= OnStop;
            GameManager.OnPlay -= OnPlay;
            //SoundEngine.StopSound(9);
            base.OnDestroy();
        }
        public void OnStop()
        {
            soundEffectWalk.Stop();
            soundEffectRun.Stop();
        }
        public void OnPlay()
        {
            if (runningTime > 0)
            {
                soundEffectRun.Play();
            }
            else
            {
                soundEffectWalk.Play();
            }
        }
        public override void Update(GameTime gameTime)
        {
            //base.Update(gameTime);
            //SoundEngine.SetSoundForCamera(SoundEngine.SoundEffectsInstances[9], Transform.Position);
            AnimFrame += (int)ActualMove.Length();
            drawableItem.RectangleIndex = AnimFrame % 30 / 10;
            if (caughtInWall && !(Physics.ForceCheckCollissions(collider)?.Layer == 4))
            {
                caughtInWall = false;
            }
            //(Transform.Position - Player.player.Transform.Position).Length() < 2000 && 
            if (Physics.RaycastCollidesTarget(Player.player.collider, collider, 1000, 1, false))
            {
                destination = Player.player.Transform.Position;
                //GoTo(Player.player.Transform.Position);
                Renderer.dest = destination;
                //Debug.WriteLine("vidim" + Game1.FrameID);
                if (runningTime == 0)
                {
                    runningTime = 300;
                    //Speed = 1.5f;
                    //MoveAmount *= 1.5f;
                    soundEffectRun.Play();
                    soundEffectWalk.Stop();
                    Run();
                }
                if (ActualMove == Vector2.Zero)
                {
                    MoveToDestination2();
                }
                else
                {
                    MoveToDestination();
                }
            }
            else if (runningTime > 0)
            {
                //Debug.WriteLine(Physics.Raycast(Player.player.Transform.Position, Transform.Position, 1, false));
                if (destination != default)
                {
                    if (ActualMove == Vector2.Zero)
                    {
                        MoveToDestination2();
                    }
                    else
                    {
                        MoveToDestination();
                    }
                }
                else
                {
                    WanderAround();
                    runningTime--;
                    if (runningTime == 0)
                    {
                        //MoveAmount *= .5f;
                        soundEffectRun.Stop();
                        soundEffectWalk.Play();
                        Walk();
                        if (Physics.ForceCheckCollissions(collider)?.Layer == 4)
                        {
                            caughtInWall = true;
                        }
                        //Speed = 1f;
                    }
                }
            }
            else
            {
                WanderAround();
            }
            //if (ActualMove == Vector2.Zero)
            //{
            //    MoveLeft();
            //}
            colliderStateID++;
            //audioEmitter.Position = new Vector3(Transform.Position.X, Transform.Position.Y, 0);
        }
        private void MoveToDestination2()
        {
            if (destination == default)
            {
                MoveLeftOrRight();
                return;
            }
            Vector2 normalized = Vector2.Normalize(destination - Transform.Position);
            if (normalized.X > 0)
            {
                if (normalized.Y > 0)
                {
                    if (normalized.X > normalized.Y)
                    {
                        if (collidersState[2] == colliderStateID)
                        {
                            MoveGloballyRight();
                        }
                        else
                        {
                            MoveGloballyDown();
                        }
                    }
                    else
                    {
                        if (collidersState[1] == colliderStateID)
                        {
                            MoveGloballyDown();
                        }
                        else
                        {
                            MoveGloballyRight();
                        }
                    }
                }
                else
                {
                    if (normalized.X > -normalized.Y)
                    {
                        if (collidersState[0] == colliderStateID)
                        {
                            MoveGloballyRight();
                        }
                        else
                        {
                            MoveGloballyUp();
                        }
                    }
                    else
                    {
                        if (collidersState[1] == colliderStateID)
                        {
                            MoveGloballyUp();
                        }
                        else
                        {
                            MoveGloballyRight();
                        }
                    }
                }
            }
            else
            {
                if (normalized.Y > 0)
                {
                    if (-normalized.X > normalized.Y)
                    {
                        if (collidersState[2] == colliderStateID)
                        {
                            MoveGloballyLeft();
                        }
                        else
                        {
                            MoveGloballyDown();
                        }
                    }
                    else
                    {
                        if (collidersState[3] == colliderStateID)
                        {
                            MoveGloballyDown();
                        }
                        else
                        {
                            MoveGloballyLeft();
                        }
                    }
                }
                else
                {
                    if (-normalized.X > -normalized.Y)
                    {
                        if (collidersState[0] == colliderStateID)
                        {
                            MoveGloballyLeft();
                        }
                        else
                        {
                            MoveGloballyUp();
                        }
                    }
                    else
                    {
                        if (collidersState[3] == colliderStateID)
                        {
                            MoveGloballyUp();
                        }
                        else
                        {
                            MoveGloballyLeft();
                        }
                    }
                }
            }
            //if (Math.Abs(normalized.X) > Math.Abs(normalized.Y))
            //{
            //    if (normalized.Y > 0)
            //    {
            //        MoveGloballyDown();
            //    }
            //    else
            //    {
            //        MoveGloballyUp();
            //    }
            //}
            //else
            //{
            //    if (normalized.X > 0)
            //    {
            //        MoveGloballyRight();
            //    }
            //    else
            //    {
            //        MoveGloballyLeft();
            //    }
            //}
        }
        private void MoveToDestination()
        {
            if (destination == default)
            {
                MoveLeftOrRight();
                return;
            }
            if ((destination - Transform.Position).Length() < 50)
            {
                destination = default;
                Renderer.dest = default;
                lockMovement = true;
                //WanderAround();
            }
            Vector2 normalized = Vector2.Normalize(destination - Transform.Position);
            if (Math.Abs(normalized.X) > Math.Abs(normalized.Y))
            {
                if (normalized.X > 0)
                {
                    if (collidersState[1] >= colliderStateID) return;
                    MoveGloballyRight();
                }
                else
                {
                    if (collidersState[3] >= colliderStateID) return;
                    MoveGloballyLeft();
                }
            }
            else
            {
                if (normalized.Y > 0)
                {
                    if (collidersState[2] >= colliderStateID) return;
                    MoveGloballyDown();
                }
                else
                {
                    if (collidersState[0] >= colliderStateID) return;
                    MoveGloballyUp();
                }
            }
            cornerCollisions[0] = false;
            cornerCollisions[1] = false;
            cornerCollisions[2] = false;
            cornerCollisions[3] = false;


            //MoveAmount = destination - Transform.Position;
            //float length = MoveAmount.Length();
            //if (length > 0.01f)
            //{
            //    MoveAmount.Normalize();
            //}
            //else
            //{
            //    destination = default;
            //}
            //if (destination == default)
            //{
            //    if (_waypoints.Count > 0)
            //    {
            //        destination = _waypoints.Last();
            //        _waypoints.RemoveAt(_waypoints.Count - 1);
            //    }
            //    else
            //    {
            //        destination = default;
            //    }
            //}

        }
        private void MoveLeftOrRight()
        {
            int x = new Random().Next(2);
            if (x == 0)
            {
                if (collidersState[(moveDir + 3) % 4] != colliderStateID)
                {
                    MoveLeft();
                }
                else
                {
                    if (collidersState[(moveDir + 1) % 4] != colliderStateID)
                    {
                        MoveRight();
                    }
                    else
                    {
                        MoveBack();
                    }
                }
            }
            else
            {
                if (collidersState[(moveDir + 1) % 4] != colliderStateID)
                {
                    MoveRight();
                }
                else
                {
                    if (collidersState[(moveDir + 3) % 4] != colliderStateID)
                    {
                        MoveLeft();
                    }
                    else
                    {
                        MoveBack();
                    }
                }
            }
        }
        private void WanderAround()
        {
            if (lockMovement) return;
            int x = new Random().Next(5);
            if (collidersState[(moveDir + 3) % 4] != colliderStateID)
            {
                if (collidersState[(moveDir + 3) % 4] != -1)
                {
                    collidersState[(moveDir + 3) % 4] = -1;
                    if (x == 0)
                    {
                        MoveLeft();
                    }
                }
                else
                {
                    if (AnimFrame % (100 * Speed) == 0 && x == 0)
                    {
                        MoveLeft();
                    }
                }
            }
            if (collidersState[(moveDir + 1) % 4] != colliderStateID)
            {
                if (collidersState[(moveDir + 1) % 4] != -1)
                {
                    collidersState[(moveDir + 1) % 4] = -1;
                    if (x == 1)
                    {
                        MoveRight();
                    }
                }
                else
                {
                    if (AnimFrame % (100 * Speed) == 0 && x == 1)
                    {
                        MoveRight();
                    }
                }
            }
        }
        bool[] cornerCollisions = new bool[4];
        public void OnTriggerUpLeft(Collider collisionObject)
        {
            if (!(collisionObject.Layer == 4 && runningTime > 0))
            {
                cornerCollisions[0] = true;
            }
        }
        public void OnTriggerUpRight(Collider collisionObject)
        {
            if (!(collisionObject.Layer == 4 && runningTime > 0))
            {
                cornerCollisions[1] = true;
            }
        }
        public void OnTriggerDownRight(Collider collisionObject)
        {
            if (!(collisionObject.Layer == 4 && runningTime > 0))
            {
                cornerCollisions[2] = true;
            }
        }
        public void OnTriggerDownLeft(Collider collisionObject)
        {
            if (!(collisionObject.Layer == 4 && runningTime > 0))
            {
                cornerCollisions[3] = true;
            }
        }
        public void OnTriggerUp(Collider collisionObject)
        {
            if (!(collisionObject.Layer == 4 && runningTime > 0))
            {
                collidersState[0] = colliderStateID;
                if (Vector2.Normalize(MoveAmount) == new Vector2(0, -1))
                {
                    lockMovement = false;
                    if (runningTime > 0)
                    {
                        if (cornerCollisions[0])
                        {
                            if (cornerCollisions[1])
                            {
                                MoveToDestination2();
                            }
                            else
                            {
                                MoveGloballyRight();
                            }
                        }
                        else
                        {
                            if (cornerCollisions[1])
                            {
                                MoveGloballyLeft();
                            }
                            else
                            {
                                MoveToDestination();
                            }
                        }
                    }
                    else
                        OnTrigger();
                }
            }
        }
        public void OnTriggerDown(Collider collisionObject)
        {
            if (!(collisionObject.Layer == 4 && runningTime > 0))
            {
                collidersState[2] = colliderStateID;
                if (Vector2.Normalize(MoveAmount) == new Vector2(0, 1))
                {
                    lockMovement = false;
                    if (runningTime > 0)
                    {
                        if (cornerCollisions[2])
                        {
                            if (cornerCollisions[3])
                            {
                                MoveToDestination2();
                            }
                            else
                            {
                                MoveGloballyLeft();
                            }
                        }
                        else
                        {
                            if (cornerCollisions[3])
                            {
                                MoveGloballyRight();
                            }
                            else
                            {
                                MoveToDestination();
                            }
                        }
                    }
                    else
                        OnTrigger();
                }
            }
        }
        public void OnTriggerLeft(Collider collisionObject)
        {
            if (!(collisionObject.Layer == 4 && runningTime > 0))
            {
                collidersState[3] = colliderStateID;
                if (Vector2.Normalize(MoveAmount) == new Vector2(-1, 0))
                {
                    lockMovement = false;
                    if (runningTime > 0)
                    {
                        if (cornerCollisions[3])
                        {
                            if (cornerCollisions[0])
                            {
                                MoveToDestination2();
                            }
                            else
                            {
                                MoveGloballyUp();
                            }
                        }
                        else
                        {
                            if (cornerCollisions[0])
                            {
                                MoveGloballyDown();
                            }
                            else
                            {
                                MoveToDestination();
                            }
                        }
                    }
                    else
                        OnTrigger();
                }
            }
        }
        public void OnTriggerRight(Collider collisionObject)
        {
            if (!(collisionObject.Layer == 4 && runningTime > 0))
            {
                collidersState[1] = colliderStateID;
                if (Vector2.Normalize(MoveAmount) == new Vector2(1, 0))
                {
                    lockMovement = false;
                    if (runningTime > 0)
                    {
                        if (cornerCollisions[1])
                        {
                            if (cornerCollisions[2])
                            {
                                MoveToDestination2();
                            }
                            else
                            {
                                MoveGloballyDown();
                            }
                        }
                        else if (cornerCollisions[2])
                        {
                            MoveGloballyUp();
                        }
                        else
                        {
                            MoveToDestination();
                        }
                    }
                    else
                        OnTrigger();
                }
            }
        }
        public void OnTrigger()
        {
            //if (runningTime > 0)
            //{
            //    MoveToDestination();
            //    return;
            //}
            if (caughtInWall)
            {
                if (Physics.ForceCheckCollissions(colliders[moveDir])?.Layer != 4)
                {
                    if (!(Physics.ForceCheckCollissions(colliders[(moveDir + 3) % 4])?.Layer != 4))
                    {
                        MoveLeft();
                    }
                    else if (!(Physics.ForceCheckCollissions(colliders[(moveDir + 3) % 4])?.Layer != 4))
                    {
                        MoveRight();
                    }
                    else
                    {
                        MoveBack();
                    }
                }
            }
            else
            {
                int x = new Random().Next(2);
                if (x == 0)
                {
                    if (!Physics.ForceCheckCollissions(colliders[(moveDir + 3) % 4]))
                    {
                        MoveLeft();
                    }
                    else if (!Physics.ForceCheckCollissions(colliders[(moveDir + 1) % 4]))
                    {
                        MoveRight();
                    }
                    else
                    {
                        MoveBack();
                    }
                }
                else
                {
                    if (!Physics.ForceCheckCollissions(colliders[(moveDir + 1) % 4]))
                    {
                        MoveRight();
                    }
                    else if (!Physics.ForceCheckCollissions(colliders[(moveDir + 3) % 4]))
                    {
                        MoveLeft();
                    }
                    else
                    {
                        MoveBack();
                    }
                }

            }
        }
        void OnCollision(Collider other)
        {
            if (other.gameObject.GetType() == typeof(Wall) && Physics.CollisionAmount(collider, other) > 10)
            {
                Destroy();
            }
        }
    }
}
