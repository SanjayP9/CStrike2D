using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace cstrike2d
{
    public class AudioManager
    {
        public float MusicVolume { get; set; }
        public float UiVolume { get; set; }
        public float VoiceVolume { get; set; }
        public float SoundEffectVolume { get; set; }

        public float MasterVolume
        {
            get
            {
                return SoundEffect.MasterVolume;
            }
            set
            {
                SoundEffect.MasterVolume = value;
            }
        }

        private List<SoundContainer> sounds = new List<SoundContainer>(); 

        public AudioManager()
        {
            MusicVolume = 1.0f;
            UiVolume = 1.0f;
            VoiceVolume = 1.0f;
            SoundEffectVolume = 1.0f;
            MasterVolume = 0.2f;
            SoundEffect.DistanceScale = 100f;
        }

        /// <summary>
        /// Adds a sound to the list
        /// </summary>
        /// <param name="sound"></param>
        public void AddSound(SoundContainer sound)
        {
            sounds.Add(sound);
        }

        /// <summary>
        /// Plays a sound with positional effects
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="listenerPosition"></param>
        /// <param name="emitterPosition"></param>
        /// <param name="volume"></param>
        public void PlaySound(string identifier, float volume, Vector2 listenerPosition, Vector2 emitterPosition)
        {
            SoundContainer sound = sounds.Find(snd => snd.Identifier == identifier);

            if (sound != null)
            {
                sound.Play(volume, listenerPosition, emitterPosition);
            }
            else
            {
                throw new Exception("Sound \"" + identifier + "\" does not exist.");
            }
        }

        /// <summary>
        /// Plays a sound without any positional effects
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="volume"></param>
        public void PlaySound(string identifier, float volume)
        {
            SoundContainer sound = sounds.Find(snd => snd.Identifier == identifier);

            if (sound != null)
            {
                sound.Play(volume);
            }
            else
            {
                throw new Exception("Sound \"" + identifier + "\" does not exist.");
            }
        }

        /// <summary>
        /// Stops a sound
        /// </summary>
        /// <param name="identifier"></param>
        public void StopSound(string identifier)
        {
            SoundContainer sound = sounds.Find(snd => snd.Identifier == identifier);

            if (sound != null)
            {
                sound.Stop();
            }
            else
            {
                throw new Exception("Sound \"" + identifier + "\" does not exist.");
            }
        }

        /// <summary>
        /// Pauses a sound
        /// </summary>
        /// <param name="identifier"></param>
        public void PauseSound(string identifier)
        {
            SoundContainer sound = sounds.Find(snd => snd.Identifier == identifier);

            if (sound != null)
            {
                sound.Pause();
            }
            else
            {
                throw new Exception("Sound \"" + identifier + "\" does not exist.");
            }
        }

        /// <summary>
        /// Resumes a sound
        /// </summary>
        /// <param name="identifier"></param>
        public void ResumeSound(string identifier)
        {
            SoundContainer sound = sounds.Find(snd => snd.Identifier == identifier);

            if (sound != null)
            {
                sound.Resume();
            }
            else
            {
                throw new Exception("Sound \"" + identifier + "\" does not exist.");
            }
        }

        /// <summary>
        /// Stops all sounds
        /// </summary>
        public void StopAllSounds()
        {
            foreach (SoundContainer sound in sounds)
            {
                sound.Stop();
            }
        }
    }
}