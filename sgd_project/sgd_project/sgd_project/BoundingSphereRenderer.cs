//File:     BoundingSphereRenderer.cs
//Name:     Samuel Lewis (5821103) & Thomas Kempton (5781000)
//Date:     2013-04-15
//Class:    Simulation and Game Development
//Ass:      Project
//
//Desc:     
//          Renders a BoundSphere to the display
//

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sgd_project
{
    /// <summary>
    /// Renders a BoundSphere to the display
    /// </summary>
    internal class BoundingSphereRenderer
    {
        private static EffectParameter _positionColorEffectWvp;
        private static Effect _positionColorEffect;

        /// <summary>
        /// Inits the Renderer this must be called once before the Render function can be called
        /// </summary>
        /// <param name="positionColorEffectWvp">The position color effect WVP.</param>
        /// <param name="positionColorEffect">The position color effect.</param>
        public static void Init(EffectParameter positionColorEffectWvp, Effect positionColorEffect)
        {
            _positionColorEffectWvp = positionColorEffectWvp;
            _positionColorEffect = positionColorEffect;
        }

        /// <summary>
        /// Renders the sphere to the graphics device
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="sphere">The sphere.</param>
        /// <param name="view">The view.</param>
        /// <param name="projection">The projection.</param>
        public static void Render(GraphicsDevice graphics, BoundSphere sphere, Matrix view, Matrix projection)
        {
            Matrix world = Matrix.Identity;

            // 4: set variables in shader
            _positionColorEffectWvp.SetValue(world
                                             *view*projection);

            // 5: draw object - primitive type, vertex data, # primitives
            PositionColorShader(
                graphics,
                PrimitiveType.LineStrip,
                sphere.Vertices,
                sphere.PrimativeCount);
        }

        private static void PositionColorShader(GraphicsDevice graphics,
                                                PrimitiveType primitiveType,
                                                VertexPositionColor[] vertexData,
                                                int numPrimitives)
        {
            _positionColorEffect.Techniques[0].Passes[0].Apply();

            // set drawing format and vertex data then draw primitive surface
            graphics.DrawUserPrimitives(
                primitiveType, vertexData, 0, numPrimitives);
        }
    }
}