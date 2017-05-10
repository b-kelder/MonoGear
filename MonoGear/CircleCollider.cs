﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
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
            var map = other as TilemapCollider;
            if(map != null)
            {
                return CircleTilemapOverlap(this, map);
            }
            if(BoxOverlap(this, other))
            {
                return CircleBoxOverlap(this, other);
            }
            return false;
        }
    }
}