using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace MonoGear
{
    class AudioSource : WorldEntity
    {
        private List<SoundEffectInstance> soundEffects;

        public AudioSource()
        {
            Visible = false;
        }

        public AudioSource(List<SoundEffectInstance> soundEffects)
        {
            this.soundEffects = soundEffects;
            Visible = false;
        }

        public void AddSoundEffect(SoundEffect soundEffect)
        {
            soundEffects.Add(soundEffect.CreateInstance());
        }

        public void PlaySoundEffects()
        {
            foreach (var audio in soundEffects)
            {
                audio.Play();
            }
        }

        public void StopSoundEffects()
        {
            foreach (var audio in soundEffects)
            {
                audio.Stop();
            }
        }

        public void ChangeVolume(float volume)
        {
            foreach (var audio in soundEffects)
            {
                audio.Volume = volume;
            }
        }
    }
}
