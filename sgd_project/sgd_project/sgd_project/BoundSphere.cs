using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sgd_project
{
    public class BoundSphere : IBound
    {
        const int Slices = 10;
        const int Stacks = 10;

        public BoundingSphere Sphere { get; private set; }
        public VertexPositionColor[] Vertices { get; private set; }
        public int PrimativeCount { get; private set; }

        public BoundSphere(BoundingSphere sphere)
        {
            Sphere = sphere;
            PrimativeCount = Slices * Stacks * 2 - 1;
            SphereVertices sphereVertices
                          = new SphereVertices(Color.Red, PrimativeCount, Sphere.Center);
            Vertices
                          = sphereVertices.InitializeSphere(Slices, Stacks, Sphere.Radius);
        }

        public bool Intersects(IBound that)
        {
            if(that is BoundSphere)
            {
                return Sphere.Intersects((that as BoundSphere).Sphere);
            }
            if(that is BoundBox)
            {
                return Sphere.Intersects((that as BoundBox).Box);
            }
            return false;
        }

        public void Draw(GraphicsDevice graphicsDevice, Matrix view, Matrix projection)
        {
            BoundingSphereRenderer.Render(graphicsDevice, this, view, projection);
        }

        public Vector3 Min()
        {
            return BoundingBox.CreateFromSphere(Sphere).Min;
        }

        public Vector3 Max()
        {
            return BoundingBox.CreateFromSphere(Sphere).Max;
        }
    }
}
