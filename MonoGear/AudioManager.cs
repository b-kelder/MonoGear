using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace MonoGear
{
    static class AudioManager
    {
        private static List<AudioSource> audioSources = new List<AudioSource>();
        private static Song music;
        public static int settingsMusicVolume;

        #region Music controll

        /// <summary>
        /// Method to set the 'background' music.
        /// </summary>
        /// <param name="music">The music to set.</param>
        public static void MusicSet(Song newMusic)
        {
            music = newMusic;
            MediaPlayer.IsRepeating = true;
        }

        /// <summary>
        /// Method to start playing the 'background' music.
        /// </summary>
        public static void MusicPlay()
        {
            if (music != null && MediaPlayer.State != MediaState.Playing)
                MediaPlayer.Play(music);
        }

        /// <summary>
        /// Method to pause playing the 'background' music.
        /// </summary>
        public static void MusicPause()
        {
            if (MediaPlayer.State != MediaState.Paused)
                MediaPlayer.Pause();
        }

        /// <summary>
        /// Method to stop playing the 'background' music.
        /// </summary>
        public static void MusicStop()
        {
            if (MediaPlayer.State != MediaState.Stopped)
                MediaPlayer.Stop();
        }

        /// <summary>
        /// Method to change the volume of the 'background' music.
        /// </summary>
        public static void MusicVolume(float volume)
        {
            MediaPlayer.Volume = volume;
        }
        #endregion

        #region Simple sound playback

        public static void PlayOnce(SoundEffect audio, float volume)
        {
            SoundEffectInstance audioInstance = audio.CreateInstance();
            audioInstance.Volume = volume;
            audioInstance.Play();
            audioInstance.IsLooped = false;
        }

        public static void PlayOnce(SoundEffect audio, float volume, Vector2 location, int range)
        {
            AudioSource source = new AudioSource(location);
            source.AddSoundEffect(audio, range);
            audioSources.Add(source);
        }

        #endregion

        #region Global audio controll

        /// <summary>
        /// Method to start playing the global audio.
        /// </summary>
        public static void GlobalAudioPlay(SoundEffectInstance audio, bool loop = false, int volume = 1)
        {
            audio.Volume = volume;
            audio.IsLooped = loop;
            if (audio.State != SoundState.Playing)
                audio.Play();
        }

        /// <summary>
        /// Method to pause playing the global audio.
        /// </summary>
        public static void GlobalAudioPause(SoundEffectInstance audio)
        {
            if (audio.State != SoundState.Paused)
                audio.Pause();
        }

        /// <summary>
        /// Method to resume playing the global audio.
        /// </summary>
        public static void GlobalAudioResume(SoundEffectInstance audio)
        {
            if (audio.State == SoundState.Paused)
                audio.Resume();
        }

        /// <summary>
        /// Method to stop playing the global audio.
        /// </summary>
        public static void GlobalAudioStop(SoundEffectInstance audio)
        {
            if (audio.State != SoundState.Stopped)
                audio.Stop();
        }

        /// <summary>
        /// Method to change the volume of a global sound.
        /// </summary>
        /// <param name="audio">The audio the change the volume of.</param>
        /// <param name="volume">The volume to change to.</param>
        public static void GlobalAudioVolume(SoundEffectInstance audio , float volume)
        {
            audio.Volume = volume;
        }

        #endregion

        #region Soundsource audio controll

        /// <summary>
        /// Method to add a audio source to the audio manager.
        /// </summary>
        /// <param name="audioSource">The audio source to add.</param>
        public static void AddAudioSource(AudioSource audioSource)
        {
            audioSources.Add(audioSource);
        }

        /// <summary>
        /// Method to remove a audio source to the audio manager.
        /// </summary>
        /// <param name="audioSource">The audio source to remove.</param>
        public static void RemoveAudioSources(AudioSource audioSource)
        {
            audioSources.Remove(audioSource);
        }

        /// <summary>
        /// Method to update the state and volume of audio based on their location relative to the player.
        /// </summary>
        /// <param name="player">The player to base the calculation on.</param>
        public static void UpdateAudioSourceAudio(Player player)
        {
            foreach (var audioSource in audioSources)
            {
                float distance = Vector2.Distance(new Vector2(audioSource.Position.X, audioSource.Position.Y), new Vector2(player.Position.X, player.Position.Y));

                foreach (var audio in audioSource.GetSoundEffect())
                {
                    if (distance > audio.Value)
                    {
                        if(audio.Key.State  != SoundState.Stopped)
                            audio.Key.Stop();
                    }
                    else
                    {
                        if (audio.Key.State != SoundState.Playing)
                            audio.Key.Play();

                        audio.Key.Volume = (1 - (distance / audio.Value));
                    }
                }
            }
        }

        #endregion
    }
}
