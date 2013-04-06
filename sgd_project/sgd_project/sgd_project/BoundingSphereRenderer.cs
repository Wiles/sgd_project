using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sgd_project
{
    class BoundingSphereRenderer
    {
        private static EffectParameter _positionColorEffectWvp;
        private static Effect _positionColorEffect;
        public static void Init(EffectParameter positionColorEffectWvp, Effect positionColorEffect)
        {
            _positionColorEffectWvp = positionColorEffectWvp;
            _positionColorEffect = positionColorEffect;
        }

        public static void Render(GraphicsDevice graphics, BoundSphere sphere, Matrix view, Matrix projection)
        {
            Matrix world;

            world = Matrix.Identity;

            // 4: set variables in shader
            _positionColorEffectWvp.SetValue(world
                                            * view * projection);

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
