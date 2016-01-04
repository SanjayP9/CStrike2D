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
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public class GameEngine
    {
        public List<Entity> Entities { get; private set; }
        private CStrike2D driver;
        private NetworkManager networkManager;

        public GameEngineState CurState { get; private set; }

        public GameEngine(CStrike2D Driver)
        {
            Driver = driver;
            CurState = GameEngineState.InActive;
        }

        public void Initialize(NetworkManager networkManager)
        {
            this.networkManager = networkManager;
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

        public void Update(float gameTime)
        {
            foreach (Entity entity in Entities)
            {
                entity.Update(gameTime);
            }

        }

        public void Draw(SpriteBatch sb)
        {
            foreach (Entity entity in Entities)
            {
                entity.Draw(sb);
            }
        }
    }
}
