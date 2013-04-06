using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sgd_project
{
    public interface IBound
    {
        bool Intersects(IBound that);
        void Draw(GraphicsDevice graphicsDevice, Matrix view, Matrix projection);
        Vector3 Min();
        Vector3 Max();
    }
}
