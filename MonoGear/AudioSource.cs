using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace MonoGear
{
    class AudioSource : WorldEntity
    {
        Dictionary<SoundEffectInstance, int> soundEffects;

        public AudioSource()
        {
            soundEffects = new Dictionary<SoundEffectInstance, int>();
            Visible = false;
        }

        public AudioSource(int x, int y, int z)
        {
            Position = new Vector3(x, y, z);
            soundEffects = new Dictionary<SoundEffectInstance, int>();
            Visible = false;
        }
        public AudioSource(Vector3 position)
        {
            Position = position;
            soundEffects = new Dictionary<SoundEffectInstance, int>();
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
