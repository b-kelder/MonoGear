using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    class AudioManager
    {
        private List<AudioSource> soundSources;

        public AudioManager()
        {

        }

        public void AddSoundSource(AudioSource soundSource)
        {
            soundSources.Add(soundSource);
        }
    }
}
