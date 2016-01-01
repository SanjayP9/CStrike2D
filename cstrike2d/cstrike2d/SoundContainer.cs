using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cstrike2d
{
    public class SoundContainer
    {
        private AudioListener listener = new AudioListener();
        private AudioEmitter emitter = new AudioEmitter();
        private SoundEffect soundEffect;
        private SoundEffectInstance soundEffectInstance;
        public SoundState State { get { return soundEffectInstance.State; } }
        public string Identifier { get; private set; }
             

        public SoundContainer(string identifier, SoundEffect soundEffect)
        {
            Identifier = identifier;
            this.soundEffect = soundEffect;
            soundEffectInstance = soundEffect.CreateInstance();
        }

        public void Play(float volume, Vector2 listenerPos, Vector2 emiterPos)
        {
            // Convert the Vector2s to Vector3s
            Vector3 listenerPosition = new Vector3(listenerPos.X, 0f, listenerPos.Y);
            Vector3 emiterPosition = new Vector3(emiterPos.X, 0f, emiterPos.Y);

            // Create sound effect instance
            SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();

            // Set the sound effect volume
            soundEffectInstance.Volume = volume;

            // Set and apply the listener and emitter position for positional audio
            listener.Position = listenerPosition;
            emitter.Position = emiterPosition;
            soundEffectInstance.Apply3D(listener, emitter);

            // Play the sound effect
            soundEffectInstance.Play();
        }

        public void ChangeVolume(float volume)
        {
            soundEffectInstance.Volume = volume;
        }
        public void Play()
        {
            
            soundEffectInstance.Play();
        }
        public void Stop()
        {
            soundEffectInstance.Stop();
        }
        public void Pause()
        {
            soundEffectInstance.Pause();
        }
        public void Resume()
        {
            soundEffectInstance.Resume();
        }
    }
}