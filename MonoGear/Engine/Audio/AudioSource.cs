using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace MonoGear.Engine.Audio
{
    class AudioSource : WorldEntity
    {
        Dictionary<SoundEffectInstance, float[]> soundEffects;

        /// <summary>
        /// Creates a new audio source instance.
        /// </summary>
        public AudioSource()
        {
            soundEffects = new Dictionary<SoundEffectInstance, float[]>();
            Visible = false;
        }

        /// <summary>
        /// Creates a new audio source instance at a given location.
        /// </summary>
        /// <param name="x">The x coördinate of the location.</param>
        /// <param name="y">The y coördinate of the location.</param>
        public AudioSource(int x, int y)
        {
            Position = new Vector2(x, y);
            soundEffects = new Dictionary<SoundEffectInstance, float[]>();
            Visible = false;
        }

        /// <summary>
        /// Creates a new audio source instance at a given location.
        /// </summary>
        /// <param name="position">Vector2 with the position.</param>
        public AudioSource(Vector2 position)
        {
            Position = position;
            soundEffects = new Dictionary<SoundEffectInstance, float[]>();
            Visible = false;
        }

        /// <summary>
        /// Creates a new audio source instance with the given sound effects.
        /// </summary>
        /// <param name="soundEffects">The sound effects that should be added to this audio source.</param>
        public AudioSource(Dictionary<SoundEffectInstance, float[]> soundEffects)
        {
            this.soundEffects = soundEffects;
            Visible = false;
        }

        /// <summary>
        /// Method to add a sound effect to this audio source.
        /// </summary>
        /// <param name="soundEffect">The sound effect that should be added.</param>
        /// <param name="maxDistance">The max distance of the sound effect.</param>
        /// <param name="volume">The volume of the sound effect.</param>
        public void AddSoundEffect(SoundEffect soundEffect, float maxDistance, float volume = 1)
        {
            var effect = soundEffect.CreateInstance();
            effect.Volume = volume;
            soundEffects.Add(effect, new float[2] { maxDistance, volume });
        }

        /// <summary>
        /// Method that returns a Dictionary with all sound effects which are a part of this audio source.
        /// </summary>
        /// <returns>Dictionary containing sound effects</returns>
        public Dictionary<SoundEffectInstance, float[]> GetSoundEffect()
        {
            return soundEffects;
        }

        /// <summary>
        /// Method that pauses all sound effects that are a part of this audio source.
        /// </summary>
        public void Pause()
        {
            foreach (var soundEffect in soundEffects)
            {
                soundEffect.Key.Pause();
            }
        }

        /// <summary>
        /// Method that plays all sound effects that are a part of this audio source.
        /// </summary>
        public void PlayAll()
        {
            foreach (var soundEffect in soundEffects)
            {
                soundEffect.Key.Play();
            }
        }
    }
}
