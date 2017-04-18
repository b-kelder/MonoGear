using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    class Viewport : WorldEntity
    {
        public Viewport(int width, int height) : base()
        {
            Size = new Vector2(width, height);
        }

        public bool InView(Vector3 position)
        {
            if(position.X < this.Position.X
                || position.X > this.Position.X + this.Size.X
                || position.Y < this.Position.Y
                || position.Y > this.Position.Y + this.Size.Y)
            {
                return false;
            }
            return true;
        }

        public Vector2 Translate(Vector2 position)
        {
            return new Vector2(position.X + this.Position.X, position.Y + this.Position.Y);
        }
    }
}
