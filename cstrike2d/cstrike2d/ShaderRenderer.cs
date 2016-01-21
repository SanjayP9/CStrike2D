// Author: Mark Voong
// File Name: ShaderRenderer.cs
// Project Name: Global Offensive
// Creation Date: Jan 11th, 2016
// Modified Date: Jan 21st, 2016
// Description: Houses all of the shaders present in this game. Allows for a scene to be
//              post-processed and drawn to the screen via 2 methods for easy use
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace CStrike2D
{
    public class ShaderRenderer
    {
        private CStrike2D driver;           // Driver class
        private RenderTarget2D sceneRender; // Regular render of the game
        private RenderTarget2D firstRender; // First gaussian pass
        private RenderTarget2D secondRender;// Second gaussian pass

        /// <summary>
        /// Amount of blur
        /// </summary>
        public float BlurAmount { get; private set; }

        // Shader
        private Effect blurEffect;

        private Matrix matrix;

        private AlphaTestEffect alphaEffect;

        private DepthStencilState stencilOne;
        private DepthStencilState stencilTwo;

        private Assets assets;

        /// <summary>
        /// Renderer
        /// </summary>
        /// <param name="instance"></param>
        public ShaderRenderer(CStrike2D instance)
        {
            driver = instance;
            assets = driver.Assets;
            BlurAmount = 12f;

            PresentationParameters pp = driver.GraphicsDevice.PresentationParameters;
            SurfaceFormat format = pp.BackBufferFormat;

            int width = 1280;
            int height = 720;

            sceneRender = new RenderTarget2D(driver.GraphicsDevice, width, height, false,
                                                   format, pp.DepthStencilFormat, pp.MultiSampleCount,
                                                   RenderTargetUsage.DiscardContents);

            width /= 2;
            height /= 2;

            firstRender = new RenderTarget2D(driver.GraphicsDevice, width, height, false, format, DepthFormat.None);
            secondRender = new RenderTarget2D(driver.GraphicsDevice, width, height, false, format, DepthFormat.None);

            matrix = Matrix.CreateOrthographicOffCenter(0,
                instance.GraphicsDevice.PresentationParameters.BackBufferWidth,
                instance.GraphicsDevice.PresentationParameters.BackBufferHeight,
                0, 0, 1
                );
            
            alphaEffect = new AlphaTestEffect(instance.GraphicsDevice)
            {
                Projection = matrix
            };

            stencilOne = new DepthStencilState
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.Always,
                StencilPass = StencilOperation.Replace,
                ReferenceStencil = 1,
                DepthBufferEnable = false,
            };

            stencilTwo = new DepthStencilState
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.LessEqual,
                StencilPass = StencilOperation.Keep,
                ReferenceStencil = 1,
                DepthBufferEnable = false,
            };
        }

        /// <summary>
        /// Changes the blur ammount
        /// </summary>
        /// <param name="amount"></param>
        public void ChangeBlurAmount(float amount)
        {
            BlurAmount = amount;
        }

        /// <summary>
        /// Loads the shader
        /// </summary>
        public void Load()
        {
            blurEffect = driver.Assets.BlurEffect;
        }

        /// <summary>
        /// Begin the render
        /// </summary>
        public void BeginRender()
        {
            driver.GraphicsDevice.SetRenderTarget(sceneRender);
            driver.GraphicsDevice.Clear(Color.Transparent);
        }

        /// <summary>
        /// Draws everything
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            // If there is blur to be drawn
            if (BlurAmount > 0f)
            {
                driver.GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;

                DrawScreen(sb, sceneRender, firstRender, null);

                SetBlurEffectParameters(1.0f/(float) firstRender.Width, 0);

                DrawScreen(sb, firstRender, secondRender,
                    blurEffect);

                SetBlurEffectParameters(0, 1.0f/(float) firstRender.Height);

                DrawScreen(sb, secondRender, firstRender,
                    blurEffect);


                driver.GraphicsDevice.SetRenderTarget(null);

                DrawScreen(sb, firstRender, 1280, 720, null);
            }
            else
            {
                driver.GraphicsDevice.SetRenderTarget(null);
                DrawScreen(sb, sceneRender, 1280, 720, null);
            }
        }

        /// <summary>
        /// Prepares a screen tob e rendered
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="texture"></param>
        /// <param name="render"></param>
        /// <param name="effect"></param>
        private void DrawScreen(SpriteBatch sb, Texture2D texture, RenderTarget2D render, Effect effect)
        {
            driver.GraphicsDevice.SetRenderTarget(render);

            DrawScreen(sb, texture,
                render.Width, render.Height, effect);
        }

        /// <summary>
        /// Draws a render
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="texture"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="effect"></param>
        private void DrawScreen(SpriteBatch sb, Texture2D texture, float width, float height,
                Effect effect)
        {
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, effect);

            sb.Draw(texture, new Rectangle(0, 0, (int)width, (int)height), Color.White);

            sb.End();
        }

        /// <summary>
        /// Sets blur parameters
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        private void SetBlurEffectParameters(float dx, float dy)
        {
            // Look up the sample weight and offset effect parameters.
            EffectParameter weightsParameter, offsetsParameter;

            weightsParameter = blurEffect.Parameters["SampleWeights"];
            offsetsParameter = blurEffect.Parameters["SampleOffsets"];

            // Get max samples the shader can support
            int sampleCount = weightsParameter.Elements.Count;

            // Store filler settings
            float[] sampleWeights = new float[sampleCount];
            Vector2[] sampleOffsets = new Vector2[sampleCount];

            // Set the first sample
            sampleWeights[0] = ComputeGaussian(0);
            sampleOffsets[0] = new Vector2(0);

            // Sum of all sample weights
            float totalWeights = sampleWeights[0];

            // Go through each sample
            for (int i = 0; i < sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussian(i + 1);

                sampleWeights[i * 2 + 1] = weight;
                sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                // Get the location between two texels
                // the shader automatically averages the two
                float sampleOffset = i * 2 + 1.5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                sampleOffsets[i * 2 + 1] = delta;
                sampleOffsets[i * 2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for (int i = 0; i < sampleWeights.Length; i++)
            {
                sampleWeights[i] /= totalWeights;
            }

            // Set new effect settings
            weightsParameter.SetValue(sampleWeights);
            offsetsParameter.SetValue(sampleOffsets);
        }


        /// <summary>
        /// Calculates gaussian
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        float ComputeGaussian(float n)
        {
            float theta = BlurAmount;

            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                           Math.Exp(-(n * n) / (2 * theta * theta)));
        }


        public void DrawPolygon(SpriteBatch sb)
        {
            // Draw mask
            sb.Begin(SpriteSortMode.Immediate, null, null, stencilOne, null, alphaEffect);

            sb.Draw(assets.PixelTexture, new Rectangle(0, 0, 1280, 720), Color.Black);
            sb.Draw(assets.CTTexture, Vector2.Zero, Color.White);

            sb.End();

            sb.Begin(SpriteSortMode.Immediate, null, null, stencilTwo, null, alphaEffect);
            sb.Draw(assets.PixelTexture, new Rectangle(20, 20, 800, 40), Color.Transparent);

            sb.End();
        }
    }
}
