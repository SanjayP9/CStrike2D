// Author: Mark Voong
// File Name: View.cs
// Project Name: CStrike2D
// Creation Date: Dec 6th, 2015
// Modified Date: Jan 3rd, 2016
// Description: Handles all graphical aspects of the game
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public class CStrikeView
    {
        // Asset loaders
        private Assets assets;
        private RasterizerState cullableRasterizer;

        /// <summary>
        /// Initializes the view and loads all applicable assets
        /// </summary>
        /// <param name="assets"></param>
        public CStrikeView(CStrike2D driver)
        {
            assets = driver.Assets;
            cullableRasterizer = new RasterizerState {ScissorTestEnable = true};
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
            switch (model.CurState)
            {
                case CStrikeModel.State.Menu:
                    model.Shader.BeginRender();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, null, null);
                    // Show counter-terrorist background if true, terrorist background if false
                    sb.Draw(model.MenuBackgroundType ? assets.CTMenuBackground : assets.TMenuBackground, Vector2.Zero,
                        Color.White);

                    // Draw Background UI elements

                    //sb.Draw(assets.PixelTexture, new Rectangle(0, 20, (int)model.Dimensions.X, 80), Color.Black * 0.8f);

                    foreach (GUIPage page in model.InterfaceManager.GUIPages)
                    {
                        switch (page.Identifier)
                        {
                            case "optionsMenu":
                                sb.End();
                                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null,
                                    cullableRasterizer, null);
                                sb.GraphicsDevice.ScissorRectangle = new Rectangle(0, 100, 1280, 620);
                                foreach (GUIComponent component in page.Components)
                                {
                                    switch (component.Identifier)
                                    {
                                        case "masterVolumeText":
                                            ((TextBox)component).Draw(sb, "                        " + model.AudioManager.MasterVolume);
                                            break;
                                        case "uiVolumeText":
                                            ((TextBox)component).Draw(sb, "                                   " + model.AudioManager.UiVolume);
                                            break;
                                        case "musicVolumeText":
                                            ((TextBox)component).Draw(sb, "                          " + model.AudioManager.MusicVolume);
                                            break;
                                        case "sfxVolumeText":
                                            ((TextBox)component).Draw(sb, "          " + model.AudioManager.SoundEffectVolume);
                                            break;
                                        case "voiceVolumeText":
                                            ((TextBox)component).Draw(sb, "           " + model.AudioManager.VoiceVolume);
                                            break;
                                        default:
                                            component.Draw(sb);
                                            break;
                                    }
                                }
                                sb.End();
                                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, null, null);
                                break;
                            case "defaultMenu":
                                sb.End();
                                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null,
                                    cullableRasterizer, null);
                                sb.GraphicsDevice.ScissorRectangle = new Rectangle(0, 19, 1280, 82);
                                page.Draw(sb);
                                sb.End();
                                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, null, null);
                                break;
                            default:
                                page.Draw(sb);
                                break;
                        }
                    }

                    sb.End();
                    model.Shader.Draw(sb);
                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);

                    break;
                case CStrikeModel.State.Options:
                    break;
                case CStrikeModel.State.Lobby:
                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);

                    
                    sb.DrawString(assets.DefaultFont, "Connect to Server: " + model.Address, Vector2.Zero, Color.White);

                    break;
                case CStrikeModel.State.LevelEditor:
                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, cullableRasterizer, null, model.Camera.GetTransform(model.DriverInstance.GraphicsDevice));
                    sb.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, 1280, 720);
                    //model.Editor.DrawWorld(sb);
                    sb.End();

                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null,
                        null, null);
                    //model.Editor.DrawUI(sb);
                    break;
                case CStrikeModel.State.InGame:
                    model.Shader.BeginRender();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, model.Camera.GetTransform(model.DriverInstance.GraphicsDevice));
                    model.GameEngine.DrawWorld(sb);
                    sb.End();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null,
                        cullableRasterizer, null);

                    sb.GraphicsDevice.ScissorRectangle = new Rectangle(90, 20, 1100, 650);
                                                           
                    model.InterfaceManager.Draw(sb);
                    model.GameEngine.DrawUI(sb);

                    sb.End();
                                        
                    model.Shader.Draw(sb);
                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);

                    sb.DrawString(assets.DefaultFont, "Camera Pos: " + model.Camera.Position, new Vector2(model.Dimensions.X - 200, 30),  Color.White);
                    break;
            }

            /*
            for (int col = 0; col < model.NewMap.MaxCol; col++)
            {
                for (int row = 0; row < model.NewMap.MaxRow; row++)
                {
                    Rectangle rect = new Rectangle(col*64, row*64, 64, 64);

                    switch (model.NewMap.TileMap[col, row].TileType)
                    {
                        case 1:
                            if (model.RaycastCollidables.Contains(model.NewMap.ToTile(col, row)))
                            {
                                sb.Draw(assets.PixelTexture, rect, Color.Gray);
                                sb.Draw(assets.PixelTexture, rect, Color.Red*(0.6f*(model.GlowTimer/1000f)));
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

                    sb.DrawString(assets.DefaultFont, col + ", " + row + "\n" + ((row*model.NewMap.MaxCol) + col), new Vector2(rect.Center.X - 15, rect.Center.Y - 15), Color.White);
                }
            }

            // model.Camera model.Center
            sb.Draw(assets.PixelTexture, new Rectangle((int) (model.Camera.Position.X - 5), (int) (model.Camera.Position.Y - 5), 10, 10), Color.Red);

            sb.End();

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, model.Camera.GetTransform(model.DriverInstance.GraphicsDevice));

            for (int x = 0; x <= model.NewMap.TileMap.GetLength(0); x++)
            {
                for (int y = 0; y <= model.NewMap.TileMap.GetLength(1); y++)
                {
                    sb.Draw(assets.PixelTexture, new Rectangle(0, y*64, 64*x, 1), Color.Black);
                    sb.Draw(assets.PixelTexture, new Rectangle(x*64, 0, 1, 64*y), Color.Black);
                }
            }

            sb.End();

            sb.Begin();

            sb.DrawString(assets.DefaultFont, "Mouse (Local): " + model.Input.MousePosition + "\n" + "Mouse (World): " + model.Input.ScreenToWorld(model.Input.MousePosition, model.Camera, model.Center) + "\n" + "model.Camera (World): " + model.Camera.Position + "\n" + "Mouse (Row): " + model.Input.GetRow(model.Camera, model.Center) + "\n" + "Mouse (Column): " + model.Input.GetColumn(model.Camera, model.Center) + "\n" + "Quadrant of Direction: " + MathOps.ReturnQuadrant(MathOps.RealRadians(MathOps.Angle(model.Input.MousePosition, model.Center))) + "\n" + "Angle (Degrees): " + MathOps.ToDegrees(MathOps.Angle(model.Input.MousePosition, model.Center)) + "\n" + "Angle (Radians): " + MathOps.RealRadians(MathOps.Angle(model.Input.MousePosition, model.Center)) + "\n" + "Location (Col, Row): " + (int) (model.Camera.Position.X/64) + ", " + (int) (model.Camera.Position.Y/64) + "\n" + "Collidable Face Calculation Time: " + model.PathfindingTime + "ms" + "\n" + "Check Angle: " + ((24*(model.GlowTimer/1000f))*(float) Math.PI)/12f, Vector2.Zero, Color.White);

            // model.Center
            sb.Draw(assets.PixelTexture, new Rectangle((int) model.Center.X, (int) model.Center.Y, (int) (MathOps.Delta(model.Input.MousePosition, model.Center).Length()), 2), null, Color.Red, (MathOps.Angle(model.Input.MousePosition, model.Center)), Vector2.Zero, SpriteEffects.None, 0);


            // Ray
            for (int i = 0; i < 24; i++)
            {
                sb.Draw(assets.PixelTexture, new Rectangle((int) model.Center.X, (int) model.Center.Y, (int) MathOps.Delta(model.Center, MathOps.AngleToVector(i*(float) Math.PI/12f)).Length(), 1), null, Color.Red, (i*(float) Math.PI/12f), Vector2.Zero, SpriteEffects.None, 0);
            }

            
            // Mouse Point
            sb.Draw(assets.PixelTexture, new Rectangle((int) model.Input.MousePosition.X, (int) model.Input.MousePosition.Y, 10, 10), null, Color.Green, 0, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0);

            */

            // FPS
            sb.DrawString(assets.DefaultFont, "FPS: " + model.DriverInstance.FPS, new Vector2(model.Dimensions.X - (assets.DefaultFont.MeasureString("FPS: " + model.DriverInstance.FPS).X), 0), Color.White);

            sb.End();
        }
    }
}