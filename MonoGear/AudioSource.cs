using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace MonoGear
{
    class AudioSource : WorldEntity
    {
        Dictionary<SoundEffectInstance, float[]> soundEffects;

        public AudioSource()
        {
            soundEffects = new Dictionary<SoundEffectInstance, float[]>();
            Visible = false;
        }

        public AudioSource(int x, int y)
        {
            Position = new Vector2(x, y);
            soundEffects = new Dictionary<SoundEffectInstance, float[]>();
            Visible = false;
        }

        public AudioSource(Vector2 position)
        {
            Position = position;
            soundEffects = new Dictionary<SoundEffectInstance, float[]>();
            Visible = false;
        }

        public AudioSource(Dictionary<SoundEffectInstance, float[]> soundEffects)
        {
            this.soundEffects = soundEffects;
            Visible = false;
        }

        public void AddSoundEffect(SoundEffect soundEffect, float maxDistance, float volume = 1)
        {
            var effect = soundEffect.CreateInstance();
            effect.Volume = volume;
            soundEffects.Add(effect, new float[2] { maxDistance, volume });
        }

        public Dictionary<SoundEffectInstance, float[]> GetSoundEffect()
        {
            return soundEffects;
        }

        public void Pause()
        {
            foreach (var item in soundEffects)
            {
                item.Key.Pause();
            }
        }

        public void PlayAll()
        {
            foreach (var item in soundEffects)
            {
                item.Key.Play();
            }
        }
    }
}
