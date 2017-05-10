﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    public class BoxCollider : Collider
    {
        public Vector2 Size
        {
            get { return BBSize; }
            set { BBSize = value; }
        }

        public BoxCollider(WorldEntity entity, Vector2 size) : base(entity)
        {
            Size = size;
        }

        public override bool Collides(Collider other)
        {
            if(BoxOverlap(this, other))
            {
                var circle = other as CircleCollider;
                if(circle != null)
                {
                    return CircleBoxOverlap(circle, this);
                }
                var map = other as TilemapCollider;
                if(map != null)
                {
                    return BoxTilemapOverlap(this, map);
                }
                return true;
            }
            return false;
        }
    }
}
