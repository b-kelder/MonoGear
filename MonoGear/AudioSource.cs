using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            foreach(var audio in soundEffects)
            {
                audio.Play();
            }
        }
    }
}
