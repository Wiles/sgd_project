using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace sgd_project
{
    internal class Lem
    {
        private Model _model;
        public Vector3 Position { get; private set; }
        private float _rotationZ;
        private float _rotationX;
        private Vector3 _gravity;
        private const float Rps = MathHelper.TwoPi;
        private Vector3 _velocity;
        public const float MinY = 1.78f;

        public Lem()
        {
        }

        public void Init(Vector3 position, Model model, Vector3 gravity)
        {
            _model = model;
            Position = position;
            _gravity = gravity;
        }

        public void Update(long delta, GamePadState gamePad, KeyboardState keyboard)
        {
            if(delta == 0)
            {
                return;
            }
            var timePercent = delta/1000f;
            _velocity += _gravity * timePercent;

            var thrust = new Vector3(0, gamePad.Triggers.Right * timePercent * 25f, 0);
            if(keyboard.IsKeyDown(Keys.Space)){
             thrust = new Vector3(0, timePercent * 25f, 0);
            } else {
             thrust = new Vector3(0, gamePad.Triggers.Right * timePercent * 25f, 0);
            
            }

            thrust = Vector3.Transform(thrust, Matrix.CreateFromAxisAngle(Vector3.UnitX, _rotationX));
            thrust = Vector3.Transform(thrust, Matrix.CreateFromAxisAngle(Vector3.UnitZ, _rotationZ));

            _velocity += thrust;

            Position += _velocity * timePercent;
            if (Position.Y <= MinY)
            {
                _velocity = Vector3.Zero;
                Position = new Vector3(Position.X, MinY, Position.Z);
            }

            if(keyboard.IsKeyDown(Keys.A)){
                _rotationZ += timePercent * Rps;

            }
            else if(keyboard.IsKeyDown(Keys.D)){
                _rotationZ -= timePercent * Rps;
            }
            else {
            
                _rotationZ += gamePad.ThumbSticks.Left.X * timePercent * Rps;
            }
            
            _rotationZ = MathHelper.Clamp(_rotationZ, -MathHelper.PiOver2, MathHelper.PiOver2);
            if(keyboard.IsKeyDown(Keys.W)){
                _rotationX += timePercent * Rps;

            }
            else if(keyboard.IsKeyDown(Keys.S)){
                _rotationX -= timePercent * Rps;
            }
            else {
            
                _rotationX += gamePad.ThumbSticks.Left.Y * timePercent * Rps;
            }
            _rotationX = MathHelper.Clamp(_rotationX, -MathHelper.PiOver2, MathHelper.PiOver2);
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
                        Matrix.CreateRotationZ(_rotationZ) *
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
