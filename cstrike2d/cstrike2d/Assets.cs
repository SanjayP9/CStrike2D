// Author: Mark Voong
// File Name: Assets.cs
// Project Name: Global Offensive
// Creation Date: Dec 23rd, 2015
// Modified Date: Jan 20th, 2016
// Description: Stores all assets required in the game and is globally accessible
using System;
using CStrike2DServer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    /// <summary>
    /// Contains all assets that are required in the game
    /// </summary>
    public class Assets
    {
        // Assets
        public Texture2D PixelTexture { get; private set; }     // Pixel texture
        public SpriteFont DefaultFont { get; private set; }     // Default Font

        public Texture2D CTMenuBackground { get; private set; } // Counter-Terrorist Menu Background
        public Texture2D TMenuBackground { get; private set; }  // Terrorist Menu Background

        public Texture2D CTTexture { get; private set; }

        public Texture2D TileSet { get; private set; }

        // Textures
        public Texture2D ParticleTexture { get; private set; }
        public Texture2D SmokeParticle { get; private set; }
        public Texture2D DebrisParticle { get; private set; }
        public Texture2D ShellTexture { get; private set; }

        public Effect BlurEffect { get; private set; }

        public Map MapData { get; private set; }


        /// <summary>
        /// Loads assets that are required at the start of the application (fonts, UI)
        /// </summary>
        private ContentManager coreContentLoader;

        /// <summary>
        /// Loads assets that are specific to a map (tilemaps, particles)
        /// </summary>
        private ContentManager mapContentLoader;

        /// <summary>
        /// Loads assets that are required in-game (weapons, playermodels, grenades
        /// </summary>
        private ContentManager gameContentLoader;

        public bool GameContentLoaded { get; private set; }

        private Texture2D[,] weaponTextures;

        public Assets(CStrike2D instance)
        {
            // Initialize Content Loaders
            coreContentLoader = new ContentManager(instance.Services);
            mapContentLoader = new ContentManager(instance.Services);
            gameContentLoader = new ContentManager(instance.Services);

            coreContentLoader.RootDirectory = "Content";
            mapContentLoader.RootDirectory = "Content";
            gameContentLoader.RootDirectory = "Content";

            GameContentLoaded = false;
        }

        /// <summary>
        /// Load core assets in this method
        /// </summary>
        public void LoadCoreContent(CStrike2D instance)
        {
            // Load Pixel texture
            PixelTexture = new Texture2D(instance.GraphicsDevice, 1, 1);
            PixelTexture.SetData(new [] {Color.White});

            DefaultFont = coreContentLoader.Load<SpriteFont>("font/defFont");
            CTMenuBackground = coreContentLoader.Load<Texture2D>("texture/bg/ctmenu");
            TMenuBackground = coreContentLoader.Load<Texture2D>("texture/bg/tmenu");
            CTTexture = coreContentLoader.Load<Texture2D>("texture/player/ct1");
            TileSet = coreContentLoader.Load<Texture2D>("texture/map/dustTileSet");

            instance.Model.AudioManager.AddSound(new SoundContainer("menuMusic", coreContentLoader.Load<SoundEffect>("sound/music/mainmenu")));
            instance.Model.AudioManager.AddSound(new SoundContainer("ak47shot", coreContentLoader.Load<SoundEffect>("sound/sfx/weapon/ak47")));
            instance.Model.AudioManager.AddSound(new SoundContainer("ak47shotdistant", coreContentLoader.Load<SoundEffect>("sound/sfx/weapon/ak47d")));


            instance.Model.AudioManager.AddSound(new SoundContainer("m4a1shot", coreContentLoader.Load<SoundEffect>("sound/sfx/weapon/m4a1")));

            
            instance.Model.AudioManager.AddSound(new SoundContainer("buttonclick", coreContentLoader.Load<SoundEffect>("sound/sfx/ui/buttonclick")));
            instance.Model.AudioManager.AddSound(new SoundContainer("awpshot", coreContentLoader.Load<SoundEffect>("sound/sfx/weapon/awp")));
            instance.Model.AudioManager.AddSound(new SoundContainer("flashbang1", coreContentLoader.Load<SoundEffect>("sound/sfx/weapon/flashbang_explode1")));
            instance.Model.AudioManager.AddSound(new SoundContainer("flashbang2", coreContentLoader.Load<SoundEffect>("sound/sfx/weapon/flashbang_explode2")));
            
            instance.Model.AudioManager.AddSound(new SoundContainer("bombdef", coreContentLoader.Load<SoundEffect>("sound/sfx/radio/bombdef")));
            instance.Model.AudioManager.AddSound(new SoundContainer("bombpl", coreContentLoader.Load<SoundEffect>("sound/sfx/radio/bombpl")));
            instance.Model.AudioManager.AddSound(new SoundContainer("ctwin", coreContentLoader.Load<SoundEffect>("sound/sfx/radio/ctwin")));
            instance.Model.AudioManager.AddSound(new SoundContainer("rounddraw", coreContentLoader.Load<SoundEffect>("sound/sfx/radio/rounddraw")));
            instance.Model.AudioManager.AddSound(new SoundContainer("terwin", coreContentLoader.Load<SoundEffect>("sound/sfx/radio/terwin")));

            BlurEffect = coreContentLoader.Load<Effect>("fx/blur");
        }

        /// <summary>
        /// Load map assets in this method
        /// </summary>
        public void LoadMapContent()
        {
            MapData = new Map();
            MapData.Load("Content/maps/de_cache.txt", this);
        }

        /// <summary>
        /// Load game assets in this method
        /// </summary>
        public void LoadGameContent()
        {
            string[] weaponNames = Enum.GetNames(typeof (WeaponData.Weapon));
            weaponTextures = new Texture2D[weaponNames.Length, 3];

            for (int i = 0; i < weaponNames.Length; i++)
            {
                if (weaponNames[i].Contains("Knife"))
                {
                    string filePath = "texture/weapon/" + weaponNames[i].ToLower() + "/" +
                                      weaponNames[i].ToLower();
                    weaponTextures[i, 0] = gameContentLoader.Load<Texture2D>(filePath);
                }
                else if (!weaponNames[i].Contains("None"))
                {
                    string filePath = "texture/weapon/" + weaponNames[i].ToLower() + "/" +
                                      weaponNames[i].ToLower();
                    weaponTextures[i, 0] = gameContentLoader.Load<Texture2D>(filePath);
                    weaponTextures[i, 1] = gameContentLoader.Load<Texture2D>(filePath + "_d");
                    weaponTextures[i, 2] = gameContentLoader.Load<Texture2D>(filePath + "_m");
                }
            }

            ParticleTexture = gameContentLoader.Load<Texture2D>("texture/Textures/particle");
            SmokeParticle = gameContentLoader.Load<Texture2D>("texture/Textures/newparticle");
            DebrisParticle = gameContentLoader.Load<Texture2D>("texture/Textures/rocks");
            ShellTexture = gameContentLoader.Load<Texture2D>("texture/Textures/bullet");

            GameContentLoaded = true;
        }

        /// <summary>
        /// Unloads any content that is associated with the map
        /// </summary>
        public void UnloadMapContent()
        {
            mapContentLoader.Unload();
        }

        /// <summary>
        /// Unloads any content that is associated with the game
        /// </summary>
        public void UnloadGameContent()
        {
            gameContentLoader.Unload();
        }

        /// <summary>
        /// Unloads all content from the pipeline, use only when the game
        /// is shutting down
        /// </summary>
        public void UnloadAll()
        {
            coreContentLoader.Unload();
            mapContentLoader.Unload();
            gameContentLoader.Unload();
        }

        public Texture2D GetWeaponTexture(WeaponData.Weapon weapon)
        {
            int index = Array.FindIndex((WeaponData.WeaponEnums), wepEnum => wepEnum == weapon);
            switch (weapon)
            {
                case WeaponData.Weapon.Knife:
                    return weaponTextures[index, 0];
                case WeaponData.Weapon.Awp:
                    return weaponTextures[index, 0];
                case WeaponData.Weapon.Ak47:
                    return weaponTextures[index, 0];
                case WeaponData.Weapon.M4A1:
                    return weaponTextures[index, 0];
                default:
                    throw new ArgumentOutOfRangeException("weapon", weapon, null);
            }
        }
    }
}