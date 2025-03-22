using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace KrofEngine
{
    public class SoundEngine
    {
        private static List<SoundEffect> soundEffects = new();
        public static List<SoundEffectInstance> SoundEffectsInstances = new();
        public static List<SoundEffectInstance> SoundEffectsInstances2D = new();
        internal SoundEngine()
        {
            SoundEffectsInstances.Add(Game1.Instance.Content.Load<SoundEffect>("audio/ButtonPress").CreateInstance());
            SoundEffectsInstances.Add(Game1.Instance.Content.Load<SoundEffect>("audio/MenuMusic").CreateInstance());
            SoundEffectsInstances.Add(Game1.Instance.Content.Load<SoundEffect>("audio/GameMusic").CreateInstance());
            SoundEffectsInstances.Add(Game1.Instance.Content.Load<SoundEffect>("audio/Walk").CreateInstance());
            SoundEffectsInstances.Add(Game1.Instance.Content.Load<SoundEffect>("audio/KeyPickup").CreateInstance());
            SoundEffectsInstances.Add(Game1.Instance.Content.Load<SoundEffect>("audio/Doors").CreateInstance());
            SoundEffectsInstances.Add(Game1.Instance.Content.Load<SoundEffect>("audio/FinishSuccess").CreateInstance()); // 6
            SoundEffectsInstances.Add(Game1.Instance.Content.Load<SoundEffect>("audio/FinishFail").CreateInstance());
            soundEffects.Add(Game1.Instance.Content.Load<SoundEffect>("audio/Explosion"));
            soundEffects.Add(Game1.Instance.Content.Load<SoundEffect>("audio/MonsterNormalWalk"));
            soundEffects.Add(Game1.Instance.Content.Load<SoundEffect>("audio/MonsterNormalRun"));
            soundEffects.Add(Game1.Instance.Content.Load<SoundEffect>("audio/MonsterShootWalk"));
            soundEffects.Add(Game1.Instance.Content.Load<SoundEffect>("audio/MonsterShootShoot"));
            soundEffects.Add(Game1.Instance.Content.Load<SoundEffect>("audio/MonsterBossWalk"));
            soundEffects.Add(Game1.Instance.Content.Load<SoundEffect>("audio/MonsterBossRun")); // 6
            soundEffects.Add(Game1.Instance.Content.Load<SoundEffect>("audio/MonsterShootRun"));
        }

        public static void PlaySound(int soundID)
        {
            SoundEffectsInstances[soundID].Play();
        }
        public static void StopSound(int soundID)
        {
            SoundEffectsInstances[soundID].Stop();
        }
        public static void PauseSound(int soundID)
        {
            SoundEffectsInstances[soundID].Pause();
        }
        public static void ResumeSound(int soundID)
        {
            SoundEffectsInstances[soundID].Resume();
        }
        public static void StopAllSounds()
        {
            foreach (var item in SoundEffectsInstances)
            {
                item.Stop();
            }
        }
        public static Transform Listener;
        public static Dictionary<SoundEffectInstance, Transform> Emitters = new();
        public static SoundEffectInstance CreateAudioEmitter(int soundID, Transform transform)
        {
            SoundEffectInstance emitter = soundEffects[soundID].CreateInstance();
            emitter.Volume = 0;
            Emitters.Add(emitter, transform);
            return emitter;
        }
        internal void Update(GameTime gameTime)
        {
            if (Listener != null)
            {
                foreach (var item in Emitters)
                {
                    item.Key.Volume = GameManager.Settings.EffectsVolume*(1 - (Math.Clamp(Vector2.Distance(Listener.Position, item.Value.Position), 100, 700) - 100) / 600);
                    item.Key.Pan = MathHelper.Clamp((item.Value.Position.X - Listener.Position.X) / 700, -1, 1);
                }
            }
        }
        public static void Reset()
        {
            Listener = null;
            Emitters.Clear();
            StopAllSounds();
        }
    }
}
