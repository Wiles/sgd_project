//File:     LEM.cs
//Name:     Samuel Lewis (5821103) & Thomas Kempton (5781000)
//Date:     2013-04-15
//Class:    Simulation and Game Development
//Ass:      Project
//
//Desc:     
//          Lunar Excursion Module
//

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace sgd_project
{
    /// <summary>
    /// 
    /// </summary>
    internal class Lem : IEntity, IDisposable
    {
        /// <summary>
        /// The maximum rotations per second
        /// </summary>
        public float Rps {get; set;}

        /// <summary>
        /// The max thrust
        /// </summary>
        public float MaxThrust { get; set; }

        /// <summary>
        /// The min Y
        /// </summary>
        public static readonly float MinY = 1.75f*Lander.Metre.Y;

        private Model _flame;
        private Model _model;
        private float _thrust;
        private float _thrustX;
        private float _thrustZ;

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the rotation Z.
        /// </summary>
        /// <value>
        /// The rotation Z.
        /// </value>
        public float RotationZ { get; set; }

        /// <summary>
        /// Gets or sets the rotation X.
        /// </summary>
        /// <value>
        /// The rotation X.
        /// </value>
        public float RotationX { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public Body Body { get; set; }

        /// <summary>
        /// Gets or sets the velocity.
        /// </summary>
        /// <value>
        /// The velocity.
        /// </value>
        public Vector3 Velocity { get; set; }

        /// <summary>
        /// Gets the fuel.
        /// </summary>
        /// <value>
        /// The fuel.
        /// </value>
        public float Fuel { get; private set; }

        private SoundEffectInstance _engine;
        private AudioListener _listener;

        /// <summary>
        /// Inits the Lunar Excursion Module
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="model">The model.</param>
        /// <param name="flame">The flame.</param>
        /// <param name="gravity">The gravity.</param>
        /// <param name="fuel">The fuel.</param>
        /// <param name="engine">The engine sound effect </param>
        public void Init(Vector3 position, Model model, Model flame, Body gravity, float fuel, SoundEffect engine, AudioListener listener)
        {
            MaxThrust = 5f;
            Rps = MathHelper.PiOver4/2;
            _model = model;
            _flame = flame;
            Position = position;
            Body = gravity;
            Fuel = fuel;
            _engine = engine.CreateInstance();
            _engine.Volume = 0;
            _engine.IsLooped = true;
            _engine.Apply3D(listener, new AudioEmitter());
            _engine.Play();
            _listener = listener;
        }

        /// <summary>
        /// Updates the Lunar Excursion Module
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <param name="input">The input.</param>
        public void Update(long delta, Input input)
        {
            _thrustZ = 0;
            _thrustX = 0;
            _thrust = 0;

            if (delta == 0)
            {
                return;
            }
            float timePercent = delta/1000f;
            Velocity += (Body.Gravity + Body.Wind)*timePercent;

            _engine.Volume = 0;
            Vector3 thrust = Vector3.Zero;
            if (Fuel > 0)
            {
                thrust = new Vector3(0, input.Thrust()*timePercent*MaxThrust, 0);
                _thrust = input.Thrust();
                Fuel -= Math.Abs(input.Thrust()*timePercent);

                thrust = Vector3.Transform(thrust, Matrix.CreateFromAxisAngle(Vector3.UnitX, RotationX));
                thrust = Vector3.Transform(thrust, Matrix.CreateFromAxisAngle(Vector3.UnitZ, RotationZ));
            }

            Velocity += thrust;

            Position += Velocity*Lander.Metre*timePercent;

            if (Position.Y <= MinY)
            {
                Velocity = Vector3.Zero;
                Position = new Vector3(Position.X, MinY, Position.Z);
            }

            if (Fuel > 0)
            {
                _thrustZ = input.RotationZ();
                RotationZ += _thrustZ*timePercent*Rps;
                Fuel -= Math.Abs(_thrustZ)*timePercent;

                RotationZ = MathHelper.Clamp(RotationZ, -MathHelper.PiOver4, MathHelper.PiOver4);
                _thrustX = input.RotationX();
                RotationX += _thrustX*timePercent*Rps;
                Fuel -= Math.Abs(_thrustX)*timePercent;
                RotationX = MathHelper.Clamp(RotationX, -MathHelper.PiOver4, MathHelper.PiOver4);
            }
            if (Fuel < 0)
            {
                Fuel = 0;
            }
        }

        /// <summary>
        /// Draws the Lunar Excursion Module to the graphcis device
        /// </summary>
        /// <param name="graphics">graphics device </param>
        /// <param name="camera">The camera.</param>
        /// <param name="projection">The projection.</param>
        public void Draw(GraphicsDevice graphics, Matrix camera, Matrix projection)
        {
            var transforms = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in _model.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World =
                        Matrix.CreateRotationZ(MathHelper.ToRadians(45f))*
                        transforms[mesh.ParentBone.Index]*
                        Matrix.CreateRotationZ(RotationZ)*
                        Matrix.CreateRotationX(RotationX)*
                        Matrix.CreateTranslation(Position);
                    effect.View = camera;
                    effect.Projection = projection;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }

            transforms = new Matrix[_flame.Bones.Count];
            _flame.CopyAbsoluteBoneTransformsTo(transforms);


            Matrix m =
                Matrix.CreateFromAxisAngle(Vector3.UnitZ, RotationZ) *
                Matrix.CreateFromAxisAngle(Vector3.UnitX, RotationX);
            Vector3 pos = Position + Vector3.Transform(new Vector3(0, -1.5f, 0) * Lander.Metre, m);


            var emitter = new AudioEmitter();
            emitter.Position = pos;
            _engine.Apply3D(_listener, emitter);
            if (_thrust > 0)
            {
                _engine.Volume = (float)(.5 + (_thrust * .5));
            }


            foreach (ModelMesh mesh in _flame.Meshes)
            {
                pos = Position + Vector3.Transform(new Vector3(0, -1.5f, 0) * Lander.Metre, m);

                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World =
                        Matrix.CreateScale(_thrust)*
                        transforms[mesh.ParentBone.Index]*
                        Matrix.CreateRotationZ(RotationZ)*
                        Matrix.CreateRotationX(RotationX)*
                        Matrix.CreateTranslation( pos)
                        ;
                    effect.View = camera;
                    effect.Projection = projection;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
            if (_thrustZ > 0)
            {
                foreach (ModelMesh mesh in _flame.Meshes)
                {
                    pos = Vector3.Transform(new Vector3(1.3f, .35f, -0.05f)*Lander.Metre, m);

                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World =
                            Matrix.CreateScale(_thrustZ*.2f)*
                            transforms[mesh.ParentBone.Index]*
                            Matrix.CreateRotationZ(RotationZ)*
                            Matrix.CreateRotationX(RotationX)*
                            Matrix.CreateTranslation(Position + pos)
                            ;
                        effect.View = camera;
                        effect.Projection = projection;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }


                foreach (ModelMesh mesh in _flame.Meshes)
                {
                    pos = Vector3.Transform(new Vector3(-1.3f, 1.0f, 0.1f)*Lander.Metre, m);

                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World =
                            Matrix.CreateScale(_thrustZ*-.2f)*
                            transforms[mesh.ParentBone.Index]*
                            Matrix.CreateRotationZ(RotationZ)*
                            Matrix.CreateRotationX(RotationX)*
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
                foreach (ModelMesh mesh in _flame.Meshes)
                {
                    pos = Vector3.Transform(new Vector3(1.3f, 1f, -0.05f)*Lander.Metre, m);

                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World =
                            Matrix.CreateScale(_thrustZ*.2f)*
                            transforms[mesh.ParentBone.Index]*
                            Matrix.CreateRotationZ(RotationZ)*
                            Matrix.CreateRotationX(RotationX)*
                            Matrix.CreateTranslation(Position + pos)
                            ;
                        effect.View = camera;
                        effect.Projection = projection;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }


                foreach (ModelMesh mesh in _flame.Meshes)
                {
                    pos = Vector3.Transform(new Vector3(-1.3f, .35f, 0.1f)*Lander.Metre, m);

                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World =
                            Matrix.CreateScale(_thrustZ*-.2f)*
                            transforms[mesh.ParentBone.Index]*
                            Matrix.CreateRotationZ(RotationZ)*
                            Matrix.CreateRotationX(RotationX)*
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
                foreach (ModelMesh mesh in _flame.Meshes)
                {
                    pos = Vector3.Transform(new Vector3(-0.05f, .35f, 1.3f)*Lander.Metre, m);

                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World =
                            Matrix.CreateScale(_thrustX*-.2f)*
                            transforms[mesh.ParentBone.Index]*
                            Matrix.CreateRotationZ(RotationZ)*
                            Matrix.CreateRotationX(RotationX)*
                            Matrix.CreateTranslation(Position + pos)
                            ;
                        effect.View = camera;
                        effect.Projection = projection;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }


                foreach (ModelMesh mesh in _flame.Meshes)
                {
                    pos = Vector3.Transform(new Vector3(0.1f, 1.0f, -1.3f)*Lander.Metre, m);

                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World =
                            Matrix.CreateScale(_thrustX*.2f)*
                            transforms[mesh.ParentBone.Index]*
                            Matrix.CreateRotationZ(RotationZ)*
                            Matrix.CreateRotationX(RotationX)*
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
                foreach (ModelMesh mesh in _flame.Meshes)
                {
                    pos = Vector3.Transform(new Vector3(-0.05f, 1f, 1.3f)*Lander.Metre, m);

                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World =
                            Matrix.CreateScale(_thrustX*-.2f)*
                            transforms[mesh.ParentBone.Index]*
                            Matrix.CreateRotationZ(RotationZ)*
                            Matrix.CreateRotationX(RotationX)*
                            Matrix.CreateTranslation(Position + pos)
                            ;
                        effect.View = camera;
                        effect.Projection = projection;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }


                foreach (ModelMesh mesh in _flame.Meshes)
                {
                    pos = Vector3.Transform(new Vector3(0.1f, .35f, -1.3f)*Lander.Metre, m);

                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World =
                            Matrix.CreateScale(_thrustX*.2f)*
                            transforms[mesh.ParentBone.Index]*
                            Matrix.CreateRotationZ(RotationZ)*
                            Matrix.CreateRotationX(RotationX)*
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

        /// <summary>
        /// Gets the bounds.
        /// </summary>
        /// <returns></returns>
        public IBound[] GetBounds()
        {
            Matrix m =
                Matrix.CreateFromAxisAngle(Vector3.UnitZ, RotationZ)*
                Matrix.CreateFromAxisAngle(Vector3.UnitX, RotationX);
            return new IBound[]
                {
                    new BoundSphere(
                        new BoundingSphere(
                            Position + Vector3.Transform((new Vector3(1.47f, -1.675f, 1.47f)*Lander.Metre), m), 5f)),
                    new BoundSphere(
                        new BoundingSphere(
                            Position + Vector3.Transform((new Vector3(-1.47f, -1.675f, -1.47f)*Lander.Metre), m), 5f)),
                    new BoundSphere(
                        new BoundingSphere(
                            Position + Vector3.Transform((new Vector3(-1.47f, -1.675f, 1.47f)*Lander.Metre), m), 5f)),
                    new BoundSphere(
                        new BoundingSphere(
                            Position + Vector3.Transform((new Vector3(1.47f, -1.675f, -1.47f)*Lander.Metre), m), 5f))
                };
        }

        public void Dispose()
        {
            _engine.Dispose();
        }
    }
}