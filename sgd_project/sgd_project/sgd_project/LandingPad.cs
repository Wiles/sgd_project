//File:     LandingPad.cs
//Name:     Samuel Lewis (5821103) & Thomas Kempton (5781000)
//Date:     2013-04-15
//Class:    Simulation and Game Development
//Ass:      Project
//
//Desc:     
//          Landing pad Entity
//

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sgd_project
{
    /// <summary>
    /// Landing pad Entity
    /// </summary>
    internal class LandingPad : IEntity
    {
        private Model _model;

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public Vector3 Position { get; private set; }

        #region IEntity Members

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <param name="input">The input.</param>
        public void Update(long delta, Input input)
        {
        }

        /// <summary>
        /// Draws the landing pad to the graphics
        /// </summary>
        /// <param name="graphics">The graphics.</param>
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
                        transforms[mesh.ParentBone.Index]*
                        Matrix.CreateTranslation(Position);
                    effect.View = camera;
                    effect.Projection = projection;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }

        /// <summary>
        /// Gets the collision bounds.
        /// </summary>
        /// <returns></returns>
        public IBound[] GetBounds()
        {
            return new IBound[]
                {
                    new BoundBox(new BoundingBox(Position - (new Vector3(7.5f, 15, 7.5f)*Lander.Metre),
                                                 Position - (new Vector3(-7.5f, 0, -7.5f)*Lander.Metre)))
                };
        }

        #endregion

        /// <summary>
        /// Inits the landing pad
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="model">The model.</param>
        public void Init(Vector3 position, Model model)
        {
            _model = model;
            Position = position;
        }
    }
}