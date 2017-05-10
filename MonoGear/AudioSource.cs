using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace MonoGear
{
    class AudioSource : WorldEntity
    {
        Dictionary<SoundEffectInstance, int> soundEffects;

        public AudioSource()
        {
            Visible = false;
        }

        public AudioSource(Dictionary<SoundEffectInstance, int> soundEffects)
        {
            this.soundEffects = soundEffects;
            Visible = false;
        }

        public void AddSoundEffect(SoundEffect soundEffect, int maxDistance)
        {
            soundEffects.Add(soundEffect.CreateInstance(), maxDistance);
        }

        public Dictionary<SoundEffectInstance, int> GetSoundEffect()
        {
            return soundEffects;
        }
    }
}
