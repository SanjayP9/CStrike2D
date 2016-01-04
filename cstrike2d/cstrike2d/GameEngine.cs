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

        public void Initialize(NetworkManager networkManager, AudioManager audio)
        {
            network = networkManager;
            audioManager = audio;
            players = new List<Player>();
        }

        public enum GameEngineState
        {
            InActive,
            Active
        }

        public void AddEntity(Entity entity)
        {
            Entities.Add(entity);
        }

        public void Update(InputManager input, NetworkManager network, float gameTime)
        {
            if (CurState == GameEngineState.Active)
            {
                if (input.Tapped(Keys.W))
                {
                    network.SendInputData(NetworkManager.MOVE_UP);
                }
                else if (input.Tapped(Keys.S))
                {
                    network.SendInputData(NetworkManager.MOVE_DOWN);
                }

                if (input.Tapped(Keys.A))
                {
                    network.SendInputData(NetworkManager.MOVE_LEFT);
                }
                else if (input.Tapped(Keys.D))
                {
                    network.SendInputData(NetworkManager.MOVE_RIGHT);
                }



                if (input.LeftClick())
                {
                    network.SendInputData(20);
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            
        }

        public void PlaySound()
        {
            audioManager.PlaySound("ak47shot", audioManager.SoundEffectVolume, Vector2.Zero, Vector2.Zero);
        }
    }
}
