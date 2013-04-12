//File:     IBound.cs
//Name:     Samuel Lewis (5821103) & Thomas Kempton (5781000)
//Date:     2013-04-15
//Class:    Simulation and Game Development
//Ass:      Project
//
//Desc:     
//          Inferface used to make it easier to check collisions between boxes and spheres
//
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sgd_project
{
    /// <summary>
    /// Inferface used to make it easier to check collisions between boxes and spheres
    /// </summary>
    public interface IBound
    {
        /// <summary>
        /// Checks if the two bounds intersect eachother
        /// </summary>
        /// <param name="that">The other set of bounds</param>
        /// <returns>True if they intersect</returns>
        bool Intersects(IBound that);

        /// <summary>
        /// Draws the collision object.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="view">The view.</param>
        /// <param name="projection">The projection.</param>
        void Draw(GraphicsDevice graphicsDevice, Matrix view, Matrix projection);

        /// <summary>
        /// Minimum point on the body
        /// </summary>
        /// <returns></returns>
        Vector3 Min();

        /// <summary>
        /// Maximum point on the body
        /// </summary>
        /// <returns></returns>
        Vector3 Max();
    }
}
