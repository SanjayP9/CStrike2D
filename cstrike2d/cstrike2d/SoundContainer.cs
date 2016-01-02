using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;

namespace CStrike2D
{
    public class SoundContainer
    {
        // Stores the audio listener and emitter
        private AudioListener listener = new AudioListener();
        private AudioEmitter emitter = new AudioEmitter();

        // Variables used to store the sound effect, instance, state, and identifier
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

        /// <summary>
        /// Plays the sound effect positionaly to the listener and emitter
        /// </summary>
        /// <param name="volume">the volume level to be played at</param>
        /// <param name="listenerPos">the listener position</param>
        /// <param name="emiterPos">the emitter position</param>
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

        /// <summary>
        /// Changes the volume of the sound effect
        /// </summary>
        /// <param name="volume">the volume level</param>
        public void ChangeVolume(float volume)
        {
            soundEffectInstance.Volume = volume;
        }

        /// <summary>
        /// Plays the sound effect
        /// </summary>
        public void Play()
        {
            soundEffectInstance.Play();
        }

        /// <summary>
        /// Stops playing the sound effect
        /// </summary>
        public void Stop()
        {
            soundEffectInstance.Stop();
        }

        /// <summary>
        /// Pauses the sound effect
        /// </summary>
        public void Pause()
        {
            soundEffectInstance.Pause();
        }
        
        /// <summary>
        /// Resumes playing the sound effect
        /// </summary>
        public void Resume()
        {
            soundEffectInstance.Resume();
        }
    }
}