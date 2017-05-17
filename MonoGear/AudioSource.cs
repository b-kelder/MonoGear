﻿using Microsoft.Xna.Framework;
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

        public AudioSource(int x, int y)
        {
            Position = new Vector2(x, y);
            soundEffects = new Dictionary<SoundEffectInstance, int>();
            Visible = false;
        }

        public AudioSource(Vector2 position)
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

        public void Pause()
        {
            foreach (var item in soundEffects)
            {
                item.Key.Pause();
            }
        }

        public void PlayAll()
        {
            foreach (var item in soundEffects)
            {
                item.Key.Play();
            }
        }
    }
}
