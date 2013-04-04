using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace sgd_project
{
    internal class Lem
    {
        private Model _model;
        private Model _flame;
        public Vector3 Position { get; private set; }
        public float RotationZ { get; private set; }
        public float RotationX { get; private set; }
        public Body Gravity { get; set; }
        private const float Rps = MathHelper.PiOver4;
        private const float MaxThrust = 15f;
        private Vector3 _velocity;
        public static readonly float MinY = 1.75f * Lander.Metre.Y;
        public float Fuel { get; private set; }
        public float _thrust;
        public float _thrustX;
        public float _thrustZ;

        public void Init(Vector3 position, Model model, Model flame,  Body gravity, float fuel)
        {
            _model = model;
            _flame = flame;
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
            _velocity += (Gravity.Gravity + Gravity.Wind) * timePercent;

            var thrust = Vector3.Zero;
            if(Fuel > 0)
            {
                if (keyboard.IsKeyDown(Keys.Space))
                {
                    _thrust = 1;
                    thrust = new Vector3(0, timePercent * MaxThrust, 0);
                    Fuel -= timePercent;
                }
                else
                {
                    
                    thrust = new Vector3(0, gamePad.Triggers.Right * timePercent * MaxThrust, 0);
                    _thrust = gamePad.Triggers.Right;
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
                    Fuel -= timePercent;
                    _thrustZ = -1;

                }
                else if(keyboard.IsKeyDown(Keys.A)){
                    RotationZ -= timePercent * Rps;
                    Fuel -= timePercent;
                    _thrustZ = 1;
                }
                else
                {
                    _thrustZ = gamePad.ThumbSticks.Left.X;
                    RotationZ += gamePad.ThumbSticks.Left.X * timePercent * Rps;
                    Fuel -= Math.Abs(gamePad.ThumbSticks.Left.X) * timePercent;
                }

                RotationZ = MathHelper.Clamp(RotationZ, -MathHelper.PiOver4, MathHelper.PiOver4);
                if(keyboard.IsKeyDown(Keys.W)){
                    RotationX += timePercent * Rps;
                    Fuel -= timePercent;
                    _thrustX = -1;

                }
                else if(keyboard.IsKeyDown(Keys.S)){
                    RotationX -= timePercent * Rps;
                    Fuel -= timePercent;
                    _thrustX = -1;
                }
                else {

                    RotationX += gamePad.ThumbSticks.Left.Y * timePercent * Rps;
                    Fuel -= Math.Abs(gamePad.ThumbSticks.Left.Y) * timePercent;
                    _thrustX = gamePad.ThumbSticks.Left.Y;
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
                    effect.World =
                        Matrix.CreateRotationZ(MathHelper.ToRadians(45f)) *
                        transforms[mesh.ParentBone.Index] *
                        Matrix.CreateRotationZ(RotationZ) *
                        Matrix.CreateRotationX(RotationX) *
                        Matrix.CreateTranslation(Position);
                    effect.View = camera;
                    effect.Projection = projection;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }

            transforms = new Matrix[_flame.Bones.Count];
            _flame.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (var mesh in _flame.Meshes)
            {
                var m =  
                    Matrix.CreateFromAxisAngle(Vector3.UnitZ, RotationZ) * 
                    Matrix.CreateFromAxisAngle(Vector3.UnitX, RotationX);
                var pos = Vector3.Transform(new Vector3(0, -1.5f, 0) * Lander.Metre, m);

                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World =
                        Matrix.CreateScale(_thrust) * 
                        transforms[mesh.ParentBone.Index] *

                        Matrix.CreateRotationZ(RotationZ) *
                        Matrix.CreateRotationX(RotationX) *
                        Matrix.CreateTranslation(Position + pos) 
                        ;
                    effect.View = camera;
                    effect.Projection = projection;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
            if (_thrustZ > 0)
            {
                foreach (var mesh in _flame.Meshes)
                {
                    var m =
                        Matrix.CreateFromAxisAngle(Vector3.UnitZ, RotationZ) *
                        Matrix.CreateFromAxisAngle(Vector3.UnitX, RotationX);
                    var pos = Vector3.Transform(new Vector3(1.3f, .35f, -0.05f) * Lander.Metre, m);

                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World =
                            Matrix.CreateScale(_thrustZ * .2f) *
                            transforms[mesh.ParentBone.Index] *

                            Matrix.CreateRotationZ(RotationZ) *
                            Matrix.CreateRotationX(RotationX) *
                            Matrix.CreateTranslation(Position + pos)
                            ;
                        effect.View = camera;
                        effect.Projection = projection;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }


                foreach (var mesh in _flame.Meshes)
                {
                    var m =
                        Matrix.CreateFromAxisAngle(Vector3.UnitZ, RotationZ) *
                        Matrix.CreateFromAxisAngle(Vector3.UnitX, RotationX);
                    var pos = Vector3.Transform(new Vector3(-1.3f, 1.0f, 0.1f) * Lander.Metre, m);

                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World =
                            Matrix.CreateScale(_thrustZ * -.2f) *
                            transforms[mesh.ParentBone.Index] *

                            Matrix.CreateRotationZ(RotationZ) *
                            Matrix.CreateRotationX(RotationX) *
                            Matrix.CreateTranslation(Position + pos)
                            ;
                        effect.View = camera;
                        effect.Projection = projection;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }

            }
            else
            {
                foreach (var mesh in _flame.Meshes)
                {
                    var m =
                        Matrix.CreateFromAxisAngle(Vector3.UnitZ, RotationZ) *
                        Matrix.CreateFromAxisAngle(Vector3.UnitX, RotationX);
                    var pos = Vector3.Transform(new Vector3(1.3f, 1f, -0.05f) * Lander.Metre, m);

                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World =
                            Matrix.CreateScale(_thrustZ * .2f) *
                            transforms[mesh.ParentBone.Index] *

                            Matrix.CreateRotationZ(RotationZ) *
                            Matrix.CreateRotationX(RotationX) *
                            Matrix.CreateTranslation(Position + pos)
                            ;
                        effect.View = camera;
                        effect.Projection = projection;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }


                foreach (var mesh in _flame.Meshes)
                {
                    var m =
                        Matrix.CreateFromAxisAngle(Vector3.UnitZ, RotationZ) *
                        Matrix.CreateFromAxisAngle(Vector3.UnitX, RotationX);
                    var pos = Vector3.Transform(new Vector3(-1.3f, .35f, 0.1f) * Lander.Metre, m);

                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World =
                            Matrix.CreateScale(_thrustZ * -.2f) *
                            transforms[mesh.ParentBone.Index] *

                            Matrix.CreateRotationZ(RotationZ) *
                            Matrix.CreateRotationX(RotationX) *
                            Matrix.CreateTranslation(Position + pos)
                            ;
                        effect.View = camera;
                        effect.Projection = projection;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }

            }
            if (_thrustX < 0)
            {
                foreach (var mesh in _flame.Meshes)
                {
                    var m =
                        Matrix.CreateFromAxisAngle(Vector3.UnitZ, RotationZ) *
                        Matrix.CreateFromAxisAngle(Vector3.UnitX, RotationX);
                    var pos = Vector3.Transform(new Vector3(-0.05f, .35f, 1.3f) * Lander.Metre, m);

                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World =
                            Matrix.CreateScale(_thrustX * -.2f) *
                            transforms[mesh.ParentBone.Index] *

                            Matrix.CreateRotationZ(RotationZ) *
                            Matrix.CreateRotationX(RotationX) *
                            Matrix.CreateTranslation(Position + pos)
                            ;
                        effect.View = camera;
                        effect.Projection = projection;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }


                foreach (var mesh in _flame.Meshes)
                {
                    var m =
                        Matrix.CreateFromAxisAngle(Vector3.UnitZ, RotationZ) *
                        Matrix.CreateFromAxisAngle(Vector3.UnitX, RotationX);
                    var pos = Vector3.Transform(new Vector3(0.1f, 1.0f, -1.3f) * Lander.Metre, m);

                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World =
                            Matrix.CreateScale(_thrustX * .2f) *
                            transforms[mesh.ParentBone.Index] *

                            Matrix.CreateRotationZ(RotationZ) *
                            Matrix.CreateRotationX(RotationX) *
                            Matrix.CreateTranslation(Position + pos)
                            ;
                        effect.View = camera;
                        effect.Projection = projection;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }

            }
            else
            {
                foreach (var mesh in _flame.Meshes)
                {
                    var m =
                        Matrix.CreateFromAxisAngle(Vector3.UnitZ, RotationZ) *
                        Matrix.CreateFromAxisAngle(Vector3.UnitX, RotationX);
                    var pos = Vector3.Transform(new Vector3(-0.05f, 1f, 1.3f) * Lander.Metre, m);

                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World =
                            Matrix.CreateScale(_thrustX * -.2f) *
                            transforms[mesh.ParentBone.Index] *

                            Matrix.CreateRotationZ(RotationZ) *
                            Matrix.CreateRotationX(RotationX) *
                            Matrix.CreateTranslation(Position + pos)
                            ;
                        effect.View = camera;
                        effect.Projection = projection;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }


                foreach (var mesh in _flame.Meshes)
                {
                    var m =
                        Matrix.CreateFromAxisAngle(Vector3.UnitZ, RotationZ) *
                        Matrix.CreateFromAxisAngle(Vector3.UnitX, RotationX);
                    var pos = Vector3.Transform(new Vector3(0.1f, .35f, -1.3f) * Lander.Metre, m);

                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World =
                            Matrix.CreateScale(_thrustX * .2f) *
                            transforms[mesh.ParentBone.Index] *

                            Matrix.CreateRotationZ(RotationZ) *
                            Matrix.CreateRotationX(RotationX) *
                            Matrix.CreateTranslation(Position + pos)
                            ;
                        effect.View = camera;
                        effect.Projection = projection;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }

            }
        }
    }
}
