// Author: Mark Voong
// File Name: GameEngine.cs
// Project Name: Global Offensive
// Creation Date: Jan 4th, 2016
// Modified Date: Jan 20th, 2016
// Description: Handles all logic and drawing of the in-game components
using System.Collections.Generic;
using CStrike2DServer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CStrike2D
{
    public class GameEngine
    {
        public List<Entity> Entities { get; private set; }
        private CStrike2D driver;
        private NetworkManager network;
        private InputManager input;
        private AudioManager audioManager;
        private Assets assets;
        public List<ClientPlayer> Players { get; private set; }
        public ClientPlayer Client { get; private set; }

        private float prevRotation;

        private bool showMenu;
        private bool teamSelect = true;
        private bool showScoreBoard = false;

        public bool Flashed { get; private set; }
        private float flashTimer;

        public GameEngineState CurState { get; set; }

        public enum GameEngineState
        {
            InActive,
            Loaded,
            Active
        }

        private MenuState CurMenuState = MenuState.MainMenu;

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
        /// 
        /// </summary>
        /// <param name="driver"></param>
        public GameEngine(CStrike2D driver)
        {
            this.driver = driver;
            CurState = GameEngineState.InActive;
        }

        /// <summary>
        /// 
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

        public void SetWeapon(short identifier, byte weapon)
        {
            ClientPlayer player = Players.Find(ply => ply.Identifier == identifier);

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

        public void MovePlayer(short identifier, byte direction)
        {
            ClientPlayer player = Players.Find(ply => ply.Identifier == identifier);
            player.Move(direction);
        }

        public void ChangeTeam(short identifier, byte team)
        {
            ClientPlayer player = Players.Find(ply => ply.Identifier == identifier);
            player.SetTeam(team);
        }

        public void FireWeapon(short identifier)
        {
            ClientPlayer shooter = Players.Find(ply => ply.Identifier == identifier);
            PlaySound(shooter);
            shooter.Fire();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="soundID"></param>
        public void PlaySound(ClientPlayer shooter)
        {
            if (shooter != null)
            {
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


        public void PlaySound(ClientPlayer player, string sound)
        {
            audioManager.PlaySound(sound, audioManager.SoundEffectVolume, Client.Position,
                player.Position);
        }

        public void FlashPlayer()
        {
            audioManager.PlaySound("flashbang1", audioManager.SoundEffectVolume, Client.Position,
                Client.Position);
            Flashed = true;
            flashTimer = 20f;
        }

        public void Damage(short identifier, int health, int armor)
        {
            ClientPlayer player = Players.Find(ply => ply.Identifier == identifier);

            // Play sound if the player's health isn't the same (they were damaged)
            if (player.Health != health)
            {
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
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(float gameTime)
        {
            if (CurState == GameEngineState.Active)
            {
                if (!teamSelect)
                {
                    byte dir = 0;
                    if (Client.CurrentTeam != ServerClientInterface.Team.Spectator &&
                        Client.State != ServerClientInterface.PlayerState.Dead)
                    {
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
                                    else if (input.Tapped(Keys.D1) ||
                                             driver.Model.InterfaceManager.Clicked(input, "buyButtonMenu",
                                                 "pistolMenuButton"))
                                    {
                                        if (Client.CurrentTeam == ServerClientInterface.Team.CounterTerrorist)
                                        {
                                            driver.Model.InterfaceManager.ShowPage("pistolMenuButton");
                                        }
                                        else
                                        {

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

                                        if (driver.Model.InterfaceManager.Clicked(input, "ctRifleButtonMenu", "m4a1MenuButton"))
                                        {
                                            network.BuyWeapon(WeaponData.Weapon.M4A1);
                                            driver.Model.InterfaceManager.HidePage("ctRifleButtonMenu");
                                            CurMenuState = MenuState.MainMenu;
                                        }
                                    }
                                    else
                                    {
                                        driver.Model.InterfaceManager.ShowPage("tRifleButtonMenu");

                                        if (driver.Model.InterfaceManager.Clicked(input, "tRifleButtonMenu", "ak47MenuButton"))
                                        {
                                            network.BuyWeapon(WeaponData.Weapon.Ak47);
                                            driver.Model.InterfaceManager.HidePage("tRifleButtonMenu");
                                            CurMenuState = MenuState.MainMenu;
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
                            if (input.Tapped(Keys.D1))
                            {
                                //Client.SwitchWeapon(WeaponInfo.WeaponType.Primary);
                                //network.SwitchWeapon(Client.PrimaryWeapon.EntityID, NetInterface.SWITCH_PRIMARY);
                            }
                            else if (input.Tapped(Keys.D2))
                            {
                                //Client.SwitchWeapon(WeaponInfo.WeaponType.Secondary);
                                //network.SwitchWeapon(Client.SecondaryWeapon.EntityID, NetInterface.SWITCH_SECONDARY);
                            }
                            else if (input.Tapped(Keys.D3))
                            {
                                //Client.SwitchWeapon(WeaponInfo.WeaponType.Knife);
                                //network.SwitchWeapon(Client.Knife.EntityID, NetInterface.SWITCH_KNIFE);
                            }

                            if (input.Tapped(Keys.K))
                            {
                                network.Flash();
                            }
                        }

                        showScoreBoard = input.Held(Keys.Tab);

                        if (showScoreBoard)
                        {
                            driver.Model.InterfaceManager.ShowPage("scoreboard");
                        }
                        else
                        {
                            driver.Model.InterfaceManager.HidePage("scoreboard");
                        }

                        if (Flashed)
                        {
                            if (flashTimer >= 0f)
                            {
                                flashTimer -= 0.15f;
                                driver.Model.Shader.ChangeBlurAmount(flashTimer * 3f);
                            }
                        }

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

                        if ((input.LeftClickImmediate() || input.LeftHold()) && !showMenu)
                        {
                            if (!Client.CurrentWeapon.Fired)
                            {
                                Client.CurrentWeapon.FireWeapon();
                                network.FireWeapon();
                            }
                        }

                        if (Players.Count > 0)
                        {
                            float curRotation = input.MouseRotation(driver.Model.Camera);

                            if (curRotation != prevRotation)
                            {
                                network.Rotate(curRotation);
                                //network.SendRotData(curRotation);
                            }

                            driver.Model.Camera.Position = Client.Position;
                            //Client.SetRotation(curRotation);
                            prevRotation = curRotation;
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
                    }

                    foreach (ClientPlayer ply in Players)
                    {
                        ply.Update(gameTime);
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

                    if (input.Tapped(Keys.P))
                    {
                        if (Client.CurrentTeam != ServerClientInterface.Team.Spectator &&
                            Client.State == ServerClientInterface.PlayerState.Dead)
                        {
                            network.RequestRespawn();
                        }
                    }

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
            //sb.Draw(assets.PixelTexture, new Rectangle(0, 0, (int)driver.Model.Dimensions.X, (int)driver.Model.Dimensions.Y), Color.White);
            
            if (CurState == GameEngineState.Active)
            {
                assets.MapData.Draw(sb);

                foreach (ClientPlayer ply in Players)
                {
                    ply.Draw(sb);
                }

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
                                ServerClientInterface.CT_Color);

                            numCts++;
                            break;
                        case ServerClientInterface.Team.Terrorist:
                            sb.DrawString(assets.DefaultFont, player.UserName + " | " +
                                                              ((player.State == ServerClientInterface.PlayerState.Dead) ? "DEAD" : ""), new Vector2(645, 75 + (numTs * 50)),
                                ServerClientInterface.T_Color);
                            numTs++;

                            break;
                    }
                }
            }
        }
    }
}