using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace sgd_project
{
    class HeadsUpDisplay
    {
        private SpriteFont _font;

        public HeadsUpDisplay(SpriteFont font)
        {
            _font = font;
        }

        public void Draw(SpriteBatch spriteBatch, Viewport view, Lem lem)
        {

            float height = _font.MeasureString("_").Y;
            spriteBatch.DrawString(_font,
                                   string.Format(@"X Rot.: {0:0.00}", MathHelper.ToDegrees(lem.RotationX)),
                                   new Vector2((view.Width / 2) + 2, height + 2),
                                   Color.White, 0.0f,
                                   Vector2.Zero,
                                   1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(_font,
                                   string.Format(@"Y Rot.: {0:0.00}", MathHelper.ToDegrees(lem.RotationZ)),
                                   new Vector2((view.Width / 2) + 2, height * 2 + 2),
                                   Color.White, 0.0f,
                                   Vector2.Zero,
                                   1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(_font,
                                   string.Format(@"  Fuel: {0:0.00}", lem.Fuel),
                                   new Vector2((view.Width / 2) + 2, height * 3 + 2),
                                   Color.White, 0.0f,
                                   Vector2.Zero,
                                   1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(_font,
                                   string.Format(@"  Height: {0:0.00}", (lem.Position.Y - Lem.MinY) / Lander.Metre.Y),
                                   new Vector2((view.Width / 2) + 2, height * 4 + 2),
                                   Color.White, 0.0f,
                                   Vector2.Zero,
                                   1.0f, SpriteEffects.None, 0.0f);

        }
    }
}
