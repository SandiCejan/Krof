using KrofEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Krof
{
    internal class Explosive : UpdatableGameObject
    {
        float waitTime = 10;
        bool exploded = false;
        DrawableItem drawableItem;
        SoundEffectInstance soundEffectInstance;
        public Explosive(Vector2 position) : base(position)
        {
            drawableItem = (DrawableItem)AddComponent(new DrawableItem(Renderer.Sprites[10]));
            AddComponent(new RectangleCollider(2, new Vector2(20, 20), new Vector2(-10, -10), onTrigger: OnTrigger));
            collider.Trigger = true;
        }
        public override void Update(GameTime gameTime)
        {
            waitTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (waitTime < 0)
            {
                if (exploded)
                {
                    if (waitTime < -.5f)
                    {
                        Destroy();
                    }
                    else
                    {
                        drawableItem.RectangleIndex = (int)(waitTime * -10f) + 1;
                    }
                }
                else
                {
                    Explode();
                }
            }
        }
        public void OnTrigger(Collider collisionObject)
        {
            Explode();
        }
        internal override void OnDestroy()
        {
            if (soundEffectInstance != null)
            {
                SoundEngine.Emitters.Remove(soundEffectInstance);
                soundEffectInstance = null;
            }
            base.OnDestroy();
        }
        public void Explode()
        {
            waitTime = .01f;
            exploded = true;
            if (Player.player)
            {
                float vector = Vector2.Distance(Player.player.Transform.Position, Transform.Position);
                if (vector < 50)
                {
                    Player.player.Die();
                    SoundEngine.PlaySound(8);
                }
                else if (vector < 1300)
                {
                    soundEffectInstance = SoundEngine.CreateAudioEmitter(0, Transform);
                    soundEffectInstance.Play();
                }
            }
        }
    }
}
