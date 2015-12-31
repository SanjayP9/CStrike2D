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
        public string Identifier;

        public SoundContainer(string identifier, SoundEffect soundEffect)
        {
            Identifier = identifier;
            soundEffectInstance = soundEffect.CreateInstance();
        }

        public void PlaySound(SoundEffect soundEffect, Vector2 playerPos, Vector2 emiterPos)
        {
            //Convert the Vector2s to Vector3s
            Vector3 playerPosition = new Vector3(playerPos.X, 0, emiterPos.Y);
            Vector3 emiterPosition = new Vector3(emiterPos.X, 0, emiterPos.Y);

            SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();

            listener.Position = playerPosition;
            emitter.Position = emiterPosition;

            soundEffectInstance.Apply3D(listener, emitter);
            soundEffectInstance.Play();
        }
    }
}
