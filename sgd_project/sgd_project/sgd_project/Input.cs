using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace sgd_project
{
    class Input
    {
        private GamePadState gamePad;
        private KeyboardState kb;
        private bool inverted;

        public Input(GamePadState pg, KeyboardState kb, bool inverted)
        {
            this.gamePad = pg;
            this.kb = kb;
            this.inverted = inverted;
        }

        public float Thrust()
        {
            var val = 0f;
            if (kb.IsKeyDown(Keys.Space))
            {
                val = 1.0f;
            }
            else
            {
                val = gamePad.Triggers.Right;
            }
            return val;
        }

        public float RotationZ()
        {
            var val = 0f;
            if (kb.IsKeyDown(Keys.D))
            {
                val = 1.0f;
            }
            else if (kb.IsKeyDown(Keys.A))
            {
                val = -1.0f;
            }
            else
            {
                val = gamePad.ThumbSticks.Left.X;
            }
            return inverted ? -val : val;
        }

        public float RotationX()
        {
            var val = 0f;
            if (kb.IsKeyDown(Keys.W))
            {
                val = 1.0f;
            }
            else if (kb.IsKeyDown(Keys.S))
            {
                val = -1.0f;
            }
            else
            {
                val = gamePad.ThumbSticks.Left.Y;
            }
            return inverted ? -val : val;
        }

        public bool Escape()
        {
            return gamePad.IsButtonDown(Buttons.Back) || kb.IsKeyDown(Keys.Escape);
        }

        public bool Select()
        {
            return gamePad.IsButtonDown(Buttons.A) || kb.IsKeyDown(Keys.Space);
        }

        public bool Up()
        {
            return gamePad.IsButtonDown(Buttons.DPadUp) || kb.IsKeyDown(Keys.W);
        }

        public bool Down()
        {
            return gamePad.IsButtonDown(Buttons.DPadDown) || kb.IsKeyDown(Keys.S);
        }
    }
}
