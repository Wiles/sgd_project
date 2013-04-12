//File:     HeadsUpDisplay.cs
//Name:     Samuel Lewis (5821103) & Thomas Kempton (5781000)
//Date:     2013-04-15
//Class:    Simulation and Game Development
//Ass:      Project
//
//Desc:     
//          Heads up display used to convey information to the player
//
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace sgd_project
{
    /// <summary>
    /// Heads up display used to convey information to the player
    /// </summary>
    class HeadsUpDisplay
    {
        private readonly SpriteFont _font;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeadsUpDisplay"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        public HeadsUpDisplay(SpriteFont font)
        {
            _font = font;
        }

        /// <summary>
        /// Draws the HUD to the display
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="view">The view.</param>
        /// <param name="lem">The lem.</param>
        public void Draw(SpriteBatch spriteBatch, Viewport view, Lem lem)
        {

            var height = _font.MeasureString("_").Y;
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
