// Author: Mark Voong
// Class Name: CStrikeModel.cs
// Project: CStrike2D
// Creation Date: Dec 21st 2015
// Modified Date:
// Description: Handles all logic processing of the game

using System;
using cstrike2d;
using LightEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CStrike2D
{
    public class CStrikeModel
    {
        public UIManager InterfaceManager { get; }

        public enum State
        {
            Menu,
            Options,
            Lobby,
            InGame
        }

        public State CurState { get; }

        public CStrikeModel()
        {
            CurState = State.Menu;

            // Initialize UIManager
            InterfaceManager = new UIManager();

            // Initialize buttons
            InterfaceManager.AddComponent(new Button("TestButton", new Rectangle(20, 500, 200, 40), Color.Black, Color.Gray, Color.Blue, "Test", 1.0f, EasingFunctions.AnimationType.QuinticIn));
            
            InterfaceManager.Show("TestButton");
        }

        public void Update(float gameTime)
        {
            InterfaceManager.Update(gameTime);

            switch (CurState)
            {
                case State.Menu:

                    if (fadeIn)
                    {
                        glowTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                        if (glowTimer >= 1000f)
                        {
                            fadeIn = !fadeIn;
                        }
                    }
                    else
                    {
                        glowTimer -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                        if (glowTimer <= 0f)
                        {
                            fadeIn = !fadeIn;
                        }
                    }

                    if (timer >= 100m)
                    {
                        timer = 0;
                        FPS = counter * 10m;
                        counter = 0;
                    }

                    if (inputManager.Held(Keys.A))
                    {
                        camera.Position.X -= 5f;
                    }

                    if (inputManager.Held(Keys.D))
                    {
                        camera.Position.X += 5f;
                    }

                    if (inputManager.Held(Keys.W))
                    {
                        camera.Position.Y -= 5f;
                    }

                    if (inputManager.Held(Keys.S))
                    {
                        camera.Position.Y += 5f;
                    }

                    stopWatch.Start();
                    collidables = GetCollidableTiles(newMap.ToTile(camera.Col(), camera.Row()));
                    stopWatch.Stop();

                    raycastCollidables = GetWalls(newMap.ToTile(camera.Col(), camera.Row()), (24 * (glowTimer / 1000f)) * (float)Math.PI / 12f);

                    pathfindingTime = Math.Round((double)stopWatch.ElapsedMilliseconds, 10);

                    stopWatch.Reset();

                    break;
                case State.Options:
                    break;
                case State.Lobby:
                    break;
                case State.InGame:
                    break;
            }
        }
    }
}