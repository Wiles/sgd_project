//File:     Body.cs
//Name:     Samuel Lewis (5821103) & Thomas Kempton (5781000)
//Date:     2013-04-15
//Class:    Simulation and Game Development
//Ass:      Project
//
//Desc:     
//  Represents an astronomical body
//

using Microsoft.Xna.Framework;

namespace sgd_project
{
    /// <summary>
    /// Represents an astronomical body
    /// </summary>
    internal class Body
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Body"/> class.
        /// </summary>
        /// <param name="gravity">The gravity.</param>
        /// <param name="wind">The wind.</param>
        /// <param name="ground">The ground.</param>
        public Body(Vector3 gravity, Vector3 wind, Ground ground)
        {
            Gravity = gravity;
            Wind = wind;
            Ground = ground;
        }

        /// <summary>
        /// Gets the gravity.
        /// </summary>
        /// <value>
        /// The gravity.
        /// </value>
        public Vector3 Gravity { get; private set; }

        /// <summary>
        /// Gets the wind.
        /// </summary>
        /// <value>
        /// The wind.
        /// </value>
        public Vector3 Wind { get; private set; }

        /// <summary>
        /// Gets the ground.
        /// </summary>
        /// <value>
        /// The ground.
        /// </value>
        public Ground Ground { get; private set; }
    }
}