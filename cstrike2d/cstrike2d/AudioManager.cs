using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cstrike2d
{
    public class AudioManager
    {
        public float MasterVolume { get; set; }
        public float MusicVolume { get; set; }
        public float UiVolume { get; set; }
        public float VoiceVolume { get; set; }
        public float SoundEffectVolume { get; set; }

        private List<Sound> sounds = new List<Sound>(); 

        public AudioManager()
        {
            
        }

        public void AddSound(Sound sound)
        {
            sounds.Add(sound);
        }

        public void PlaySound(string identifier, float volume)
        {
            
        }

        public void StopSound(string identifier)
        {
            
        }

        public void PauseSound()
        {
            
        }

        public void ResumeSound()
        {
            
        }

        public void StopAllSounds()
        {
            foreach (Sound sound in sounds)
            {

            }
        }
    }
}