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
        public Vector3 Gravity { get; set; }
        private const float Rps = MathHelper.PiOver4;
        private const float MaxThrust = 15f;
        private Vector3 _velocity;
        public static readonly float MinY = 1.75f * Lander.Metre.Y;
        public float Fuel { get; private set; }
        
        public void Init(Vector3 position, Model model, Vector3 gravity, float fuel)
        {
            _model = model;
            Position = position;
            Gravity = gravity;
            Fuel = fuel;
        }

        public void Update(long delta, GamePadState gamePad, KeyboardState keyboard)
        {
            if(delta == 0)
            {
                return;
            }
            var timePercent = delta/1000f;
            _velocity += Gravity * timePercent;

            var thrust = Vector3.Zero;
            if(Fuel > 0)
            {
                if (keyboard.IsKeyDown(Keys.Space))
                {
                    thrust = new Vector3(0, timePercent * MaxThrust, 0);
                    Fuel -= timePercent;
                }
                else
                {
                    thrust = new Vector3(0, gamePad.Triggers.Right * timePercent * MaxThrust, 0);
                    Fuel -= gamePad.Triggers.Right * timePercent;
                }

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
                if(keyboard.IsKeyDown(Keys.D)){
                    RotationZ += timePercent * Rps;

                }
                else if(keyboard.IsKeyDown(Keys.A)){
                    RotationZ -= timePercent * Rps;
                    Fuel -= timePercent;
                }
                else {

                    RotationZ += gamePad.ThumbSticks.Left.X * timePercent * Rps;
                    Fuel -= Math.Abs(gamePad.ThumbSticks.Left.X) * timePercent;
                }

                RotationZ = MathHelper.Clamp(RotationZ, -MathHelper.PiOver4, MathHelper.PiOver4);
                if(keyboard.IsKeyDown(Keys.W)){
                    RotationX += timePercent * Rps;

                }
                else if(keyboard.IsKeyDown(Keys.S)){
                    RotationX -= timePercent * Rps;
                    Fuel -= timePercent;
                }
                else {

                    RotationX += gamePad.ThumbSticks.Left.Y * timePercent * Rps;
                    Fuel -= Math.Abs(gamePad.ThumbSticks.Left.Y) * timePercent;
                }
                RotationX = MathHelper.Clamp(RotationX, -MathHelper.PiOver4, MathHelper.PiOver4);
            }


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
