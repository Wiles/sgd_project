//File:     BoundBox.cs
//Name:     Samuel Lewis (5821103) & Thomas Kempton (5781000)
//Date:     2013-04-15
//Class:    Simulation and Game Development
//Ass:      Project
//
//Desc:     
//          Wrapper for the BoundingBox object
//

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sgd_project
{
    /// <summary>
    /// Wrapper for the BoundingBox object
    /// </summary>
    public class BoundBox : IBound
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoundBox"/> class.
        /// </summary>
        /// <param name="box">The box.</param>
        public BoundBox(BoundingBox box)
        {
            Box = box;
            Vector3[] corners = box.GetCorners();
            Vertices = new VertexPositionColor[corners.Length];


            // Assign the 8 box vertices
            for (int i = 0; i < corners.Length; i++)
            {
                Vertices[i] = new VertexPositionColor(corners[i], Color.Green);
            }
        }

        /// <summary>
        /// Gets the box.
        /// </summary>
        /// <value>
        /// The box.
        /// </value>
        public BoundingBox Box { get; private set; }

        /// <summary>
        /// Gets the vertices.
        /// </summary>
        /// <value>
        /// The vertices.
        /// </value>
        public VertexPositionColor[] Vertices { get; private set; }

        #region IBound Members

        /// <summary>
        /// Checks if the two bounds intersect eachother
        /// </summary>
        /// <param name="that">The other set of bounds</param>
        /// <returns>
        /// True if they intersect
        /// </returns>
        public bool Intersects(IBound that)
        {
            if (that is BoundBox)
            {
                return Box.Intersects((that as BoundBox).Box);
            }
            if (that is BoundSphere)
            {
                return Box.Intersects((that as BoundSphere).Sphere);
            }

            return false;
        }

        /// <summary>
        /// Draws the collision object.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="view">The view.</param>
        /// <param name="projection">The projection.</param>
        public void Draw(GraphicsDevice graphicsDevice, Matrix view, Matrix projection)
        {
            BoundingBoxRenderer.Render(this, graphicsDevice, view, projection, Color.Red);
        }

        /// <summary>
        /// Minimum point on the body
        /// </summary>
        /// <returns></returns>
        public Vector3 Min()
        {
            return Box.Min;
        }

        /// <summary>
        /// Maximum point on the body
        /// </summary>
        /// <returns></returns>
        public Vector3 Max()
        {
            return Box.Max;
        }

        #endregion
    }
}