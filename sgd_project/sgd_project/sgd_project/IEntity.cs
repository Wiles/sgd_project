//File:     IEntity.cs
//Name:     Samuel Lewis (5821103) & Thomas Kempton (5781000)
//Date:     2013-04-15
//Class:    Simulation and Game Development
//Ass:      Project
//
//Desc:     
//          Interface for a drawable Entity          
//

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sgd_project
{
    /// <summary>
    /// Interface for a drawable Entity
    /// </summary>
    internal interface IEntity
    {
        /// <summary>
        /// Gets the collision bounds.
        /// </summary>
        /// <returns></returns>
        IBound[] GetBounds();

        /// <summary>
        /// Draws the entity to the graphics device
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="view">The view.</param>
        /// <param name="projection">The projection.</param>
        void Draw(GraphicsDevice device, Matrix view, Matrix projection);

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <param name="input">The input.</param>
        void Update(long delta, Input input);
    }
}