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
        private SoundEffectInstance soundEffectInstance;
        public string Identifier { public get; private set; }

        public SoundContainer(string identifier, SoundEffect soundEffect)
        {
            Identifier = identifier;
            soundEffectInstance = soundEffect.CreateInstance();
        }

        public void Play(Vector2 listenerPos, Vector2 emiterPos)
        {
            //Convert the Vector2s to Vector3s
            Vector3 listenerPosition = new Vector3(listenerPos.X, 0f, emiterPos.Y);
            Vector3 emiterPosition = new Vector3(emiterPos.X, 0f, emiterPos.Y);

            listener.Position = listenerPosition;
            emitter.Position = emiterPosition;

            soundEffectInstance.Apply3D(listener, emitter);
            soundEffectInstance.Play();
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
