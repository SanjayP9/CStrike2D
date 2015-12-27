// Author: Mark Voong
// File Name: View.cs
// Project: CStrike2D
// Date Created: Dec 6th 2015
// Date Modified: Dec 23rd 2015
// Description: Handles all graphical aspects of the game

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    internal class CStrikeRenderer
    {
        // Asset loaders

        private Assets assets;

        /// <summary>
        /// Initializes the view and loads all applicable assets
        /// </summary>
        /// <param name="instance">The driver class that contains the ServiceProvider</param>
        public CStrikeRenderer(Game instance)
        {
            assets = new Assets(instance);
        }

        /// <summary>
        /// Load map assets in this method
        /// </summary>
        public void LoadMapContent()
        {
            assets.LoadMapContent();
        }

        /// <summary>
        /// Load game assets in this method
        /// </summary>
        public void LoadGameContent()
        {
            assets.LoadGameContent();
        }

        /// <summary>
        /// Unloads any content that is associated with the map
        /// </summary>
        public void UnloadMapContent()
        {
            assets.UnloadMapContent();
        }

        /// <summary>
        /// Unloads any content that is associated with the game
        /// </summary>
        public void UnloadGameContent()
        {
            assets.UnloadGameContent();
        }

        /// <summary>
        /// Unloads all content from the pipeline, use only when the game
        /// is shutting down
        /// </summary>
        public void UnloadAll()
        {
            assets.UnloadAll();
        }

        /// <summary>
        /// Draws all components of the game
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="model"></param>
        public void Draw(SpriteBatch sb, CStrikeModel model)
        {

            sb.Begin(SpriteSortMode.Deferred,
    BlendState.AlphaBlend, null, null, null, null,
    camera.GetTransform(model.DriverInstance.GraphicsDevice));

            for (int col = 0; col < newMap.MaxCol; col++)
            {
                for (int row = 0; row < newMap.MaxRow; row++)
                {
                    Rectangle rect = new Rectangle(col * 64, row * 64, 64, 64);

                    switch (newMap.TileMap[col, row].TileType)
                    {
                        case 1:
                            if (raycastCollidables.Contains(newMap.ToTile(col, row)))
                            {
                                spriteBatch.Draw(pixelTexture, rect, Color.Gray);
                                sb.Draw(pixelTexture, rect,
                                Color.Red * (0.6f * (glowTimer / 1000f)));
                            }
                            else
                            {
                                sb.Draw(pixelTexture, rect, Color.Green);
                            }

                            break;
                        case 0:
                            sb.Draw(pixelTexture, rect, Color.Gold);
                            break;
                    }

                    sb.DrawString(defFont, col + ", " + row + "\n" + ((row * newMap.MaxCol) + col), new Vector2(rect.Center.X - 15, rect.Center.Y - 15), Color.White);
                }
            }

            // Camera Center
            sb.Draw(pixelTexture,
                new Rectangle((int)(camera.Position.X - 5), (int)(camera.Position.Y - 5), 10, 10), Color.Red);

            sb.End();

            sb.Begin(SpriteSortMode.Deferred,
                BlendState.AlphaBlend, null, null, null, null,
                camera.GetTransform(GraphicsDevice));

            for (int x = 0; x <= newMap.TileMap.GetLength(0); x++)
            {
                for (int y = 0; y <= newMap.TileMap.GetLength(1); y++)
                {
                    sb.Draw(pixelTexture, new Rectangle(0, y * 64, 64 * x, 1), Color.Black);
                    sb.Draw(pixelTexture, new Rectangle(x * 64, 0, 1, 64 * y), Color.Black);
                }
            }

            sb.End();

            sb.Begin();

            sb.DrawString(defFont,
                "Mouse (Local): " + inputManager.MousePosition + "\n" +
                "Mouse (World): " + inputManager.ScreenToWorld(inputManager.MousePosition, camera, Center) + "\n" +
                "Camera (World): " + camera.Position + "\n" +
                "Mouse (Row): " + inputManager.GetRow(camera, Center) + "\n" +
                "Mouse (Column): " + inputManager.GetColumn(camera, Center) + "\n" +
                "Quadrant of Direction: " +
                MathOps.ReturnQuadrant(MathOps.RealRadians(MathOps.Angle(inputManager.MousePosition, Center))) + "\n" +
                "Angle (Degrees): " + MathOps.ToDegrees(MathOps.Angle(inputManager.MousePosition, Center)) + "\n" +
                "Angle (Radians): " + MathOps.RealRadians(MathOps.Angle(inputManager.MousePosition, Center)) + "\n" +
                "Location (Col, Row): " + (int)(camera.Position.X / 64) + ", " + (int)(camera.Position.Y / 64) + "\n" +
                "Collidable Face Calculation Time: " + pathfindingTime + "ms" + "\n" +
                "Check Angle: " + ((24 * (glowTimer / 1000f)) * (float)Math.PI) / 12f,
                Vector2.Zero, Color.White);

            // Center
            sb.Draw(pixelTexture, new Rectangle((int)Center.X, (int)Center.Y,
                (int)(MathOps.Delta(inputManager.MousePosition, Center).Length()),
                2), null, Color.Red, (MathOps.Angle(inputManager.MousePosition, Center)), Vector2.Zero,
                SpriteEffects.None, 0);

            // FPS
            sb.DrawString(defFont, "FPS: " + FPS, new Vector2(graphics.PreferredBackBufferWidth - (defFont.MeasureString("FPS: " + FPS).X), 0), Color.White);

            // Ray
            for (int i = 0; i < 24; i++)
            {
                sb.Draw(pixelTexture,
                    new Rectangle((int)Center.X, (int)Center.Y,
                        (int)
                            MathOps.Delta(Center, MathOps.AngleToVector(i * (float)Math.PI / 12f))
                                .Length(),
                        1), null, Color.Red,
                    (i * (float)Math.PI / 12f), Vector2.Zero, SpriteEffects.None, 0);
            }


            // Mouse Point
            sb.Draw(pixelTexture,
                new Rectangle((int)inputManager.MousePosition.X, (int)inputManager.MousePosition.Y, 10, 10), null,
                Color.Green, 0, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0);

        }
    }
}
