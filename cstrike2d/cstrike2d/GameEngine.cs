// Author: Mark Voong
// File Name: GameEngine.cs
// Project Name: Global Offensive
// Creation Date: Jan 4th, 2016
// Modified Date: Jan 21st, 2016
// Description: Handles all logic and drawing of the in-game components

using System;
using System.Collections.Generic;
using CStrike2DServer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CStrike2D
{
    public class GameEngine
    {
        private CStrike2D driver;           // Driver class instance
        private NetworkManager network;     // Networking
        private InputManager input;         // Input class instance
        private AudioManager audioManager;  // Audio
        private Assets assets;              // Assets
        private RayEmitter emitter;         // Used for visibility polygon drawing
        private RayCast raycaster = new RayCast();

        /// <summary>
        /// Contains all players connected on the server including the user
        /// </summary>
        public List<ClientPlayer> Players { get; private set; }

        /// <summary>
        /// The user's version of the player for easy access
        /// </summary>
        public ClientPlayer Client { get; private set; }

        private float prevRotation;     // The user's previous rotation used for reducing
                                        // input requests to the server

        private bool showMenu;              // Should the buy menu be shown
        private bool teamSelect = true;     // Should the team selection be shown
        private bool showScoreBoard;        // Should the scoreboard be shown

        /// <summary>
        /// Is the user flashed
        /// </summary>
        public bool Flashed { get; private set; }
        private float flashTimer;                   // Controls flash timing

        /// <summary>
        /// Current state of the game
        /// </summary>
        public GameEngineState CurState { get; set; }

        private float reloadTimer;                  // Reload timer
                
        private const int MAX_GRENADES = 1;         // Max number of grenades
        private const int MAX_FLASHBANGS = 2;       // Max number of flashbangs
        private const int MAX_SMOKE = 1;            // Max number of smokes
        private int numGrenades = 0;                // Number of grenades the user has
        private int numFlash = 0;                   // Number of flashbangs the user has
        private int numSmoke = 0;                   // Number of smokes the user has

        /// <summary>
        /// The current state of the engine
        /// </summary>
        public enum GameEngineState
        {
            InActive,
            Loaded,
            Active
        }

        private MenuState CurMenuState = MenuState.MainMenu;    // Current state of the menu

        /// <summary>
        /// Different buy menus and their states
        /// </summary>
        private enum MenuState
        {
            MainMenu,
            Pistols,
            Heavy,
            Smgs,
            Rifles,
            Gear,
            Grenades
        }

        /// <summary>
        /// Driver class for in-game logic
        /// </summary>
        /// <param name="driver"></param>
        public GameEngine(CStrike2D driver)
        {
            this.driver = driver;
            CurState = GameEngineState.InActive;
        }

        /// <summary>
        /// Initializes the game and sets up networking and driver classes
        /// </summary>
        /// <param name="networkManager"></param>
        /// <param name="audio"></param>
        /// <param name="input"></param>
        /// <param name="assets"></param>
        public void Initialize(NetworkManager networkManager, AudioManager audio, InputManager input, Assets assets)
        {
            if (CurState == GameEngineState.InActive)
            {
                network = networkManager;
                audioManager = audio;
                this.input = input;
                this.assets = assets;
                Players = new List<ClientPlayer>();
                CurState = GameEngineState.Loaded;
                emitter = new RayEmitter();
            }
        }

        /// <summary>
        /// Syncs the client's player instance with the server
        /// </summary>
        /// <param name="username"></param>
        /// <param name="identifier"></param>
        public void SyncClient(string username, short identifier)
        {
            ClientPlayer player = new ClientPlayer(username, identifier, assets);

            Client = player;
            Players.Add(Client);
        }

        /// <summary>
        /// Syncs other players that are connected to the server
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="username"></param>
        /// <param name="?"></param>
        public void SyncPlayer(short identifier, string username, byte team, float posX, float posY,
            float rot, byte weapon, byte state)
        {
            ClientPlayer player = Players.Find(ply => ply.Identifier == identifier);

            // If the player doesn't currently exist, set up the data and add it to the list
            if (player == null)
            {
                player = new ClientPlayer(username, identifier, driver.Assets);
                player.SetPosition(new Vector2(posX, posY));
                player.SetRotation(rot);
                player.SetCurrentWeapon(WeaponData.ByteToWeapon(weapon));
                player.SetTeam(team);
                player.SetState(state);
                Players.Add(player);
            }
        }

        /// <summary>
        /// Sets the weapon of a player. Used by the server
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="weapon"></param>
        public void SetWeapon(short identifier, byte weapon)
        {
            ClientPlayer player = Players.Find(ply => ply.Identifier == identifier);

            // Play pickup sound
            PlaySound(player, "pickup");

            // Sets the user's current weapon to the selected one
            player.SetCurrentWeapon(WeaponData.ByteToWeapon(weapon));
        }

        /// <summary>
        /// Syncs a new player that has just joined the server
        /// </summary>
        /// <param name="username"></param>
        /// <param name="identifier"></param>
        public void SyncNewPlayer(string username, short identifier)
        {
            ClientPlayer player = new ClientPlayer(username, identifier, assets);
            Players.Add(player);
        }

        /// <summary>
        /// Moves a player in a certain direction
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="direction"></param>
        public void MovePlayer(short identifier, byte direction)
        {
            ClientPlayer player = Players.Find(ply => ply.Identifier == identifier);
            player.Move(direction);
        }

        /// <summary>
        /// Changes a player's team
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="team"></param>
        public void ChangeTeam(short identifier, byte team)
        {
            ClientPlayer player = Players.Find(ply => ply.Identifier == identifier);
            player.SetTeam(team);
        }

        /// <summary>
        /// Fires a player's weapon
        /// </summary>
        /// <param name="identifier"></param>
        public void FireWeapon(short identifier)
        {
            ClientPlayer shooter = Players.Find(ply => ply.Identifier == identifier);

            // Play the shooting sound
            PlaySound(shooter);

                
            // If the player's weapon is a knife, don't draw the raycast
            if (shooter.CurrentWeapon.Weapon != WeaponData.Weapon.Knife)
            {
                shooter.Fire();
            }
        }

        /// <summary>
        /// Spawns a player at their current position
        /// </summary>
        /// <param name="identifier"></param>
        public void SpawnPlayer(short identifier)
        {
            ClientPlayer player = Players.Find(ply => ply.Identifier == identifier);
            player.Respawn(player.Position);
            PlaySound(player, "pickup");
        }

        /// <summary>
        /// Plays a gun sound at the location of the shooter
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="soundID"></param>
        public void PlaySound(ClientPlayer shooter)
        {
            // If the shooter exists
            if (shooter != null)
            {
                // Play the correct sound based on their weapon
                switch (shooter.CurrentWeapon.Weapon)
                {
                    case WeaponData.Weapon.Ak47:
                        audioManager.PlaySound("ak47shot", audioManager.SoundEffectVolume,
                            Client.Position, shooter.Position);
                        break;
                    case WeaponData.Weapon.M4A1:
                        audioManager.PlaySound("m4a1shot", audioManager.SoundEffectVolume,
                            Client.Position, shooter.Position);
                        break;
                }
            }
        }

        /// <summary>
        /// Plays a sound at the location of the player
        /// </summary>
        /// <param name="player"></param>
        /// <param name="sound"></param>
        public void PlaySound(ClientPlayer player, string sound)
        {
            audioManager.PlaySound(sound, audioManager.SoundEffectVolume, Client.Position,
                player.Position);
        }

        /// <summary>
        /// Plays a sound without positional effects
        /// </summary>
        /// <param name="player"></param>
        /// <param name="sound"></param>
        public void PlaySoundNonPos(ClientPlayer player, string sound)
        {
            audioManager.PlaySound(sound, audioManager.SoundEffectVolume);
        }

        /// <summary>
        /// Flashes the player and whites their screen
        /// </summary>
        public void FlashPlayer()
        {
            audioManager.PlaySound("flashbang1", audioManager.SoundEffectVolume, Client.Position,
                Client.Position);
            Flashed = true;
            flashTimer = 20f;
        }

        /// <summary>
        /// Damages a player
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="health"></param>
        /// <param name="armor"></param>
        public void Damage(short identifier, int health, int armor)
        {
            ClientPlayer player = Players.Find(ply => ply.Identifier == identifier);

            // Play sound if the player's health isn't the same (they were damaged)
            if (player.Health != health)
            {
                PlaySound(player, "hit2");
            }

            // If they died. Set their state to dead and play a sound
            if (health <= 0)
            {
                player.SetHealth(0);
                player.SetArmor(0);
                player.SetState(ServerClientInterface.DEAD);
                PlaySound(player, "death4");
            }
            else
            {
                player.SetHealth(health);
                player.SetArmor(armor);
            }
        }

        /// <summary>
        /// Main update loop for game logic
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(float gameTime)
        {
            // If the game is currently active
            if (CurState == GameEngineState.Active)
            {
                // If they have not selected a team, show the menu
                if (!teamSelect)
                {
                    byte dir = 0; // Direction they are moving

                    // If the client initialized and isn't a spectator 
                    if (Client != null && Client.CurrentTeam != ServerClientInterface.Team.Spectator)
                    {
                        // If the client isn't dead
                        if (Client.State != ServerClientInterface.PlayerState.Dead)
                        {
                            // If the pressed B
                            if (input.Tapped(Keys.B))
                            {
                                showMenu = !showMenu;

                                if (showMenu)
                                {
                                    driver.Model.InterfaceManager.ShowPage("buyMenu");
                                    driver.Model.InterfaceManager.ShowPage("buyButtonMenu");
                                }
                                else
                                {
                                    driver.Model.InterfaceManager.HideAll();
                                }
                            }

                            if (showMenu)
                            {
                                switch (CurMenuState)
                                {
                                    case MenuState.MainMenu:
                                        driver.Model.InterfaceManager.ShowPage("buyButtonMenu");
                                        if (input.Tapped(Keys.Escape))
                                        {
                                            showMenu = false;
                                        }
                                            // UNFINISHED
                                            /*
                                        else if (input.Tapped(Keys.D1) ||
                                                 driver.Model.InterfaceManager.Clicked(input, "buyButtonMenu",
                                                     "pistolMenuButton"))
                                        {
                                            if (Client.CurrentTeam == ServerClientInterface.Team.CounterTerrorist)
                                            {
                                                driver.Model.InterfaceManager.ShowPage("pistolMenuButton");
                                            }
                                        }
                                        else if (input.Tapped(Keys.D2) ||
                                                 driver.Model.InterfaceManager.Clicked(input, "buyButtonMenu",
                                                     "heavyMenuButton"))
                                        {
                                            if (Client.CurrentTeam == ServerClientInterface.Team.CounterTerrorist)
                                            {
                                            }
                                            else
                                            {

                                            }
                                        }
                                        else if (input.Tapped(Keys.D3) ||
                                                 driver.Model.InterfaceManager.Clicked(input, "buyButtonMenu",
                                                     "smgMenuButton"))
                                        {
                                            if (Client.CurrentTeam == ServerClientInterface.Team.CounterTerrorist)
                                            {
                                            }
                                            else
                                            {

                                            }
                                        }
                                             */
                                        else if (input.Tapped(Keys.D4) ||
                                                 driver.Model.InterfaceManager.Clicked(input, "buyButtonMenu",
                                                     "rifleMenuButton"))
                                        {
                                            driver.Model.InterfaceManager.HidePage("buyButtonMenu");
                                            CurMenuState = MenuState.Rifles;
                                        }
                                        break;
                                    case MenuState.Pistols:
                                        break;
                                    case MenuState.Heavy:
                                        break;
                                    case MenuState.Smgs:
                                        break;
                                    case MenuState.Rifles:

                                        if (input.Tapped(Keys.Escape))
                                        {
                                            driver.Model.InterfaceManager.HidePage("ctRifleButtonMenu");
                                            CurMenuState = MenuState.MainMenu;
                                        }


                                        if (Client.CurrentTeam == ServerClientInterface.Team.CounterTerrorist)
                                        {
                                            driver.Model.InterfaceManager.ShowPage("ctRifleButtonMenu");

                                            if (driver.Model.InterfaceManager.Clicked(input, "ctRifleButtonMenu",
                                                "m4a1MenuButton"))
                                            {
                                                network.BuyWeapon(WeaponData.Weapon.M4A1);
                                                driver.Model.InterfaceManager.HideAll();
                                                CurMenuState = MenuState.MainMenu;
                                                showMenu = false;
                                            }
                                        }
                                        else
                                        {
                                            driver.Model.InterfaceManager.ShowPage("tRifleButtonMenu");

                                            if (driver.Model.InterfaceManager.Clicked(input, "tRifleButtonMenu",
                                                "ak47MenuButton"))
                                            {
                                                network.BuyWeapon(WeaponData.Weapon.Ak47);
                                                driver.Model.InterfaceManager.HideAll();
                                                CurMenuState = MenuState.MainMenu;
                                                showMenu = false;
                                            }
                                        }
                                        break;
                                    case MenuState.Gear:
                                        break;
                                    case MenuState.Grenades:
                                        break;
                                }
                            }
                            else
                            {
                                // Switch weapons, UNFINISHED
                                /*
                                if (input.Tapped(Keys.D1))
                                {
                                }
                                else if (input.Tapped(Keys.D2))
                                {
                                }
                                else if (input.Tapped(Keys.D3))
                                {
                                }
                                 */

                                // Debug, press K to flash
                                /*
                                if (input.Tapped(Keys.K))
                                {
                                    network.Flash();
                                }
                                 */
                            }

                            // If the player is flashed, decrease the alpha value and timer
                            // The blur amount is relative to the flash timer
                            if (Flashed)
                            {
                                if (flashTimer >= 0f)
                                {
                                    flashTimer -= 0.15f;
                                    driver.Model.Shader.ChangeBlurAmount(flashTimer*3f);
                                }
                            }

                            // Directional movement
                            if (input.Tapped(Keys.W) || input.Held(Keys.W))
                            {
                                dir += 1;
                            }
                            else if (input.Tapped(Keys.S) || input.Held(Keys.S))
                            {
                                dir += 2;
                            }

                            if (input.Tapped(Keys.A) || input.Held(Keys.A))
                            {
                                dir += 4;
                            }
                            else if (input.Tapped(Keys.D) | input.Held(Keys.D))
                            {
                                dir += 8;
                            }

                            // Request the server to move their player
                            switch (dir)
                            {
                                case 1: // UP
                                    network.Move(ServerClientInterface.MOVE_UP);
                                    break;
                                case 2: // DOWN
                                    network.Move(ServerClientInterface.MOVE_DOWN);
                                    break;
                                case 4: // LEFT
                                    network.Move(ServerClientInterface.MOVE_LEFT);
                                    break;
                                case 8: // RIGHT
                                    network.Move(ServerClientInterface.MOVE_RIGHT);
                                    break;
                                case 9: // UP-RIGHT
                                    network.Move(ServerClientInterface.MOVE_UPRIGHT);
                                    break;
                                case 10: // DOWN-RIGHT
                                    network.Move(ServerClientInterface.MOVE_DOWNRIGHT);
                                    break;
                                case 6: // DOWN-LEFT
                                    network.Move(ServerClientInterface.MOVE_DOWNLEFT);
                                    break;
                                case 5: // UP-LEFT
                                    network.Move(ServerClientInterface.MOVE_UPLEFT);
                                    break;
                            }

                            // If the user pressed/held the left mouse button and is currently
                            // not looking at the buy menu
                            if ((input.LeftClickImmediate() || input.LeftHold()) && !showMenu)
                            {
                                // If the player has not already fired their weapon
                                if (!Client.CurrentWeapon.Fired)
                                {
                                    // Tell the server that they wish to fire their weapon
                                    network.FireWeapon();
                                }
                            }

                            // If there are more than 0 players on the server (Client is correctly synched)
                            // Update the rotation
                            if (Players.Count > 0)
                            {
                                // Get rotation of mouse relative to the center of the screen
                                float curRotation = input.MouseRotation(driver.Model.Camera);

                                // If the mouse has not moved, do not ask the server
                                // to change their rotation
                                if (curRotation != prevRotation)
                                {
                                    network.Rotate(curRotation);
                                }

                                // Set the camera to the player's position
                                driver.Model.Camera.Position = Client.Position;

                                // Set the previous rotation to the current one
                                prevRotation = curRotation;
                            }
                        }
                        else
                        {
                            // If the user is dead, allow them to look around the map
                            if (input.Tapped(Keys.W) || input.Held(Keys.W))
                            {
                                driver.Model.Camera.Position.Y -= 5f;
                            }
                            else if (input.Tapped(Keys.S) || input.Held(Keys.S))
                            {
                                driver.Model.Camera.Position.Y += 5f;
                            }

                            if (input.Tapped(Keys.A) || input.Held(Keys.A))
                            {
                                driver.Model.Camera.Position.X -= 5f;
                            }
                            else if (input.Tapped(Keys.D) | input.Held(Keys.D))
                            {
                                driver.Model.Camera.Position.X += 5f;
                            }
                        }

                        // Show scoreboard
                        showScoreBoard = input.Held(Keys.Tab);

                        if (showScoreBoard)
                        {
                            driver.Model.InterfaceManager.ShowPage("scoreboard");
                        }
                        else
                        {
                            driver.Model.InterfaceManager.HidePage("scoreboard");
                        }

                        // Ask the server to respawn their player
                        if (input.Tapped(Keys.P))
                        {
                            if (Client.State == ServerClientInterface.PlayerState.Dead)
                            {
                                network.RequestRespawn();
                            }
                        }
                    }
                    else
                    {
                        // Allow the user to move the camera if they are in spectator mode
                        if (input.Tapped(Keys.W) || input.Held(Keys.W))
                        {
                            driver.Model.Camera.Position.Y -= 5f;
                        }
                        else if (input.Tapped(Keys.S) || input.Held(Keys.S))
                        {
                            driver.Model.Camera.Position.Y += 5f;
                        }

                        if (input.Tapped(Keys.A) || input.Held(Keys.A))
                        {
                            driver.Model.Camera.Position.X -= 5f;
                        }
                        else if (input.Tapped(Keys.D) | input.Held(Keys.D))
                        {
                            driver.Model.Camera.Position.X += 5f;
                        }
                    }

                    // Update every client in the server
                    foreach (ClientPlayer ply in Players)
                    {
                        ply.Update(gameTime);
                    }

                    // Broken
                    if (Client != null)
                    {
                        //emitter.Cast(Client.Position, assets.MapData.TileMap, assets.MapData.Solids,
                        //    assets.MapData.MapArea);
                    }
                }
                else
                {
                    if (input.Tapped(Keys.W) || input.Held(Keys.W))
                    {
                        driver.Model.Camera.Position.Y -= 5f;
                    }
                    else if (input.Tapped(Keys.S) || input.Held(Keys.S))
                    {
                        driver.Model.Camera.Position.Y += 5f;
                    }

                    if (input.Tapped(Keys.A) || input.Held(Keys.A))
                    {
                        driver.Model.Camera.Position.X -= 5f;
                    }
                    else if (input.Tapped(Keys.D) | input.Held(Keys.D))
                    {
                        driver.Model.Camera.Position.X += 5f;
                    }

                    // Request to join the correct team
                    if (driver.Model.InterfaceManager.Clicked(input, "teamSelectMenu", "ctButton"))
                    {
                        teamSelect = false;
                        driver.Model.InterfaceManager.HideAll();
                        network.RequestTeamChange(ServerClientInterface.Team.CounterTerrorist);
                    }
                    else if (driver.Model.InterfaceManager.Clicked(input, "teamSelectMenu", "tButton"))
                    {
                        teamSelect = false;
                        driver.Model.InterfaceManager.HideAll();
                        network.RequestTeamChange(ServerClientInterface.Team.Terrorist);
                    }
                    else if (input.Tapped(Keys.Escape))
                    {
                        teamSelect = false;
                        driver.Model.InterfaceManager.HideAll();
                    }
                }
            }
        }

        /// <summary>
        /// Draws the game world
        /// </summary>
        /// <param name="sb"></param>
        public void DrawWorld(SpriteBatch sb)
        {
            // Draw black background
            sb.Draw(assets.PixelTexture, new Rectangle(
                -500, -500, assets.MapData.MapArea.Width + 1000, assets.MapData.MapArea.Height + 1000), Color.Black);
            
            // If the game is active
            if (CurState == GameEngineState.Active)
            {
                // Draw the map
                assets.MapData.Draw(sb);

                // Draw every player
                foreach (ClientPlayer ply in Players)
                {
                    if (ply.Identifier == Client.Identifier)
                    {
                        Client.Draw(sb);
                    }
                    else
                    {
                        if (ply.CurrentTeam == Client.CurrentTeam)
                        {
                            ply.Draw(sb);
                        }
                        else
                        {

                            // Get distance between the player and the enemy
                            Vector2 delta = ply.Position - Client.Position;

                            
                            // Get -2Pi - 2Pi version of the shooter's angle
                            float angle = (float)Math.Atan2(delta.Y, delta.X);
                            

                            // Get the direction of the shooter
                            Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                            // Get distance between the player and any possible obstacles in between the
                            // player and the enemy
                            RayCastResult result = raycaster.RayCastMethod(Client.Position, direction, 1280,
                                assets.MapData.TileMap, assets.MapData.MapArea, angle);

                            // Get the delta between the collision point and the shooter
                            Vector2 raycastDistance = result.CollisionPos - Client.Position;

                            if (raycastDistance.Length() > delta.Length())
                            {
                                ply.Draw(sb);
                            }
                        }
                    }
                    ply.DrawRay(sb);
                }

                // Draw a white background if the player is flashed
                if (Flashed)
                {
                    sb.Draw(assets.PixelTexture,
                        new Rectangle((int) Client.Position.X - 640, (int) Client.Position.Y - 360, 1280, 720),
                        Color.White*((flashTimer*2)/20f));
                }

            }
        }
        
        /// <summary>
        /// Draws the user interface
        /// </summary>
        /// <param name="sb"></param>
        public void DrawUI(SpriteBatch sb)
        {
            // Draw scoreboard and their playernames
            if (showScoreBoard)
            {
                int numCts = 0;
                int numTs = 0;
                foreach (ClientPlayer player in Players)
                {
                    switch (player.CurrentTeam)
                    {
                        case ServerClientInterface.Team.CounterTerrorist:
                            sb.DrawString(assets.DefaultFont, player.UserName + " | " +
                                                              ((player.State == ServerClientInterface.PlayerState.Dead) ? "DEAD" : ""), new Vector2(95, 75 + (numCts * 50)),
                                ServerClientInterface.CT_Colour);

                            numCts++;
                            break;
                        case ServerClientInterface.Team.Terrorist:
                            sb.DrawString(assets.DefaultFont, player.UserName + " | " +
                                                              ((player.State == ServerClientInterface.PlayerState.Dead) ? "DEAD" : ""), new Vector2(645, 75 + (numTs * 50)),
                                ServerClientInterface.T_Colour);
                            numTs++;

                            break;
                    }
                }
            }
        }
    }
}