// Author: Mark Voong
// File Name: View.cs
// Project Name: CStrike2D
// Creation Date: Dec 6th, 2015
// Modified Date: Jan 3rd, 2016
// Description: Handles all graphical aspects of the game
using System;
using System.Globalization;
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
                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, null,
                        null);
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
                                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                                    null,
                                    cullableRasterizer, null);
                                sb.GraphicsDevice.ScissorRectangle = new Rectangle(0, 100, 1280, 620);
                                foreach (GUIComponent component in page.Components)
                                {
                                    switch (component.Identifier)
                                    {
                                        case "masterVolumeText":
                                            ((TextBox) component).Draw(sb,
                                                "                        " + model.AudioManager.MasterVolume);
                                            break;
                                        case "uiVolumeText":
                                            ((TextBox) component).Draw(sb,
                                                "                                   " + model.AudioManager.UiVolume);
                                            break;
                                        case "musicVolumeText":
                                            ((TextBox) component).Draw(sb,
                                                "                          " + model.AudioManager.MusicVolume);
                                            break;
                                        case "sfxVolumeText":
                                            ((TextBox) component).Draw(sb,
                                                "          " + model.AudioManager.SoundEffectVolume);
                                            break;
                                        case "voiceVolumeText":
                                            ((TextBox) component).Draw(sb,
                                                "           " + model.AudioManager.VoiceVolume);
                                            break;
                                        default:
                                            component.Draw(sb);
                                            break;
                                    }
                                }
                                sb.End();
                                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                                    null, null, null);
                                break;
                            case "defaultMenu":
                                sb.End();
                                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                                    null,
                                    cullableRasterizer, null);
                                sb.GraphicsDevice.ScissorRectangle = new Rectangle(0, 19, 1280, 82);
                                page.Draw(sb);
                                sb.End();
                                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                                    null, null, null);
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
                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, cullableRasterizer, null,
                        model.Camera.GetTransform(model.DriverInstance.GraphicsDevice));
                    sb.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, 1280, 720);
                    model.Editor.DrawWorld(sb);
                    sb.End();

                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null,
                        null, null);
                    model.Editor.DrawUI(sb);
                    break;
                case CStrikeModel.State.InGame:
                    model.Shader.BeginRender();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null,
                        model.Camera.GetTransform(model.DriverInstance.GraphicsDevice));
                    model.GameEngine.DrawWorld(sb);
                    sb.End();



                    model.Shader.Draw(sb);

                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null,
                        cullableRasterizer, null);

                    sb.GraphicsDevice.ScissorRectangle = new Rectangle(90, 20, 1100, 650);

                    model.InterfaceManager.Draw(sb);

                    model.GameEngine.DrawUI(sb);
                    sb.End();

                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);

                    if (model.GameEngine.Client != null)
                    {
                        sb.DrawString(assets.DefaultFont, model.GameEngine.Client.Health.ToString(CultureInfo.InvariantCulture),
                            new Vector2(20, 680), Color.Yellow);
                        sb.DrawString(assets.DefaultFont, model.GameEngine.Client.Armor.ToString(CultureInfo.InvariantCulture),
                            new Vector2(200, 680), Color.Yellow);
                    }
                    break;
            }

            // FPS
            sb.DrawString(assets.DefaultFont, "FPS: " + model.DriverInstance.FPS, new Vector2(model.Dimensions.X - (assets.DefaultFont.MeasureString("FPS: " + model.DriverInstance.FPS).X), 0), Color.White);

            sb.End();
        }
    }
}