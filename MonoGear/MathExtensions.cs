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

        public static float AngleBetween(Vector2 a, Vector2 b)
        {
            return (float)Math.Atan2(b.Y - a.Y, b.X - a.X);
        }
    }

    public class FloatRect
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public float Left
        {
            get
            {
                return X;
            }
        }

        public float Right
        {
            get
            {
                return X + Width;
            }
        }

        public float Top
        {
            get
            {
                return Y;
            }
        }

        public float Bottom
        {
            get
            {
                return Y + Height;
            }
        }

        public FloatRect(float x, float y, float w, float h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

        public FloatRect()
        {
            X = 0;
            Y = 0;
            Width = 0;
            Height = 0;
        }

        public static implicit operator Rectangle(FloatRect m)
        {
            return new Rectangle((int)m.X, (int)m.Y, (int)m.Width, (int)m.Height);
        }
    }
}
