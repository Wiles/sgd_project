using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sgd_project
{

    public class BoundBox : IBound
    {
        public BoundingBox Box { get; private set; }
        public VertexPositionColor[] Vertices { get; private set; }

        public BoundBox(BoundingBox box)
        {
            Box = box;
            var corners = box.GetCorners();
            Vertices = new VertexPositionColor[corners.Length];


            // Assign the 8 box vertices
            for (var i = 0; i < corners.Length; i++)
            {
                Vertices[i] = new VertexPositionColor(corners[i], Color.Green);
            }
        }

        public bool Intersects(IBound that)
        {
            if(that is BoundBox)
            {
                return Box.Intersects((that as BoundBox).Box);
            }
            if(that is BoundSphere)
            {
                return Box.Intersects((that as BoundSphere).Sphere);
            }

            return false;
        }

        public void Draw(GraphicsDevice graphicsDevice, Matrix view, Matrix projection)
        {
            BoundingBoxRenderer.Render(this, graphicsDevice, view, projection, Color.Red);
        }

        public Vector3 Min()
        {
            return Box.Min;
        }

        public Vector3 Max()
        {
            return Box.Max;
        }
    }

}
