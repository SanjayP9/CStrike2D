// Author: Mark Voong
// File Name: GameEngine.cs
// Project Name: CStrike2D
// Creation Date: Jan 4th, 2015
// Modified Date: Jan 3rd, 2016
// Description: Handles all logic and drawing of the in-game components
using System.Collections.Generic;
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
        public List<Player> Players { get; private set; }
        private Player clientPlayer;

        private float prevRotation;

        private bool showMenu;
        private bool teamSelect = true;
        private bool showScoreBoard = false;

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
                Players = new List<Player>();
                CurState = GameEngineState.Loaded;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="playerID"></param>
        /// <param name="rotation"></param>
        /// <param name="team"></param>
        public void AddPlayer(string name, Vector2 position, short playerID, float rotation, byte team, short entityID, short curWeapon)
        {
            Player ply = new Player(name, position, playerID, team, assets);
            ply.SetRot(rotation);
            ply.SetWeapon(entityID, curWeapon);
            Players.Add(ply);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        public void SetClientPlayer(Player player)
        {
            clientPlayer = player;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public void AddEntity(Entity entity)
        {
            Entities.Add(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="soundID"></param>
        public void PlaySound(long playerID, short soundID)
        {
            Player player = Players.Find(ply => ply.PlayerID == playerID);

            if (player != null)
            {
                switch (soundID)
                {
                    case NetInterface.AK47_SHOT:
                        audioManager.PlaySound("awpshot", audioManager.SoundEffectVolume,
                            clientPlayer.Position, player.Position);
                        break;
                    case NetInterface.AWP_SHOT:
                        audioManager.PlaySound("awpshot", audioManager.SoundEffectVolume,
                            clientPlayer.Position, player.Position);
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="direction"></param>
        public void MovePlayer(short playerID, byte direction)
        {
            Player player = Players.Find(ply => ply.PlayerID == playerID);

            if (player != null)
            {
                player.Move(direction);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public bool Exists(long playerID)
        {
            return Players.Exists(ply => ply.PlayerID == playerID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(float gameTime)
        {
            if (CurState == GameEngineState.Active)
            {
                showScoreBoard = input.Held(Keys.Tab);

                if (showScoreBoard)
                {
                    driver.Model.InterfaceManager.ShowPage("scoreboard");
                }
                else
                {
                    driver.Model.InterfaceManager.HidePage("scoreboard");
                }

                if (!teamSelect)
                {
                    byte dir = 0;
                    if (clientPlayer.Team != NetInterface.Team.Spectator)
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
                                        if (clientPlayer.Team == NetInterface.Team.CT)
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
                                        if (clientPlayer.Team == NetInterface.Team.CT)
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
                                        if (clientPlayer.Team == NetInterface.Team.CT)
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
                                    if (clientPlayer.Team == NetInterface.Team.CT)
                                    {
                                        driver.Model.InterfaceManager.ShowPage("ctRifleButtonMenu");
                                    }
                                    else
                                    {
                                        driver.Model.InterfaceManager.ShowPage("tRifleButtonMenu");

                                        if (driver.Model.InterfaceManager.Clicked(input, "tRifleButtonMenu", "ak47MenuButton"))
                                        {
                                            network.RequestBuy(NetInterface.WEAPON_AK47);
                                            driver.Model.InterfaceManager.HidePage("tRifleButtonMenu");
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
                                clientPlayer.SwitchWeapon(WeaponInfo.WeaponType.Primary);
                                network.SwitchWeapon(NetInterface.SWITCH_PRIMARY);
                            }
                            else if (input.Tapped(Keys.D2))
                            {
                                clientPlayer.SwitchWeapon(WeaponInfo.WeaponType.Secondary);
                                network.SwitchWeapon(NetInterface.SWITCH_SECONDARY);
                            }
                            else if (input.Tapped(Keys.D3))
                            {
                                clientPlayer.SwitchWeapon(WeaponInfo.WeaponType.Knife);
                                network.SwitchWeapon(NetInterface.SWITCH_KNIFE);
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
                                network.SendInputData(NetInterface.MOVE_UP);
                                break;
                            case 2: // DOWN
                                network.SendInputData(NetInterface.MOVE_DOWN);
                                break;
                            case 4: // LEFT
                                network.SendInputData(NetInterface.MOVE_LEFT);
                                break;
                            case 8: // RIGHT
                                network.SendInputData(NetInterface.MOVE_RIGHT);
                                break;
                            case 9: // UP-RIGHT
                                network.SendInputData(NetInterface.MOVE_UPRIGHT);
                                break;
                            case 10: // DOWN-RIGHT
                                network.SendInputData(NetInterface.MOVE_DOWNRIGHT);
                                break;
                            case 6: // DOWN-LEFT
                                network.SendInputData(NetInterface.MOVE_DOWNLEFT);
                                break;
                            case 5: // UP-LEFT
                                network.SendInputData(NetInterface.MOVE_UPLEFT);
                                break;
                        }

                        if (input.LeftClickImmediate() && !showMenu)
                        {
                            network.SendInputData(NetInterface.FIRE);
                        }

                        if (Players.Count > 0)
                        {
                            float curRotation = input.MouseRotation(driver.Model.Camera);

                            if (curRotation != prevRotation)
                            {
                                network.SendRotData(curRotation);
                            }

                            driver.Model.Camera.Position = clientPlayer.Position;
                            clientPlayer.SetRot(curRotation);
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

                    foreach (Player ply in Players)
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

                    if (driver.Model.InterfaceManager.Clicked(input, "teamSelectMenu", "ctButton"))
                    {
                        teamSelect = false;
                        driver.Model.InterfaceManager.HideAll();
                        network.SendChangeTeam(NetInterface.PLY_CT);
                    }
                    else if (driver.Model.InterfaceManager.Clicked(input, "teamSelectMenu", "tButton"))
                    {
                        teamSelect = false;
                        driver.Model.InterfaceManager.HideAll();
                        network.SendChangeTeam(NetInterface.PLY_T);
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
        /// 
        /// </summary>
        /// <param name="sb"></param>
        public void DrawWorld(SpriteBatch sb)
        {
            sb.Draw(assets.PixelTexture, new Rectangle(0, 0, (int)driver.Model.Dimensions.X, (int)driver.Model.Dimensions.Y), Color.White);
            if (CurState == GameEngineState.Active)
            {
                foreach (Player ply in Players)
                {
                    ply.Draw(sb);
                }
            }
        }

        public void DrawUI(SpriteBatch sb)
        {
            if (showScoreBoard)
            {
                for (int i = 0; i < Players.Count; i++)
                {
                    switch (Players[i].Team)
                    {
                        case NetInterface.Team.CT:
                            sb.DrawString(assets.DefaultFont, Players[i].Name, new Vector2(95, 50 + (i*40)),
                                NetInterface.CT_Color);
                            break;
                        case NetInterface.Team.T:
                            sb.DrawString(assets.DefaultFont, Players[i].Name, new Vector2(645, 50 + (i * 50)),
                                NetInterface.T_Color);
                            break;
                    }
                }
            }
        }
    }
}