
﻿//File:     HeadsUpDisplay.cs
//Name:     Samuel Lewis (5821103) & Thomas Kempton (5781000)
//Date:     2013-04-15
//Class:    Simulation and Game Development
//Ass:      Project
//
//Desc:     
//          Heads up display used to convey information to the player
//

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sgd_project
{
    /// <summary>
    /// Heads up display used to convey information to the player
    /// </summary>
    internal class HeadsUpDisplay
    {
        /// <summary>
        /// The cardinal directions
        /// </summary>
        private static string[] Directions = new[]
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
        private readonly Texture2D _texture;
        private readonly Texture2D _fuelNormal;
        private readonly Texture2D _fuelLow;
        private readonly Texture2D _fuelAvailable;

        private readonly SpriteFont _font;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeadsUpDisplay"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        public HeadsUpDisplay(SpriteFont font, Texture2D texture, Texture2D fuelNormal, Texture2D fuelLow, Texture2D fuelAvailable)
        {
            _font = font;
            _texture = texture;
            _fuelAvailable = fuelAvailable;
            _fuelLow = fuelLow;
            _fuelNormal = fuelNormal;
        }

        /// <summary>
        /// Gets the cardinal direction from an xy vector.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        /// <returns>The cardinal direction.</returns>
        private static string GetCardinalDirection(float x, float z)
        {
            var angle = ((Math.Atan2(x, -z) * (180 / Math.PI)) + 360.0) % 360.0;
            var offset = 360.0 / (Directions.Length * 2.0);
            var fraction = 360.0 / Directions.Length;
            var i = (angle + offset) / fraction;
            return Directions[(int)i];
        }

        /// <summary>
        /// Gets the wind speed from an xz vector.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        /// <returns>The speed in km/h</returns>
        private static float GetWindSpeed(float x, float z)
        {
            return (float)Math.Sqrt(x * x + z * z) * 3.6f * Lander.Metre.X;
        }

        /// <summary>
        /// Draws the heads up display to the specified sprite batch.
        /// Draws the HUD to the display
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="view">The view.</param>
        /// <param name="lem">The lem.</param>
        /// <param name="wind">The wind vector.</param>
        public void Draw(SpriteBatch spriteBatch, Viewport view, Lem lem, Vector3 wind, int score, bool store)
        {
            spriteBatch.Draw(_texture, Vector2.Zero, Color.White);

            if(store)
            {
                spriteBatch.Draw(_fuelAvailable, new Vector2(view.Width - _fuelAvailable.Width, view.Height - _fuelAvailable.Height), Color.White);
            
            }
            else if (lem.Fuel < 10)
            {
                spriteBatch.Draw(_fuelLow, new Vector2(view.Width - _fuelLow.Width, view.Height - _fuelLow.Height), Color.White);
            }
            else
            {
                spriteBatch.Draw(_fuelNormal, new Vector2(view.Width - _fuelNormal.Width, view.Height - _fuelNormal.Height), Color.White);

            }

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
                                   string.Format(@"Wind:   {0:0.0} km\h {1}", GetWindSpeed(wind.X, wind.Z), GetCardinalDirection(wind.X, wind.Z)),
                                   new Vector2(590, 7),
                                   Color.Black, 0.0f,
                                   Vector2.Zero,
                                   1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(_font,
                                   string.Format(@"Height: {0:0.00}", (lem.Position.Y - Lem.MinY) / Lander.Metre.Y),
                                   new Vector2(590, height + 7),
                                   Color.Black, 0.0f,
                                   Vector2.Zero,
                                   1.0f, SpriteEffects.None, 0.0f);

            spriteBatch.DrawString(_font,
                                   string.Format(@"Fuel:  {0:0.00}", lem.Fuel),
                                   new Vector2(370, 7),
                                   Color.Black, 0.0f,
                                   Vector2.Zero,
                                   1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(_font,
                                   string.Format(@"Score: {0:0.00}", score),
                                   new Vector2(370, height + 7),
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