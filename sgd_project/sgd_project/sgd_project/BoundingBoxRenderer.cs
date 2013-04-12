//File:     BoundingBoxRenderer.cs
//Name:     Samuel Lewis (5821103) & Thomas Kempton (5781000)
//Date:     2013-04-15
//Class:    Simulation and Game Development
//Ass:      Project
//
//Desc:     
//          Draws a BoundBox to the display
//
//          This is currently very slow to render but it sufficient for debugging purposes
//
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sgd_project
{
    /// <summary>
    /// Draws a BoundBox to the display
    /// </summary>
    public static class BoundingBoxRenderer
    {
        /// <summary>
        /// The box indices
        /// </summary>
        private static readonly short[] BoxIndices = {
            0, 1, 1, 2, 2, 3, 3, 0, // Front edges
            4, 5, 5, 6, 6, 7, 7, 4, // Back edges
            0, 4, 1, 5, 2, 6, 3, 7 // Side edges connecting front and back
        };

        /// <summary>
        /// Renders the bounding box for debugging purposes.
        /// </summary>
        /// <param name="box">The box to render.</param>
        /// <param name="graphicsDevice">The graphics device to use when rendering.</param>
        /// <param name="view">The current view matrix.</param>
        /// <param name="projection">The current projection matrix.</param>
        /// <param name="color">The color to use drawing the lines of the box.</param>
        public static void Render(
            BoundBox box,
            GraphicsDevice graphicsDevice,
            Matrix view,
            Matrix projection,
            Color color)
        {

            var boxEffect = new BasicEffect(graphicsDevice);
            /* Set your own effect parameters here */
            boxEffect.World = Matrix.Identity;
            boxEffect.View = view;
            boxEffect.Projection = projection;
            boxEffect.TextureEnabled = false;

            // Draw the box with a LineList
            foreach (var pass in boxEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList, box.Vertices, 0, 8,
                    BoxIndices, 0, 12);
            }
        }
    }
}