// Author: Mark Voong
// File Name: ShaderRenderer.cs
// Project Name: Global Offensive
// Creation Date: Jan 11th, 2016
// Modified Date: Jan 11th, 2016
// Description: Houses all of the shaders present in this game. Allows for a scene to be
//              post-processed and drawn to the screen via 2 methods for easy use
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CStrike2D
{
    public class ShaderRenderer
    {
        private CStrike2D driver;
        private RenderTarget2D sceneRender;
        private RenderTarget2D firstRender;
        private RenderTarget2D secondRender;
        public float BlurAmount { get; private set; }
        private Effect blurEffect;

        public ShaderRenderer(CStrike2D instance)
        {
            driver = instance;
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
        }

        public void ChangeBlurAmount(float amount)
        {
            BlurAmount = amount;
        }

        public void Load()
        {
            blurEffect = driver.Assets.BlurEffect;
        }

        public void BeginRender()
        {
            driver.GraphicsDevice.SetRenderTarget(sceneRender);
            driver.GraphicsDevice.Clear(Color.Transparent);
        }

        public void Draw(SpriteBatch sb)
        {
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

        private void DrawScreen(SpriteBatch sb, Texture2D texture, RenderTarget2D render, Effect effect)
        {
            driver.GraphicsDevice.SetRenderTarget(render);

            DrawScreen(sb, texture,
                render.Width, render.Height, effect);
        }

        private void DrawScreen(SpriteBatch sb, Texture2D texture, float width, float height,
                Effect effect)
        {
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, effect);

            sb.Draw(texture, new Rectangle(0, 0, (int)width, (int)height), Color.White);

            sb.End();
        }

        /// <summary>
        /// Computes sample weightings and texture coordinate offsets
        /// for one pass of a separable gaussian blur filter.
        /// </summary>
        private void SetBlurEffectParameters(float dx, float dy)
        {
            // Look up the sample weight and offset effect parameters.
            EffectParameter weightsParameter, offsetsParameter;

            weightsParameter = blurEffect.Parameters["SampleWeights"];
            offsetsParameter = blurEffect.Parameters["SampleOffsets"];

            // Look up how many samples our gaussian blur effect supports.
            int sampleCount = weightsParameter.Elements.Count;

            // Create temporary arrays for computing our filter settings.
            float[] sampleWeights = new float[sampleCount];
            Vector2[] sampleOffsets = new Vector2[sampleCount];

            // The first sample always has a zero offset.
            sampleWeights[0] = ComputeGaussian(0);
            sampleOffsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            float totalWeights = sampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussian(i + 1);

                sampleWeights[i * 2 + 1] = weight;
                sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                // To get the maximum amount of blurring from a limited number of
                // pixel shader samples, we take advantage of the bilinear filtering
                // hardware inside the texture fetch unit. If we position our texture
                // coordinates exactly halfway between two texels, the filtering unit
                // will average them for us, giving two samples for the price of one.
                // This allows us to step in units of two texels per sample, rather
                // than just one at a time. The 1.5 offset kicks things off by
                // positioning us nicely in between two texels.
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

            // Tell the effect about our new filter settings.
            weightsParameter.SetValue(sampleWeights);
            offsetsParameter.SetValue(sampleOffsets);
        }


        /// <summary>
        /// Evaluates a single point on the gaussian falloff curve.
        /// Used for setting up the blur filter weightings.
        /// </summary>
        float ComputeGaussian(float n)
        {
            float theta = BlurAmount;

            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                           Math.Exp(-(n * n) / (2 * theta * theta)));
        }
    }
}
