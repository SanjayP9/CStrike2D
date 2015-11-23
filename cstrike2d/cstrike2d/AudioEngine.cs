// Author: Mark Voong
// File Name: AudioEngine.cs
// Project Name: LightEngine
// Creation Date: May 16th, 2015
// Modified Date: Sept 20th, 2015
// Description: Handles all sound output in the application

/* Notes
 * ---------------------
 * Version: 1.1
 * 
 * Changelog - Version Change:
 * Sept 20th, 2015 - 1.0 > 1.1
 * - Class will be rewritten for optimization and clarity. Certain methods will be removed or
 * - otherwise obsolete in Version 1.2.
 * - Methods that are slated for removal were tagged with the Obsolete attribute
 * - Added pausing functionality
 */

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Un4seen.Bass;
using System.Windows.Forms;
using Un4seen.Bass.AddOn.Flac;
using MessageBoxIcon = System.Windows.Forms.MessageBoxIcon;

namespace LightEngine
{
    /// <summary>
    /// Handles all audio processes including music and sound effects
    /// </summary>
    public class AudioEngine
    {
        private List<AudioContainer> sounds = new List<AudioContainer>();   // Stores all sound and music
        private const int BUFFER_SIZE = 2048;                               // FFT Sample Size, larger sizes are more CPU intensive but give more float values
                                                                             // per frequency range
                                                                             // 2048 (4096 bins) is the best resolution without heavily taxing the CPU
        private bool started;                                               // If Bass started, it is true
        private float globalVolume = 0.5f;                                  // Global Volume

        private bool throwExceptions = false;                               // Should this class throw exceptions?

        [Obsolete("Removed in the next version")]
        public AudioContainer MainStream = new AudioContainer(0, "none");

        [Obsolete("Removed in the next version")]
        public float MainAudioVolume = 0.5f;

        /// <summary>
        /// Sets the global volume as well as updates all sounds
        /// </summary>
        public float GlobalVolume
        {
            get { return globalVolume; }

            set
            {
                globalVolume = value;

                foreach (AudioContainer sound in sounds)
                {
                    Bass.BASS_ChannelSetAttribute(sound.Handle, BASSAttribute.BASS_ATTRIB_VOL, globalVolume);
                }
            }
        }

        /// <summary>
        /// Initialize audio engine
        /// </summary>
        /// <param name="instance">The instance of the game</param>
        public AudioEngine(Game instance)
        {
            // Initialize Bass.Net
            // Returning false means Bass did not start
            if (StartBassNet() == false)
            {
                // Display a warning to the user that Bass did not start and there is no sound
                DialogResult result = MessageBox.Show("Warning, pressing OK may result in no sound and/or unforeseen events. Press cancel to close the program",
                    "Bass failed to start", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                // If the user does not wish to continue, exit the game
                if (result == DialogResult.Cancel)
                {
                    instance.Exit();
                }
            }
        }

        /// <summary>
        /// Loading Bass.Net in another method, for ease of troubleshooting. Does all required file checks and 
        /// confirms if the API was able to start, returns true if the API starts correctly, false if it fails to locate
        /// a file or does not initialize
        /// </summary>
        private bool StartBassNet()
        {
            // Files that are required in order for Bass.Net to operate
            string[] requiredFiles = { @"bass.dll", @"Bass.Net.dll", @"bassflac.dll" };

            // Go through each file in the array of required files
            foreach (string file in requiredFiles)
            {
                // If the file does not exist
                if (!File.Exists(file))
                {
                    // Alert the user
                    MessageBox.Show("File \"" + file + "\" is missing", "File Not Found", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    // Bass failed to initialize, return false
                    started = false;
                    return false;
                }
            }

            // Register with Bass.Net to remove splash screen
            BassNet.Registration("devhalo@hotmail.com", "2X9232419312422");

            // If all the files were found, attempt to initialize Bass
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

            // If Bass failed to initialize, alert the user
            if (Bass.BASS_ErrorGetCode() != BASSError.BASS_OK)
            {
                MessageBox.Show("Bass Failed to Initialize", "Error Code: " + Bass.BASS_ErrorGetCode(),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Bass failed to initialize, return false
                started = false;
                return false;
            }

            // If all files were found and Bass successfully initializes, return true
            started = true;
            return true;
        }

        /// <summary>
        /// Creates an AudioContainer given a filename and
        /// an identifier
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="identifier"></param>
        public void LoadSound(string fileName, string identifier)
        {
            // TODO: METHOD WILL BE REWRITTEN AS OF V1.2

            int newSoundHandle = 0;     // Stream that the sound will be loaded in

            // Attempt to load sound into memory
            if (started)
            {
                newSoundHandle = Bass.BASS_StreamCreateFile(fileName, 0, 0, BASSFlag.BASS_DEFAULT);

                // If the file loaded successfully, it not newSoundHandle will be 0
                if (newSoundHandle != 0)
                {
                    sounds.Add(new AudioContainer(newSoundHandle, identifier));
                }
                else
                {
                    fileName = fileName.Replace(".mp3", ".flac");
                    // If the file was not a mp3, try loading it as a FLAC
                    newSoundHandle = BassFlac.BASS_FLAC_StreamCreateFile(fileName, 0, 0, BASSFlag.BASS_DEFAULT);

                    // If the song loaded sucesesfully, add it, else
                    // the file truly does not exist or is not supported
                    if (newSoundHandle != 0)
                    {
                        sounds.Add(new AudioContainer(newSoundHandle, identifier));
                    }
                    else
                    {
                        // Alert the user that the file was not located
                        MessageBox.Show("File: " + fileName + " could not be loaded, file is missing",
                            "File could not be located",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);

                        if (throwExceptions)
                        {
                            // The file could not be found, throw an exception
                            throw new FileNotFoundException("File Could Not Be Loaded: " + fileName + "\nError Code: " +
                                                            Bass.BASS_ErrorGetCode());
                        }
                    }
                }
            }
            else
            {
                // Alert the user that Bass did not start, therefore is unable to load any sounds
                MessageBox.Show("File: " + fileName + " could not be loaded, Bass did not initialize", "Bass failed to initialize",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (throwExceptions)
                {
                    // Bass did not start, throw an exception
                    throw new NullReferenceException("Attempted to load sound but Bass failed to initialize");
                }
            }
        }

        /// <summary>
        /// Creates an AudioContainer given a filename and an identifier
        /// </summary>
        /// <param name="fullFileName">File path including extension</param>
        /// <param name="identifier">String used to identify the sound later on</param>
        public AudioContainer LoadAndReturnSound(string fullFileName, string identifier)
        {
            // TODO: METHOD WILL BE REWRITTEN AS OF V1.2

            int newSoundHandle = 0;     // Stream that the sound will be loaded in

            // Attempt to load sound into memory
            if (started)
            {
                // Create a handle from the given file with default flags
                newSoundHandle = Bass.BASS_StreamCreateFile(fullFileName, 0, 0, BASSFlag.BASS_DEFAULT);

                // If the file loaded successfully, it not newSoundHandle will be 0
                if (newSoundHandle != 0)
                {
                    AudioContainer newSound = new AudioContainer(newSoundHandle, identifier);
                    sounds.Add(newSound);
                    return newSound;
                }

                newSoundHandle = BassFlac.BASS_FLAC_StreamCreateFile(fullFileName, 0, 0, BASSFlag.BASS_DEFAULT);

                if (newSoundHandle != 0)
                {
                    AudioContainer newSound = new AudioContainer(newSoundHandle, identifier);
                    sounds.Add(newSound);
                    return newSound;
                }

                // Alert the user that the file was not located
                MessageBox.Show("File: " + fullFileName + " could not be loaded, file is missing", "File could not be located",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (throwExceptions)
                {
                    // The file could not be found, throw an exception
                    throw new FileNotFoundException("File Could Not Be Loaded: " + fullFileName + "\nError Code: " +
                                                    Bass.BASS_ErrorGetCode());
                }
                return null;
            }

            // Alert the user that Bass did not start, therefore is unable to load any sounds
            MessageBox.Show("File: " + fullFileName + " could not be loaded, Bass did not initialize", "Bass failed to initialize",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

            if (throwExceptions)
            {
                // Bass did not start, throw an exception
                throw new NullReferenceException("Attempted to load sound but Bass failed to initialize");
            }
            return null;
        }

        /// <summary>
        /// Plays a sound effect when called
        /// </summary>
        /// <param name="identifier">Tag given for the sound file for easier access</param>
        public void Play(string identifier)
        {
            // Search for the requested sound in the list
            AudioContainer soundToPlay = sounds.Find(sound => sound.Identifier == identifier);

            // If the sound was properly loaded
            if (soundToPlay != null)
            {
                soundToPlay.Play();
                Bass.BASS_ChannelSetAttribute(soundToPlay.Handle, BASSAttribute.BASS_ATTRIB_VOL, globalVolume);
            }
            else
            {
                if (throwExceptions)
                {
                    // The sound doesn't exist, throw an exception
                    throw new NullReferenceException("Sound " + identifier + " does not exist");
                }
            }
        }

        /// <summary>
        /// Plays a sound effect when called without restarting
        /// </summary>
        /// <param name="identifier">Tag given for the sound file for easier access</param>
        public void PlayUnlooped(string identifier)
        {
            // Search for the requested sound in the list
            AudioContainer soundToPlay = sounds.Find(sound => sound.Identifier == identifier);

            // If the sound was properly loaded
            if (soundToPlay != null)
            {
                soundToPlay.PlayUnlooped();
                Bass.BASS_ChannelSetAttribute(soundToPlay.Handle, BASSAttribute.BASS_ATTRIB_VOL, globalVolume);
            }
            else
            {
                if (throwExceptions)
                {
                    // The sound doesn't exist, throw an exception
                    throw new NullReferenceException("Sound " + identifier + " does not exist");
                }
            }
        }

        /// <summary>
        /// Pauses a requested sound
        /// </summary>
        /// <param name="identifier"></param>
        public void Pause(string identifier)
        {
            // Find the container
            AudioContainer soundToPlay = sounds.Find(sound => sound.Identifier == identifier);

            // If the sound is valid and loaded into memory
            if (soundToPlay != null)
            {
                soundToPlay.Pause();
            }
            else
            {
                if (throwExceptions)
                {
                    // The sound doesn't exist, throw an exception
                    throw new NullReferenceException("Sound " + identifier + " does not exist");
                }
            }
        }

        /// <summary>
        /// Returns the FFT Data for a given stream
        /// </summary>
        /// <param name="identifier"></param>
        public float[] GetChannelData(string identifier)
        {
            // Stores FFT data 
            float[] sample = new float[BUFFER_SIZE];

            // Search for the sound in the list
            AudioContainer soundToPlay = sounds.Find(sound => sound.Identifier == identifier);

            // If the sound exists
            if (soundToPlay != null)
            {
                Bass.BASS_ChannelGetData(soundToPlay.Handle, sample, (int)BASSData.BASS_DATA_FFT4096);
            }

            if (throwExceptions)
            {
                // If the sound doesn't exist, throw an exception
                throw new NullReferenceException("Attempted to return FFT Data of non-existant stream: " + identifier);
            }

            return sample;
        }

        /// <summary>
        /// Returns the handle given the identifier of the sound
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public int GetAudioStream(string identifier)
        {
            // Search for the sound in the list
            AudioContainer soundToPlay = sounds.Find(sound => sound.Identifier == identifier);

            // If the sound exists
            if (soundToPlay != null)
            {
                return soundToPlay.Handle;
            }

            if (throwExceptions)
            {
                // If the sound doesn't exist, throw an exception
                throw new NullReferenceException("Attempted to return FFT Data of non-existant stream: " + identifier);
            }
            return 0;
        }

        /// <summary>
        /// Unloads a sound from memory
        /// </summary>
        /// <param name="identifier"></param>
        public void UnloadAudio(string identifier)
        {
            // Search for the sound in the list
            AudioContainer soundToPlay = sounds.Find(sound => sound.Identifier == identifier);

            // If the sound exists
            if (soundToPlay != null)
            {
                Bass.BASS_StreamFree(soundToPlay.Handle);
                sounds.Remove(soundToPlay);
            }

            if (throwExceptions)
            {
                // If the sound doesn't exist, throw an exception
                throw new NullReferenceException("Attempted to return FFT Data of non-existant stream: " + identifier);
            }
        }

        /// <summary>
        /// Changes the volume of the main sound
        /// </summary>
        /// <param name="change"></param>
        [Obsolete("Method will be removed in a later version")]
        public void ChangeMainVolume(float change)
        {
            MainAudioVolume += change;
            MainAudioVolume = MathHelper.Clamp(MainAudioVolume, 0f, 1f);
            Bass.BASS_ChannelSetAttribute(MainStream.Handle, BASSAttribute.BASS_ATTRIB_VOL, MainAudioVolume);
        }

        /// <summary>
        /// Sets the volume of the main sound
        /// </summary>
        /// <param name="change"></param>
        [Obsolete("Method will be removed in a later version")]
        public void SetMainVolume(float change)
        {
            MainAudioVolume = change;
            MainAudioVolume = MathHelper.Clamp(MainAudioVolume, 0f, 1f);
            Bass.BASS_ChannelSetAttribute(MainStream.Handle, BASSAttribute.BASS_ATTRIB_VOL, MainAudioVolume);
        }

        /// <summary>
        /// Returns the current state of a sound
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public BASSActive GetState(string identifier)
        {
            // Find the container
            AudioContainer soundToPlay = sounds.Find(sound => sound.Identifier == identifier);

            // If the sound is valid and loaded into memory
            if (soundToPlay != null)
            {
                // Return the state of the sound
                return Bass.BASS_ChannelIsActive(soundToPlay.Handle);
            }

            if (throwExceptions)
            {
                // The sound wasn't found, throw an exception
                throw new NullReferenceException("Sound " + identifier + " does not exist");
            }

            // In the event the sound does not exist, it is technically 
            // "not playing". Return the STOPPED state.
            return BASSActive.BASS_ACTIVE_STOPPED;
        }

        [Obsolete("Method will be removed in a later version")]
        public void PlayMainSong(string identifier)
        {
            AudioContainer soundToPlay = sounds.Find(sound => sound.Identifier == identifier);

            // If the sound was properly loaded
            if (soundToPlay != null)
            {
                soundToPlay.Play();
                Bass.BASS_ChannelStop(MainStream.Handle);
                MainStream = soundToPlay;
                Bass.BASS_ChannelSetAttribute(soundToPlay.Handle, BASSAttribute.BASS_ATTRIB_VOL, MainAudioVolume);
            }
            else
            {
                if (throwExceptions)
                {
                    // The sound doesn't exist, throw an exception
                    throw new NullReferenceException("Sound " + identifier + " does not exist");
                }
            }
        }
    }

    /// <summary>
    /// Stores a sound along with a tag for ease of access,
    /// Including a method that plays the sound
    /// </summary>
    public class AudioContainer
    {
        private readonly int sound;              // Stores the handle which can include sounds or music
        private readonly string identifier;      // Used to identify the sound, it is constant at run-time

        /// <summary>
        /// Returns the tag given for the
        /// sound effect
        /// </summary>
        public string Identifier
        {
            get { return identifier; }
        }

        /// <summary>
        /// Returns the stream associated to this container
        /// </summary>
        public int Handle
        {
            get { return sound; }
        }

        /// <summary>
        /// Creates a contained audio object with an identifier
        /// associated to a sound effect
        /// </summary>
        /// <param name="handle">The sound effect</param>
        /// <param name="identifier">Used to identify the sound for ease of use</param>
        public AudioContainer(int handle, string identifier)
        {
            sound = handle;
            this.identifier = identifier;
        }

        /// <summary>
        /// Plays the sound.
        /// </summary>
        public void Play()
        {
            Bass.BASS_ChannelPlay(sound, true);
        }

        /// <summary>
        /// Pauses the sound
        /// </summary>
        public void Pause()
        {
            Bass.BASS_ChannelPause(sound);
        }

        /// <summary>
        /// Plays the sound unlooped
        /// </summary>
        public void PlayUnlooped()
        {
            Bass.BASS_ChannelPlay(sound, false);
        }
    }
}