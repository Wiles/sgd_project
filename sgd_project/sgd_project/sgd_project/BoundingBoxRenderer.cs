using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sgd_project
{
    public static class BoundingBoxRenderer
    {
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