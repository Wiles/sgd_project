//File:     Vertices.cs
//Name:     Samuel Lewis (5821103) & Thomas Kempton (5781000)
//Date:     2013-04-15
//Class:    Simulation and Game Development
//Ass:      Project
//
//Desc:     
//          Verticies used to draw a sphere
//
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sgd_project
{
    /// <summary>
    /// Verticies used to draw a sphere
    /// </summary>
    public class SphereVertices{
        private readonly Color   _color;
        private Vector3 _offset = Vector3.Zero;

        /// <summary>
        /// Initializes a new instance of the <see cref="SphereVertices"/> class.
        /// </summary>
        /// <param name="vertexColor">Color of the vertex.</param>
        /// <param name="position">The position.</param>
        public SphereVertices(Color vertexColor, Vector3 position)
        {
            _offset          = position;
            _color           = vertexColor;
        }

        /// <summary>
        /// Initializes the sphere.
        /// </summary>
        /// <param name="numSlices">The num slices.</param>
        /// <param name="numStacks">The num stacks.</param>
        /// <param name="radius">The radius.</param>
        /// <returns></returns>
        public VertexPositionColor[] InitializeSphere(int numSlices,
                                                      int numStacks, float radius)
        {
            var position  = new Vector3[(numSlices + 1)
                                *             (numStacks + 1)];
            var rowHeight     = MathHelper.Pi/numStacks;
            var colWidth      = MathHelper.TwoPi / numSlices;

            // generate horizontal rows (stacks in sphere)
            for (var stacks = 0; stacks <= numStacks; stacks++)
            {
                var angleX = MathHelper.PiOver2 - stacks * rowHeight;
                var y = radius * (float)Math.Sin(angleX);
                var w = -radius * (float)Math.Cos(angleX);

                // generate vertical columns (slices in sphere)
                for (var slices = 0; slices <= numSlices; slices++)
                {
                    var angleY = slices * colWidth;
                    var x = w * (float)Math.Sin(angleY);
                    var z = w * (float)Math.Cos(angleY);

                    // position sphere vertices at offest from origin
                    position[stacks * numSlices + slices] =
                    new Vector3(x + _offset.X, y + _offset.Y, z + _offset.Z);
                }
            }
            var i = -1;
            var vertices = new VertexPositionColor[2 * numSlices * numStacks];
            
            // index vertices to draw sphere
            for (var stacks = 0; stacks < numStacks; stacks++)
            {
                for (var slices = 0; slices < numSlices; slices++)
                {
                    vertices[++i] = new VertexPositionColor(
                                        position[stacks * numSlices + slices], _color);
                    vertices[++i] = new VertexPositionColor(
                                        position[(stacks + 1) * numSlices + slices], _color);
                }
            }
            return vertices;
        }
    }
}