using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cstrike2d
{
    class Sound
    {
        private AudioEmitter emitter = new AudioEmitter();
        private AudioListener listener = new AudioListener();

        public SoundEffectInstance menuBgSoundInstance;
        public Sound(SoundEffect menuBgSound)
        {
            SoundEffectInstance menuBgSoundInstance = menuBgSound.CreateInstance();
        }

        public void PlaySound(SoundEffect soundEffect, Vector3 playerPos, Vector3 emiterPos)
        {
            SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();
            
            emitter.Position = emiterPos;
            listener.Position = playerPos;

            soundEffectInstance.Apply3D(listener, emitter);
            soundEffectInstance.Play();
        }

        // I dont need this i think
        public void PlaySound(SoundEffect soundEffect)
        {
            SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();
            soundEffectInstance.Play();
        }
    }
}
