using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear.Engine.Collisions
{
    /// <summary>
    /// Circle shaped collider.
    /// </summary>
    public class CircleCollider : Collider
    {
        private float radius;
        public float Radius
        {
            get { return radius; }
            set
            {
                radius = value;
                BBSize = new Vector2(radius * 2);
            }
        }

        public CircleCollider(WorldEntity entity, float radius) : base(entity)
        {
            Radius = radius;
        }

        public override bool Collides(Collider other)
        {
            var circle = other as CircleCollider;
            if(circle != null)
            {
                return CircleOverlap(circle, this);
            }
            if(BoxOverlap(this, other))
            {
                return CircleBoxOverlap(this, other);
            }
            return false;
        }
    }
}
