using Microsoft.Xna.Framework;
using System;

namespace MonoGear
{
    public static class MathExtensions
    {
        /// <summary>
        /// Method that calculates the square of a value.
        /// </summary>
        /// <param name="val">The value</param>
        /// <returns>The square of the value</returns>
        public static float Square(float val)
        {
            return val * val;
        }

        /// <summary>
        /// Method that converts an angle to a vector.
        /// </summary>
        /// <param name="angle">The angle that need to be converted</param>
        /// <returns>The angle converted to a vector</returns>
        public static Vector2 AngleToVector(float angle)
        {
            var v = new Vector2((float)Math.Sin(angle), -(float)Math.Cos(angle));
            v.Normalize();
            return v;
        }

        /// <summary>
        /// Method that converts a vector to an angle
        /// </summary>
        /// <param name="vector">The vector that needs to be converted</param>
        /// <returns>The vector converted to an angle</returns>
        public static float VectorToAngle(Vector2 vector)
        {
            return (float)Math.Atan2(vector.X, -vector.Y);
        }

        /// <summary>
        /// Method that calculates the angle between two vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>The angle between the two vectores</returns>
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
