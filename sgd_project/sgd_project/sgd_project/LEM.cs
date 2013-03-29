using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace sgd_project
{
    internal class Lem
    {
        private Model _model;
        public Vector3 Position { get; private set; }
        public float RotationZ { get; private set; }
        public float RotationX { get; private set; }
        private Vector3 _gravity;
        private const float Rps = MathHelper.PiOver4;
        private const float MaxThrust = 15f;
        private Vector3 _velocity;
        public static readonly float MinY = 1.75f * Lander.Metre.Y;
        public float Fuel { get; private set; }

        public Lem()
        {
        }

        public void Init(Vector3 position, Model model, Vector3 gravity, float fuel)
        {
            _model = model;
            Position = position;
            _gravity = gravity;
            Fuel = fuel;
        }

        public void Update(long delta, GamePadState gamePad)
        {
            if(delta == 0)
            {
                return;
            }
            var timePercent = delta/1000f;
            _velocity += _gravity * timePercent;

            Fuel -= gamePad.Triggers.Right*timePercent;
            var thrust = Vector3.Zero;
            if(Fuel > 0)
            {
                thrust = new Vector3(0, gamePad.Triggers.Right * timePercent * MaxThrust, 0);

                thrust = Vector3.Transform(thrust, Matrix.CreateFromAxisAngle(Vector3.UnitX, RotationX));
                thrust = Vector3.Transform(thrust, Matrix.CreateFromAxisAngle(Vector3.UnitZ, RotationZ));   
            }

            _velocity += thrust;

            Position += _velocity*Lander.Metre*timePercent;

            if (Position.Y <= MinY)
            {
                _velocity = Vector3.Zero;
                Position = new Vector3(Position.X, MinY, Position.Z);
            }

            if(Fuel > 0)
            {
                RotationZ -= gamePad.ThumbSticks.Left.X * timePercent * Rps;
                RotationZ = MathHelper.Clamp(RotationZ, -MathHelper.PiOver2, MathHelper.PiOver2);
                RotationX -= gamePad.ThumbSticks.Left.Y * timePercent * Rps;
                RotationX = MathHelper.Clamp(RotationX, -MathHelper.PiOver2, MathHelper.PiOver2);
            }


            Fuel -= Math.Abs(gamePad.ThumbSticks.Left.X) * timePercent;
            Fuel -= Math.Abs(gamePad.ThumbSticks.Left.Y) * timePercent;
            if(Fuel < 0)
            {
                Fuel = 0;
            }
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
                        Matrix.CreateRotationZ(RotationZ) *
                        Matrix.CreateRotationX(RotationX) *
                        Matrix.CreateTranslation(Position);
                    effect.View = camera;
                    effect.Projection = projection;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }
    }
}
