// Author: Mark Voong
// File Name: View.cs
// Project: CStrike2D
// Date Created: Dec 6th 2015
// Date Modified: Dec 23rd 2015
// Description: Handles all graphical aspects of the game

using System;
using System.Linq;
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
                model.Camera.GetTransform(model.DriverInstance.GraphicsDevice));

            for (int col = 0; col < model.NewMap.MaxCol; col++)
            {
                for (int row = 0; row < model.NewMap.MaxRow; row++)
                {
                    Rectangle rect = new Rectangle(col * 64, row * 64, 64, 64);

                    switch (model.NewMap.TileMap[col, row].TileType)
                    {
                        case 1:
                            if (model.RaycastCollidables.Contains(model.NewMap.ToTile(col, row)))
                            {
                                sb.Draw(assets.PixelTexture, rect, Color.Gray);
                                sb.Draw(assets.PixelTexture, rect,
                                Color.Red * (0.6f * (model.GlowTimer / 1000f)));
                            }
                            else
                            {
                                sb.Draw(assets.PixelTexture, rect, Color.Green);
                            }

                            break;
                        case 0:
                            sb.Draw(assets.PixelTexture, rect, Color.Gold);
                            break;
                    }

                    sb.DrawString(assets.DefaultFont, col + ", " + row + "\n" + ((row * model.NewMap.MaxCol) + col), new Vector2(rect.Center.X - 15, rect.Center.Y - 15), Color.White);
                }
            }

            // model.Camera model.Center
            sb.Draw(assets.PixelTexture,
                new Rectangle((int)(model.Camera.Position.X - 5), (int)(model.Camera.Position.Y - 5), 10, 10), Color.Red);

            sb.End();

            sb.Begin(SpriteSortMode.Deferred,
                BlendState.AlphaBlend, null, null, null, null,
                model.Camera.GetTransform(model.DriverInstance.GraphicsDevice));

            for (int x = 0; x <= model.NewMap.TileMap.GetLength(0); x++)
            {
                for (int y = 0; y <= model.NewMap.TileMap.GetLength(1); y++)
                {
                    sb.Draw(assets.PixelTexture, new Rectangle(0, y * 64, 64 * x, 1), Color.Black);
                    sb.Draw(assets.PixelTexture, new Rectangle(x * 64, 0, 1, 64 * y), Color.Black);
                }
            }

            sb.End();

            sb.Begin();

            sb.DrawString(assets.DefaultFont,
                "Mouse (Local): " + model.Input.MousePosition + "\n" +
                "Mouse (World): " + model.Input.ScreenToWorld(model.Input.MousePosition, model.Camera, model.Center) + "\n" +
                "model.Camera (World): " + model.Camera.Position + "\n" +
                "Mouse (Row): " + model.Input.GetRow(model.Camera, model.Center) + "\n" +
                "Mouse (Column): " + model.Input.GetColumn(model.Camera, model.Center) + "\n" +
                "Quadrant of Direction: " +
                MathOps.ReturnQuadrant(MathOps.RealRadians(MathOps.Angle(model.Input.MousePosition, model.Center))) + "\n" +
                "Angle (Degrees): " + MathOps.ToDegrees(MathOps.Angle(model.Input.MousePosition, model.Center)) + "\n" +
                "Angle (Radians): " + MathOps.RealRadians(MathOps.Angle(model.Input.MousePosition, model.Center)) + "\n" +
                "Location (Col, Row): " + (int)(model.Camera.Position.X / 64) + ", " + (int)(model.Camera.Position.Y / 64) + "\n" +
                "Collidable Face Calculation Time: " + model.PathfindingTime + "ms" + "\n" +
                "Check Angle: " + ((24 * (model.GlowTimer / 1000f)) * (float)Math.PI) / 12f,
                Vector2.Zero, Color.White);

            // model.Center
            sb.Draw(assets.PixelTexture, new Rectangle((int)model.Center.X, (int)model.Center.Y,
                (int)(MathOps.Delta(model.Input.MousePosition, model.Center).Length()),
                2), null, Color.Red, (MathOps.Angle(model.Input.MousePosition, model.Center)), Vector2.Zero,
                SpriteEffects.None, 0);

            // FPS
            sb.DrawString(assets.DefaultFont, "FPS: " + model.DriverInstance.FPS, new Vector2(model.Dimensions.X - (assets.DefaultFont.MeasureString("FPS: " + model.DriverInstance.FPS).X), 0), Color.White);

            // Ray
            for (int i = 0; i < 24; i++)
            {
                sb.Draw(assets.PixelTexture,
                    new Rectangle((int)model.Center.X, (int)model.Center.Y,
                        (int)
                            MathOps.Delta(model.Center, MathOps.AngleToVector(i * (float)Math.PI / 12f))
                                .Length(),
                        1), null, Color.Red,
                    (i * (float)Math.PI / 12f), Vector2.Zero, SpriteEffects.None, 0);
            }


            // Mouse Point
            sb.Draw(assets.PixelTexture,
                new Rectangle((int)model.Input.MousePosition.X, (int)model.Input.MousePosition.Y, 10, 10), null,
                Color.Green, 0, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0);

            sb.End();
        }
    }
}
