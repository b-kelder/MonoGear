using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using MonoGear.Entities;
using System;

namespace MonoGear.Engine.Audio
{
    public static class AudioManager
    {
        static List<PositionalAudio> positionalAudio = new List<PositionalAudio>();
        static List<SoundEffectInstance> globalSounds = new List<SoundEffectInstance>();

        public static void PlayGlobal(SoundEffectInstance instance)
        {
            globalSounds.Add(instance);
            instance.Play();
        }

        public static void StopGlobal(SoundEffectInstance instance)
        {
            if(globalSounds.Contains(instance))
            {
                globalSounds.Remove(instance);
            }
            instance.Stop();
        }

        public static void PlayGlobalOnce(SoundEffectInstance instance)
        {
            instance.IsLooped = false;
            instance.Play();
        }

        public static PositionalAudio AddPositionalAudio(SoundEffect soundEffect, float volume, float range, Vector2 position, bool loop)
        {
            PositionalAudio posAudio = new PositionalAudio();
            posAudio.SoundEffect = soundEffect.CreateInstance();
            posAudio.Volume = volume;
            posAudio.Range = range;
            posAudio.Position = position;
            posAudio.SoundEffect.IsLooped = loop;

            positionalAudio.Add(posAudio);
            return posAudio;
        }

        public static void UpdatePositionalAudio(Player player)
        {
            foreach (var audio in positionalAudio)
            {
                float distance = Vector2.Distance(audio.Position, player.Position);
                if (distance > audio.Range)
                {
                    //Stop playing the audio.
                    if (audio.SoundEffect.State != SoundState.Stopped)
                    {
                        audio.SoundEffect.Stop();
                    }
                }
                else
                {
                    //Start playing the audio if it's not playing already.
                    if (audio.SoundEffect.State != SoundState.Playing)
                    {
                        audio.SoundEffect.Play();
                    }
                    //Calculate the volume based on distance.
                    audio.SoundEffect.Volume = audio.Volume * (1 - (distance / audio.Range));
                }
            }
        }

        public static void ClearPositionalAudio()
        {
            foreach(var audio in positionalAudio)
            {
                if(audio.SoundEffect.State != SoundState.Stopped)
                {
                    audio.SoundEffect.Stop();
                }
            }

            positionalAudio.Clear();
        }

        public static void ClearGlobalAudio()
        {
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
        public SoundEffectInstance SoundEffect { get; set; }
        public float Volume { get; set; }
        public float Range { get; set; }
        public Vector2 Position { get; set; }
        public bool Loop { get; set; }
    }
}
