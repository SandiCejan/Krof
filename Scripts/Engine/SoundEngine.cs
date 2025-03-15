using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Media;

namespace KrofEngine
{
    public class SoundEngine
    {
        private static List<SoundEffect> soundEffects = new();
        public static List<SoundEffectInstance> SoundEffectsInstances = new();
        public static List<SoundEffectInstance> SoundEffectsInstances2D = new();
        internal SoundEngine()
        {
            //SoundEffectsInstances.Add(Game1.Instance.Content.Load<Song>("ButtonPress"));
            //SoundEffectsInstances.Add(Game1.Instance.Content.Load<Song>("MenuMusic"));
            //SoundEffectsInstances.Add(Game1.Instance.Content.Load<Song>("GameMusic"));
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
            //SoundEffectsInstances.Add(Game1.Instance.Content.Load<SoundEffect>("audio/Explosion").CreateInstance());
            //SoundEffectsInstances.Add(Game1.Instance.Content.Load<SoundEffect>("audio/Walk").CreateInstance()); // 9
            //s = Game1.Instance.Content.Load<SoundEffect>("MenuMusic");
            //SoundEffectsInstances.Add(s.CreateInstance());
            //s = Game1.Instance.Content.Load<SoundEffect>("GameMusic");
            //SoundEffectsInstances.Add(s.CreateInstance());
            //soundEffects.Add(Game1.Instance.Content.Load<SoundEffect>("MonsterNormalWalk"));
            //soundEffects.Add(Game1.Instance.Content.Load<SoundEffect>("MonsterNormalRun"));
            //soundEffects.Add(Game1.Instance.Content.Load<SoundEffect>("MonsterShootWalk"));
            //soundEffects.Add(Game1.Instance.Content.Load<SoundEffect>("MonsterShootRun"));
            //soundEffects.Add(Game1.Instance.Content.Load<SoundEffect>("MonsterShootShoot"));
            //soundEffects.Add(Game1.Instance.Content.Load<SoundEffect>("MonsterBossWalk"));
            //soundEffects.Add(Game1.Instance.Content.Load<SoundEffect>("MonsterBossRun"));
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
        public static bool SetSoundForCamera(SoundEffectInstance sound, Vector2 position)
        {
            Vector2 screenDistance = (position - Camera.Position) / Game1.GameWidth/2;
            float fade = MathHelper.Clamp(2f - screenDistance.Length(), 0, 1);
            sound.Volume = fade * fade * GameManager.Settings.EffectsVolume;
            sound.Pan = MathHelper.Clamp(screenDistance.X, -1, 1);
            return fade > 0;
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
                }
            }
        }
        //private static AudioListener listener;
        //private static List<AudioEmitter> emitters = new();
        //public static AudioEmitter CreateAudioEmitter(Vector2 position)
        //{
        //    AudioEmitter emitter = new AudioEmitter();
        //    emitters.Add(emitter);
        //    emitter.Position = new Vector3(position.X, position.Y, 0);
        //    return emitter;
        //}
        //public static void RemoveAudioEmitter(AudioEmitter emitter)
        //{
        //    emitters.Remove(emitter);
        //}
        //public static AudioListener CreateAudioListener(Vector2 position)
        //{
        //    listener = new();
        //    listener.Position = new Vector3(position.X, position.Y, 0);
        //    return listener;
        //}
        //public static void RemoveAudioListener()
        //{
        //    listener = null;
        //}
        public static void Reset()
        {
            Listener = null;
            Emitters.Clear();
            StopAllSounds();
        }
        //internal void Update(GameTime gameTime)
        //{
        //    if (listener != null)
        //    {
        //        foreach (var item in emitters)
        //        {
        //            SoundEffectsInstances[9].Apply3D(listener, item);
        //        }
        //    }
        //}
    }
}
