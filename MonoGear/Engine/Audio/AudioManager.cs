using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using MonoGear.Entities;
using System.Diagnostics;

namespace MonoGear.Engine.Audio
{
    public static class AudioManager
    {
        static List<PositionalAudio> positionalAudio = new List<PositionalAudio>();
        static List<SoundEffectInstance> globalSounds = new List<SoundEffectInstance>();

        /// <summary>
        /// Method that plays a global sound effect.
        /// </summary>
        /// <param name="instance">The instance of the sound effect that should be played</param>
        public static void PlayGlobal(SoundEffectInstance instance)
        {
            globalSounds.Add(instance);
            instance.Play();
        }

        /// <summary>
        /// Method that stops playing a global sound effect.
        /// </summary>
        /// <param name="instance">The instance of the sound effect that should stop playing</param>
        public static void StopGlobal(SoundEffectInstance instance)
        {
            if(globalSounds.Contains(instance))
            {
                globalSounds.Remove(instance);
            }
            instance.Stop();
        }

        /// <summary>
        /// Method that stops playing a positional sound effect.
        /// </summary>
        /// <param name="sound">The sound effect that should stop playing</param>
        public static void StopPositional(PositionalAudio sound)
        {
            if(positionalAudio.Contains(sound))
            {
                positionalAudio.Remove(sound);
            }
            sound.SoundEffect.Stop();
        }

        /// <summary>
        /// Method that plays a global sound effect only once.
        /// </summary>
        /// <param name="instance">The instance of the sound effect that should be played</param>
        public static void PlayGlobalOnce(SoundEffectInstance instance)
        {
            instance.IsLooped = false;
            instance.Play();
        }

        /// <summary>
        /// Method that starts playing a positional sound effect.
        /// </summary>
        /// <param name="sound">The sound effect that should be played</param>
        public static PositionalAudio AddPositionalAudio(SoundEffect soundEffect, float volume, float range, Vector2 position, bool loop)
        {
            PositionalAudio posAudio = new PositionalAudio();
            posAudio.SoundEffect = soundEffect.CreateInstance();
            posAudio.Volume = volume * SettingsPage.Volume * SettingsPage.EffectVolume;
            posAudio.Range = range;
            posAudio.Position = position;
            posAudio.SoundEffect.IsLooped = loop;

            posAudio.SoundEffect.Play();

            positionalAudio.Add(posAudio);
            return posAudio;
        }

        /// <summary>
        /// Method that updates the positional audio.
        /// </summary>
        /// <param name="player">Instance of the player</param>
        public static void UpdatePositionalAudio(Player player)
        {
            foreach (var audio in positionalAudio)
            {
                float distance = Vector2.Distance(audio.Position, player.Position);
                if (distance > audio.Range)
                {
                    audio.SoundEffect.Volume = 0;
                    
                }
                else
                {
                    //Calculate the volume based on distance.
                    audio.SoundEffect.Volume = audio.Volume * (1 - (distance / audio.Range));
                }
            }
        }

        /// <summary>
        /// Method that clears all positional audio.
        /// </summary>
        public static void ClearPositionalAudio()
        {
            // Loop through all positional audio
            foreach(var audio in positionalAudio)
            {
                if(audio.SoundEffect.State != SoundState.Stopped)
                {
                    audio.SoundEffect.Stop();
                }
            }

            positionalAudio.Clear();
        }

        // Method that clears all global audio.
        public static void ClearGlobalAudio()
        {
            Debug.WriteLine("Clearing global audio");
            // Loop through all global sounds
            foreach(var audio in globalSounds)
            {
                if(audio.State != SoundState.Stopped)
                {
                    audio.Stop();
                }
            }

            globalSounds.Clear();
        }
    }

    public class PositionalAudio
    {
        /// <summary>
        /// The instance of the sound effect.
        /// </summary>
        public SoundEffectInstance SoundEffect { get; set; }
        /// <summary>
        /// The volume of the sound effect.
        /// </summary>
        public float Volume { get; set; }
        /// <summary>
        /// The range of the sound effect.
        /// </summary>
        public float Range { get; set; }
        /// <summary>
        /// The position of the sound effect.
        /// </summary>
        public Vector2 Position { get; set; }
        /// <summary>
        /// Property that indicates of the sound effect should be looped.
        /// </summary>
        public bool Loop { get; set; }
    }
}
