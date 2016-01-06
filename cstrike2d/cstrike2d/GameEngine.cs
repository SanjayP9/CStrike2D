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
        List<Player> players;

        public GameEngineState CurState { get; set; }

        public GameEngine(CStrike2D Driver)
        {
            Driver = driver;
            CurState = GameEngineState.InActive;
        }

        public void Initialize(NetworkManager networkManager, AudioManager audio, InputManager input)
        {
            network = networkManager;
            audioManager = audio;
            this.input = input;
            players = new List<Player>();
            players.Add(new Player(networkManager.ClientName, Vector2.Zero, players.Count));
        }

        public enum GameEngineState
        {
            InActive,
            Active
        }

        public void AddPlayer(Player player)
        {
            players.Add(player);
        }

        public void AddEntity(Entity entity)
        {
            Entities.Add(entity);
        }

        public void Update(float gameTime)
        {
            if (CurState == GameEngineState.Active)
            {
                if (input.Tapped(Keys.W))
                {
                    network.SendInputData(NetInterface.MOVE_UP);
                }
                else if (input.Tapped(Keys.S))
                {
                    network.SendInputData(NetInterface.MOVE_DOWN);
                }

                if (input.Tapped(Keys.A))
                {
                    network.SendInputData(NetInterface.MOVE_LEFT);
                }
                else if (input.Tapped(Keys.D))
                {
                    network.SendInputData(NetInterface.MOVE_RIGHT);
                }

                if (input.LeftClick())
                {
                    network.SendInputData(NetInterface.FIRE);
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {

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

        public void MovePlayer(int playerID, int direction)
        {
            Player player = players.Find(ply => ply.PlayerID == playerID);
            player.Move(direction);
        }

        public bool Exists(int playerID)
        {
            return players.Exists(ply => ply.PlayerID == playerID);
        }
}
}
