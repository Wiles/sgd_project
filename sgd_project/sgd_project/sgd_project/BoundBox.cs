using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sgd_project
{

    class BoundBox : IBound
    {
        public BoundingBox Box { get; private set; }

        public BoundBox(BoundingBox box)
        {
            Box = box;
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
            BoundingBoxRenderer.Render(Box, graphicsDevice, view, projection, Color.Red);
        }
    }

}
