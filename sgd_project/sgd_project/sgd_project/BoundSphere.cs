//File:     BoundSphere.cs
//Name:     Samuel Lewis (5821103) & Thomas Kempton (5781000)
//Date:     2013-04-15
//Class:    Simulation and Game Development
//Ass:      Project
//
//Desc:     
//          Wrapper for the BoundingSphere
//

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sgd_project
{
    /// <summary>
    /// Wrapper for the BoundingSphere
    /// </summary>
    public class BoundSphere : IBound
    {
        private const int Slices = 10;
        private const int Stacks = 10;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundSphere"/> class.
        /// </summary>
        /// <param name="sphere">The sphere.</param>
        public BoundSphere(BoundingSphere sphere)
        {
            Sphere = sphere;
            PrimativeCount = Slices*Stacks*2 - 1;
            var sphereVertices = new SphereVertices(Color.Red, Sphere.Center);
            Vertices = sphereVertices.InitializeSphere(Slices, Stacks, Sphere.Radius);
        }

        /// <summary>
        /// Gets the sphere.
        /// </summary>
        /// <value>
        /// The sphere.
        /// </value>
        public BoundingSphere Sphere { get; private set; }

        /// <summary>
        /// Gets the vertices.
        /// </summary>
        /// <value>
        /// The vertices.
        /// </value>
        public VertexPositionColor[] Vertices { get; private set; }

        /// <summary>
        /// Gets the number of primatives which make up the sphere.
        /// </summary>
        /// <value>
        /// The primative count.
        /// </value>
        public int PrimativeCount { get; private set; }

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
            if (that is BoundSphere)
            {
                return Sphere.Intersects((that as BoundSphere).Sphere);
            }
            if (that is BoundBox)
            {
                return Sphere.Intersects((that as BoundBox).Box);
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
            BoundingSphereRenderer.Render(graphicsDevice, this, view, projection);
        }

        /// <summary>
        /// Minimum point on the body
        /// </summary>
        /// <returns></returns>
        public Vector3 Min()
        {
            return BoundingBox.CreateFromSphere(Sphere).Min;
        }

        /// <summary>
        /// Maximum point on the body
        /// </summary>
        /// <returns></returns>
        public Vector3 Max()
        {
            return BoundingBox.CreateFromSphere(Sphere).Max;
        }

        #endregion
    }
}