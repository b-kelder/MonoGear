using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MonoGear
{
    class AudioManager
    {
        private List<AudioSource> audioSources;

        public AudioManager()
        {
            audioSources = new List<AudioSource>();
        }

        public void AddSoundSource(AudioSource soundSource)
        {
            audioSources.Add(soundSource);
        }

        public void DistanceToSource(Player player)
        {
            foreach (var audioSource in audioSources)
            {
                float distance = Vector2.Distance(new Vector2(audioSource.Position.X, audioSource.Position.Y), new Vector2(player.Position.X, player.Position.Y));

                foreach (var audio in audioSource.GetSoundEffect())
                {
                    if (distance > audio.Value)
                    {
                        audio.Key.Stop();
                    }
                    else
                    {
                        audio.Key.Play();
                        audio.Key.Volume = (1 - (distance / audio.Value));
                    }
                }
            }
        }
    }
}
