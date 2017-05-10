using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    class AudioManager
    {
        private List<SoundSource> soundSources;

        public AudioManager() { }

        public void AddSoundSource(SoundSource soundSource)
        {
            soundSources.Add(soundSource);
        }
    }
}
