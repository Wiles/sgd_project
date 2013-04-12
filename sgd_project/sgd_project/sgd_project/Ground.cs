//File:     Ground.cs
//Name:     Samuel Lewis (5821103) & Thomas Kempton (5781000)
//Date:     2013-04-15
//Class:    Simulation and Game Development
//Ass:      Project
//
//Desc:     
//          Ground entity
//

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sgd_project
{
    /// <summary>
    /// Ground Entity
    /// </summary>
    public class Ground : IEntity
    {
        private Texture _grassTexture;
        private VertexPositionColorTexture[] _groundVertices;
        private Effect _textureEffect;
        private EffectParameter _textureEffectImage;
        private EffectParameter _textureEffectWvp;

        #region IEntity Members

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public void Update(long delta, Input input)
        {
        }

        /// <summary>
        /// Draws the Ground
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="view">The view.</param>
        /// <param name="projection">The projection.</param>
        public void Draw(GraphicsDevice device, Matrix view, Matrix projection)
        {
            // 1: declare matrices

            // 2: initialize matrices
            Matrix translation = Matrix.CreateTranslation(0.0f, 0.0f, 0.0f);

            // 3: build cumulative world matrix using I.S.R.O.T. sequence
            // identity, scale, rotate, orbit(translate & rotate), translate
            Matrix world = translation;

            // 4: set shader parameters
            _textureEffectWvp.SetValue(world*view*projection);
            _textureEffectImage.SetValue(_grassTexture);

            // 5: draw object - primitive type, vertex data, # primitives
            TextureShader(device, PrimitiveType.TriangleStrip, _groundVertices, 2);
        }

        /// <summary>
        /// Gets the bounds.
        /// </summary>
        /// <returns></returns>
        public IBound[] GetBounds()
        {
            return new IBound[]
                {
                    new BoundBox(new BoundingBox(new Vector3(-float.MaxValue, -1, -float.MaxValue),
                                                 new Vector3(float.MaxValue, 1, float.MaxValue)))
                };
        }

        #endregion

        /// <summary>
        /// Inits the Ground entity
        /// </summary>
        /// <param name="textureEffect">The texture effect.</param>
        /// <param name="textureEffectWvp">The texture effect WVP.</param>
        /// <param name="textureEffectImage">The texture effect image.</param>
        /// <param name="grassTexture">The grass texture.</param>
        /// <param name="groundVertecies">The ground vertecies.</param>
        public void Init(Effect textureEffect, EffectParameter textureEffectWvp, EffectParameter textureEffectImage,
                         Texture grassTexture, VertexPositionColorTexture[] groundVertecies)
        {
            _textureEffect = textureEffect;
            _textureEffectWvp = textureEffectWvp;
            _textureEffectImage = textureEffectImage;
            _grassTexture = grassTexture;
            _groundVertices = groundVertecies;
        }

        private void TextureShader(GraphicsDevice device,
                                   PrimitiveType primitiveType,
                                   VertexPositionColorTexture[] vertexData,
                                   int numPrimitives)
        {
            _textureEffect.Techniques[0].Passes[0].Apply();

            // set drawing format and vertex data then draw surface
            device.DrawUserPrimitives(
                primitiveType, vertexData, 0, numPrimitives);
        }
    }
}