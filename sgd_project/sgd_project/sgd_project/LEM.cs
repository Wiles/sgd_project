using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace sgd_project
{
    class LEM
    {
        private Model _model;
        public Vector3 Position { get; private set; }
        private float _rotationY;
        private float _rotationX;
        private const float RPS = MathHelper.TwoPi;

        public LEM()
        {
        }

        public void Init(Vector3 position, Model model)
        {
            _model = model;
            Position = position;
        }

        public void Update(long delta, GamePadState gamePad)
        {
            var timePercent = delta/1000.0;
            _rotationY += (float)(gamePad.ThumbSticks.Left.X * timePercent * RPS);
            _rotationY = MathHelper.Clamp(_rotationY, -MathHelper.PiOver2, MathHelper.PiOver2);
            _rotationX += (float)(gamePad.ThumbSticks.Left.Y * timePercent * RPS);
        }

        public void Draw(Matrix camera, Matrix projection)
        {
            var transforms = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (var mesh in _model.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] *
                        Matrix.CreateRotationY(_rotationY) *
                        Matrix.CreateRotationX(_rotationX)
                        * Matrix.CreateTranslation(Position);
                    effect.View = camera;
                    effect.Projection = projection;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }
    }
}
