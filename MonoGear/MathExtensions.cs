using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    public static class MathExtensions
    {
        public static float Square(float val)
        {
            return val * val;
        }

        public static Vector2 AngleToVector(float angle)
        {
            var v = new Vector2((float)Math.Sin(angle), -(float)Math.Cos(angle));
            v.Normalize();
            return v;
        }

        public static float VectorToAngle(Vector2 vector)
        {
            return (float)Math.Atan2(vector.X, -vector.Y);
        }
    }
}
