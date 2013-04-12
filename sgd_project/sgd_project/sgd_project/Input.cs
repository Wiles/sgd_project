//File:     Input.cs
//Name:     Samuel Lewis (5821103) & Thomas Kempton (5781000)
//Date:     2013-04-15
//Class:    Simulation and Game Development
//Ass:      Project
//
//Desc:     
//          Wrapper for user input handles both Keyboard and Game pad
//

using Microsoft.Xna.Framework.Input;

namespace sgd_project
{
    /// <summary>
    /// Wrapper for user input handles both Keyboard and Game pad
    /// </summary>
    public class Input
    {
        private readonly bool _inverted;
        private GamePadState _gamePad;
        private KeyboardState _keyboard;

        /// <summary>
        /// Initializes a new instance of the <see cref="Input"/> class.
        /// </summary>
        /// <param name="pg">The pg.</param>
        /// <param name="keyboard">The keyboard.</param>
        /// <param name="inverted">if set to <c>true</c> [inverted].</param>
        public Input(GamePadState pg, KeyboardState keyboard, bool inverted)
        {
            _gamePad = pg;
            _keyboard = keyboard;
            _inverted = inverted;
        }

        /// <summary>
        /// Thruster input
        /// </summary>
        /// <returns>Thrust level</returns>
        public float Thrust()
        {
            if (_keyboard.IsKeyDown(Keys.Space))
            {
                return 1.0f;
            }
            return _gamePad.Triggers.Right;
        }

        /// <summary>
        /// Thruster Z rotation
        /// </summary>
        /// <returns></returns>
        public float RotationZ()
        {
            if (_keyboard.IsKeyDown(Keys.D))
            {
                return 1.0f;
            }
            if (_keyboard.IsKeyDown(Keys.A))
            {
                return -1.0f;
            }
            return _inverted ? -_gamePad.ThumbSticks.Left.X : _gamePad.ThumbSticks.Left.X;
        }

        /// <summary>
        /// Thruster X rotation
        /// </summary>
        /// <returns></returns>
        public float RotationX()
        {
            if (_keyboard.IsKeyDown(Keys.W))
            {
                return 1.0f;
            }
            if (_keyboard.IsKeyDown(Keys.S))
            {
                return -1.0f;
            }

            return _inverted ? -_gamePad.ThumbSticks.Left.Y : _gamePad.ThumbSticks.Left.Y;
        }

        /// <summary>
        /// Cameras Y rotation
        /// </summary>
        /// <returns></returns>
        public float CameraRotationY()
        {
            if (_keyboard.IsKeyDown(Keys.Left))
            {
                return .5f;
            }
            if (_keyboard.IsKeyDown(Keys.Right))
            {
                return -.5f;
            }
            if (_keyboard.IsKeyDown(Keys.Up))
            {
                return 1f;
            }
            return _inverted ? -_gamePad.ThumbSticks.Right.X : _gamePad.ThumbSticks.Right.X;
        }

        /// <summary>
        /// Escapes Key
        /// </summary>
        /// <returns></returns>
        public bool Escape()
        {
            return _gamePad.IsButtonDown(Buttons.Back) || _keyboard.IsKeyDown(Keys.Escape);
        }

        /// <summary>
        /// Select
        /// </summary>
        /// <returns></returns>
        public bool Select()
        {
            return _gamePad.IsButtonDown(Buttons.A) || _keyboard.IsKeyDown(Keys.Space);
        }

        /// <summary>
        /// Up
        /// </summary>
        /// <returns></returns>
        public bool Up()
        {
            return _gamePad.IsButtonDown(Buttons.DPadUp) || _keyboard.IsKeyDown(Keys.W);
        }

        /// <summary>
        /// Down
        /// </summary>
        /// <returns></returns>
        public bool Down()
        {
            return _gamePad.IsButtonDown(Buttons.DPadDown) || _keyboard.IsKeyDown(Keys.S);
        }
    }
}