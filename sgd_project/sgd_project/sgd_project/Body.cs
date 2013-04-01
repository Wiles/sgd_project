using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace sgd_project
{
    class Body
    {
        public Vector3 Gravity { get; private set; }
        public Vector3 Wind { get; private set; }
        public Body(Vector3 gravity, Vector3 wind)
        {
            Gravity = gravity;
            Wind = wind;
        }
    }
}
