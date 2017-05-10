using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MonoGear
{
    class AudioManager
    {
        private List<AudioSource> soundSources;

        public AudioManager()
        {
            soundSources = new List<AudioSource>();
        }

        public void AddSoundSource(AudioSource soundSource)
        {
            soundSources.Add(soundSource);
        }

        public void DistanceToSource(Player player, float maxDistance)
        {
            foreach (var audio in soundSources)
            {
                float distance = Vector2.Distance(new Vector2(audio.Position.X, audio.Position.Y), new Vector2(player.Position.X, player.Position.Y));
                if (distance > maxDistance)
                {
                    audio.StopSoundEffects();
                }
                else
                {
                    audio.PlaySoundEffects();
                    audio.ChangeVolume(1 - (distance / maxDistance));
                }
            }
        }
    }
}
