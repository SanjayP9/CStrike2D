// Author: Mark Voong
// File Name: GameEngine.cs
// Project Name: CStrike2D
// Creation Date: Jan 4th, 2015
// Modified Date: Jan 3rd, 2016
// Description: Handles all logic and drawing of the in-game components
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        List<Player> players;

        public GameEngineState CurState { get; set; }

        public GameEngine(CStrike2D driver)
        {
            this.driver = driver;
            CurState = GameEngineState.InActive;
        }

        public void Initialize(NetworkManager networkManager, AudioManager audio, InputManager input, Assets assets)
        {
            if (CurState == GameEngineState.InActive)
            {
                network = networkManager;
                audioManager = audio;
                this.input = input;
                this.assets = assets;
                players = new List<Player>();
                CurState = GameEngineState.Loaded;
            }
        }

        public enum GameEngineState
        {
            InActive,
            Loaded,
            Active
        }

        public void AddPlayer(string name, Vector2 position, short playerID)
        {
            players.Add(new Player(name, position, playerID, assets));
        }

        public void AddEntity(Entity entity)
        {
            Entities.Add(entity);
        }

        public void Update(float gameTime)
        {
            if (CurState == GameEngineState.Active)
            {


                if (input.Tapped(Keys.W) || input.Held(Keys.W))
                {
                    network.SendInputData(NetInterface.MOVE_UP);
                }
                else if (input.Tapped(Keys.S) || input.Held(Keys.S))
                {
                    network.SendInputData(NetInterface.MOVE_DOWN);
                }

                if (input.Tapped(Keys.A) || input.Held(Keys.A))
                {
                    network.SendInputData(NetInterface.MOVE_LEFT);
                }
                else if (input.Tapped(Keys.D) | input.Held(Keys.D))
                {
                    network.SendInputData(NetInterface.MOVE_RIGHT);
                }

                if (input.LeftClick())
                {
                    network.SendInputData(NetInterface.FIRE);
                }

                driver.Model.Camera.Position = players[0].Position;

                if (players.Count > 0)
                {
                    players[0].SetRot(input.MouseRotation(driver.Model.Camera));
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (CurState == GameEngineState.Active)
            {
                foreach (Player ply in players)
                {
                    ply.Draw(sb);
                }
            }

            sb.Draw(assets.PixelTexture, new Rectangle(20, 20, 200, 200), Color.Yellow);
        }

        public void PlaySound(int playerID, short soundID)
        {
            Player player = players.Find(ply => ply.PlayerID == playerID);
            switch (soundID)
            {
                case NetInterface.AK47_SHOT:
                    audioManager.PlaySound("ak47shot", audioManager.SoundEffectVolume, players[0].Position,
                        player.Position);
                    break;
            }
        }

        public void MovePlayer(short playerID, byte direction)
        {
            Player player = players.Find(ply => ply.PlayerID == playerID);
            player.Move(direction);
        }

        public bool Exists(short playerID)
        {
            return players.Exists(ply => ply.PlayerID == playerID);
        }
}
}
