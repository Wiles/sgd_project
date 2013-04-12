using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sgd_project
{
    class HeadsUpDisplay : IDisposable
    {
        /// <summary>
        /// The font.
        /// </summary>
        private SpriteFont _font;

        /// <summary>
        /// The cardinal directions
        /// </summary>
        private static string[] Directions = new string[]
        {
            "N",
            "NNE",
            "NE",
            "ENE",
            "E",
            "ESE",
            "SE",
            "SSE",
            "S",
            "SSW",
            "SW",
            "WSW",
            "W",
            "WNW",
            "NW",
            "NNW"
        };

        /// <summary>
        /// 
        /// </summary>
        private Texture2D _texture;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="HeadsUpDisplay" /> class.
        /// </summary>
        /// <param name="font">The font.</param>
        public HeadsUpDisplay(SpriteFont font, Texture2D texture)
        {
            _font = font;
            _texture = texture;
        }

        /// <summary>
        /// Gets the cardinal direction from an xy vector.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        /// <returns>The cardinal direction.</returns>
        private static string GetCardinalDirection(float x, float z)
        {
            var angle = ((Math.Atan2(x, z) * (180 / Math.PI)) + 360.0) % 360.0;
            var offset = 360.0 / (Directions.Length * 2.0);
            var fraction = 360.0 / Directions.Length;
            var i = (angle + offset) / fraction;
            return Directions[(int)i];
        }

        /// <summary>
        /// Gets the wind speed from an xy vector.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        /// <returns>The speed in km/h</returns>
        private static float GetWindSpeed(float x, float z)
        {
            return (float)Math.Sqrt(x * x + z * z) * 3.6f;
        }

        /// <summary>
        /// Draws the heads up display to the specified sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="view">The view.</param>
        /// <param name="lem">The lem.</param>
        /// <param name="wind">The wind vector.</param>
        public void Draw(SpriteBatch spriteBatch, Viewport view, Lem lem, Vector3 wind)
        {
            spriteBatch.Draw(_texture, Vector2.Zero, Color.White);

            float height = _font.MeasureString("_").Y;

            spriteBatch.DrawString(_font,
                                   string.Format(@"X Rot.: {0:0.00}", MathHelper.ToDegrees(lem.RotationX)),
                                   new Vector2(180, 7),
                                   Color.Black, 0.0f,
                                   Vector2.Zero,
                                   1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(_font,
                                   string.Format(@"Y Rot.: {0:0.00}", MathHelper.ToDegrees(lem.RotationZ)),
                                   new Vector2(180, height + 7),
                                   Color.Black, 0.0f,
                                   Vector2.Zero,
                                   1.0f, SpriteEffects.None, 0.0f);

            spriteBatch.DrawString(_font,
                                   string.Format(@"  Wind: {0:0.0} km\h {1}", GetWindSpeed(wind.X, wind.Z), GetCardinalDirection(wind.X, wind.Y)),
                                   new Vector2(580, 7),
                                   Color.Black, 0.0f,
                                   Vector2.Zero,
                                   1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(_font,
                                   string.Format(@"Height: {0:0.00}", (lem.Position.Y - Lem.MinY) / Lander.Metre.Y),
                                   new Vector2(580, height + 7),
                                   Color.Black, 0.0f,
                                   Vector2.Zero,
                                   1.0f, SpriteEffects.None, 0.0f);

            spriteBatch.DrawString(_font,
                                   string.Format(@"  Fuel: {0:0.00}", lem.Fuel),
                                   new Vector2(370, 7),
                                   Color.Black, 0.0f,
                                   Vector2.Zero,
                                   1.0f, SpriteEffects.None, 0.0f);
        }

        /// <summary>
        /// Performs application-defined tasks associated with
        /// freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this._texture.Dispose();
        }
    }
}