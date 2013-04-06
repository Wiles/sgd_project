using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sgd_project
{
    public class Ground
    {
        private Effect _textureEffect;
        private EffectParameter _textureEffectWvp;
        private EffectParameter _textureEffectImage;
        private Texture _grassTexture;
        private VertexPositionColorTexture[] _groundVertices;

        public Ground()
        {
        }


        public void Init(Effect textureEffect, EffectParameter textureEffectWvp, EffectParameter textureEffectImage, Texture grassTexture, VertexPositionColorTexture[] groundVertecies)
        {
            _textureEffect = textureEffect;
            _textureEffectWvp = textureEffectWvp;
            _textureEffectImage = textureEffectImage;
            _grassTexture = grassTexture;
            _groundVertices = groundVertecies;
        }

        public void Update()
        {
            
        }

        public void Draw(GraphicsDevice device, Matrix view, Matrix projection)
        {
            // 1: declare matrices

            // 2: initialize matrices
            Matrix translation = Matrix.CreateTranslation(0.0f, 0.0f, 0.0f);

            // 3: build cumulative world matrix using I.S.R.O.T. sequence
            // identity, scale, rotate, orbit(translate & rotate), translate
            Matrix world = translation;

            // 4: set shader parameters
            _textureEffectWvp.SetValue(world * view * projection);
            _textureEffectImage.SetValue(_grassTexture);

            // 5: draw object - primitive type, vertex data, # primitives
            TextureShader(device, PrimitiveType.TriangleStrip, _groundVertices, 2);
            
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

        public IBound[] GetBounds()
        {
            return new IBound[]
                {
                    new BoundBox(new BoundingBox(new Vector3(-float.MaxValue, -1, -float.MaxValue), new Vector3(float.MaxValue, 1, float.MaxValue))), 
                };
        }
    }
}
