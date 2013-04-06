using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sgd_project
{
    interface IBound
    {
        bool Intersects(IBound that);
        void Draw(GraphicsDevice graphicsDevice, Matrix view, Matrix projection);
    }
}
