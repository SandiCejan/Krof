using KrofEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using System;

namespace Krof
{
    public class Player : UpdatableGameObject
    {
        public static Player player;
        public float Speed = 300;
        //private Vector2 moveAmount;
        Vector2 movDir;
        DrawableItem drawableItem;
        int AnimFrame;
        AudioListener audioListener;
        public Player(Game game, Vector2 position) : base(position, new Vector2(.3f, .3f))
        {
            UpdateOrder = int.MinValue;
            drawableItem = (DrawableItem)AddComponent(new DrawableItem(Renderer.Sprites[0]));
            //transform.Scale = new Vector2(100, 100);
            //drawableUI = (DrawableItem)AddComponent(new DrawableItem(Renderer.Sprites[1]));
            player = this;
            AddComponent(new RectangleCollider(1, new Vector2(50, 50), new Vector2(-25, -25), true, 80, OnCollision, OnTrigger));
            Static = false;
            Renderer.drawableRectangleColliders.Add(collider);
            //Camera.Transform = Matrix.Identity;
            Camera.Follow(Transform);
            SoundEngine.Listener = Transform;
            GameManager.OnPause += OnStop;
            GameManager.OnPlay += OnPlay;
            //audioListener = SoundEngine.CreateAudioListener(Transform.Position);
            //transform.Position += new Vector2(.4f, .4f);
        }
        bool audioPaused;
        public void OnStop()
        {
            if (!play)
            {
                audioPaused = SoundEngine.SoundEffectsInstances[3].State == SoundState.Playing;
                Active = false;
                if (audioPaused)
                {
                    SoundEngine.StopSound(3);
                }
            }
        }
        public void OnPlay()
        {
            Active = true;
            if (audioPaused)
            {
                SoundEngine.PlaySound(3);
            }
        }
        float x = 0;
        bool moveAudioPlaying = false;
        public override void Update(GameTime gameTime)
        {
            if (ActualMove != Vector2.Zero)
            {
                if (!moveAudioPlaying)
                {
                    moveAudioPlaying = true;
                    SoundEngine.PlaySound(3);
                }
            }else if (moveAudioPlaying)
            {
                moveAudioPlaying = false;
                SoundEngine.PauseSound(3);
            }
            if (x == 0)
            {
                var keyboardState = GameManager.KeyboardState;
                movDir = Vector2.Zero;
                if (keyboardState.GetPressedKeyCount() > 0)
                {
                    if (keyboardState.IsKeyDown(Keys.A)) movDir.X--;
                    if (keyboardState.IsKeyDown(Keys.D)) movDir.X++;
                    if (keyboardState.IsKeyDown(Keys.W)) movDir.Y--;
                    if (keyboardState.IsKeyDown(Keys.S)) movDir.Y++;
                }
                if (movDir != Vector2.Zero)
                {
                    MoveAmount = Vector2.Normalize(movDir) * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //transform.Position += moveAmount;
                    Transform.Angle = (float)Math.Atan2(movDir.X, -movDir.Y);
                    AnimFrame++;
                    drawableItem.RectangleIndex = AnimFrame % 20 / 10 + 1;
                }
                else
                {
                    drawableItem.RectangleIndex = 0;
                    AnimFrame = 0;
                    MoveAmount = Vector2.Zero;
                }
                if (keyboardState.IsKeyDown(Keys.K)) PlaySceneManager.HasKey = true;
                if (keyboardState.IsKeyDown(Keys.N)) PlaySceneManager.Restart();
                if (keyboardState.IsKeyDown(Keys.U)) Doors.UnlockDoors();
                Camera.Follow(Transform);
            }
            else
            {
                x += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (x < 1.1)
                {
                    collider.Enabled = false;
                }
                else if (x < 3)
                {
                    MoveAmount = Vector2.Zero;
                    Transform.Angle = Doors.instance.Transform.Angle;
                }else
                {
                    switch (Doors.instance.Transform.Angle)
                    {
                        case 0:
                            MoveAmount = new Vector2(0, -3f);
                            break;
                        case (float)Math.PI:
                            MoveAmount = new Vector2(0, 3f);
                            break;
                        case (float)Math.PI / 2:
                            MoveAmount = new Vector2(3f, 0);
                            break;
                        default:
                            MoveAmount = new Vector2(-3f, 0);
                            break;
                    }
                    //player = null;
                    AnimFrame++;
                    drawableItem.RectangleIndex = AnimFrame % 20 / 10 + 1;
                    if (x > 6)
                    {
                        PlaySceneManager.OnComplete(true);
                        Destroy();
                    }
                }
            }
            //audioListener.Position = new Vector3(Transform.Position.X, Transform.Position.Y, 0);
        }
        public void Die()
        {
            //PlaySceneManager.Restart();
            //GameManager.PauseGame();
            //PlaySceneManager.UpdateScene = false;
            PlaySceneManager.OnComplete(false);
            Destroy();
        }
        int Health = 1;
        bool play = false;

        public void Damage(int amount)
        {
            Health -= amount;
            if (Health < 1)
            {
                Die();
            }
        }
        public void OnTrigger(Collider collisionObject)
        {
            Type t = collisionObject.gameObject.GetType();
            if (t == typeof(Key))
            {
                PlaySceneManager.HasKey = true;
                collisionObject.Enabled = false;
                collisionObject.gameObject.GetComponent<DrawableItem>().Enabled = false;
                SoundEngine.PlaySound(4);
            }
            else if (t == typeof(Doors))
            {
                if (PlaySceneManager.HasKey)
                {
                    play = true;
                    Doors.UnlockDoors();
                    PlaySceneManager.PauseOnly();
                    Physics.Active = true;
                    drawableItem.RectangleIndex = 0;
                    x++;
                }
            }
            else if (t == typeof(MonsterNormal) || t == typeof(MonsterBoss))
            {
                Damage(1);
                return;
            }
        }
        public void OnCollision(Collider collisionObject)
        {
            if (collisionObject.gameObject == null) return;
            //Debug.WriteLine(collisionObject.gameObject.GetType().ToString());
            //Debug.WriteLine(Physics.CollisionAmount(collider, collisionObject));
            Type t = collisionObject.gameObject.GetType();
            if (t == typeof(Wall))
            {
                if (Physics.CollisionAmount(collider, collisionObject) > 12)
                {
                    Damage(1);
                    return;
                }
                //Debug.WriteLine(x);
                //transform.Position -= moveAmount;
                //moveAmount = Vector2.Zero;
            }
            //transform.Position -= movDir;
        }
        internal override void OnDestroy()
        {
            player = null;
            SoundEngine.PauseSound(3);
            GameManager.OnPause -= OnStop;
            GameManager.OnPlay -= OnPlay;
            base.OnDestroy();
        }
    }
}
