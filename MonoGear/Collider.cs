using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    public abstract class Collider
    {
        private static List<Collider> _colliders = new List<Collider>();

        public Vector2 BBSize { get; protected set; }
        public WorldEntity Entity { get; protected set; } 

        public Collider(WorldEntity entity)
        {
            if(entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            Entity = entity;
            Entity.Collider = this;

            _colliders.Add(this);
        }

        ~Collider()
        {
            _colliders.Remove(this);
        }

        public abstract bool Collides(Collider other);

        public virtual bool CollidesAny()
        {
            var colliders = BoxOverlapAny(this);
            if(colliders.Count() != 0)
            {
                foreach(var col in colliders)
                {
                    if(Collides(col))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected static bool BoxOverlap(Collider a, Collider b)
        {
            if(
                a.Entity.Position.X + a.BBSize.X / 2 < b.Entity.Position.X - b.BBSize.X / 2 ||    // a < b
                a.Entity.Position.Y + a.BBSize.Y / 2 < b.Entity.Position.Y - b.BBSize.Y / 2 ||   // a ^ b
                a.Entity.Position.X - a.BBSize.X / 2 > b.Entity.Position.X + b.BBSize.X / 2 ||   // a > b
                a.Entity.Position.Y - a.BBSize.Y / 2 > b.Entity.Position.Y + b.BBSize.Y / 2)     // a under b
            {
                return false;
            }
            return true;
        }

        protected static bool CircleOverlap(CircleCollider a, CircleCollider b)
        {
            float sqrRad = (a.Radius + b.Radius) * (a.Radius + b.Radius);
            Vector2 aPos = new Vector2(a.Entity.Position.X, a.Entity.Position.Y);
            Vector2 bPos = new Vector2(b.Entity.Position.X, b.Entity.Position.Y);

            if(Vector2.DistanceSquared(aPos, bPos) < sqrRad)
            {
                return true;
            }
            return false;
        }

        protected static bool CircleBoxOverlap(CircleCollider a, Collider b)
        {
            /*float sqrRad = a.Radius * a.Radius;
            Vector2 aPos = new Vector2(a.Entity.Position.X, a.Entity.Position.Y);
            Vector2 bPos1 = new Vector2(b.Entity.Position.X, b.Entity.Position.Y) - b.BBSize / 2;
            var bPos2 = bPos1 + b.BBSize;
            var bPos3 = bPos1 + new Vector2(b.BBSize.X, 0);
            var bPos4 = bPos1 + new Vector2(0, b.BBSize.Y);

            if(
                Vector2.DistanceSquared(aPos, bPos1) < sqrRad ||
                Vector2.DistanceSquared(aPos, bPos2) < sqrRad ||
                Vector2.DistanceSquared(aPos, bPos3) < sqrRad ||
                Vector2.DistanceSquared(aPos, bPos4) < sqrRad)
            {
                return true;
            }
            return false;*/

            Vector2 circleDistance = new Vector2();
            circleDistance.X = Math.Abs(a.Entity.Position.X - b.Entity.Position.X);
            circleDistance.Y = Math.Abs(a.Entity.Position.Y - b.Entity.Position.Y);

            if(circleDistance.X > (b.BBSize.X / 2 + a.Radius)) { return false; }
            if(circleDistance.Y > (b.BBSize.Y / 2 + a.Radius)) { return false; }

            if(circleDistance.X <= (b.BBSize.X/ 2)) { return true; }
            if(circleDistance.Y <= (b.BBSize.Y / 2)) { return true; }

            var cornerDistance_sq = MathExtensions.Square(circleDistance.X - b.BBSize.X / 2) +
                                 MathExtensions.Square(circleDistance.Y - b.BBSize.Y / 2);

            return (cornerDistance_sq <= MathExtensions.Square(a.Radius));
        }

        public static bool CircleTilemapOverlap(CircleCollider circle, TilemapCollider map)
        {
            // I'm not doing this, everything's a box now
            return BoxTilemapOverlap(circle, map);
        }

        public static bool BoxTilemapOverlap(Collider box, TilemapCollider map)
        {
            var boxStart = new Vector2(box.Entity.Position.X - box.BBSize.X / 2, box.Entity.Position.Y - box.BBSize.Y / 2);
            var boxEnd = new Vector2(box.Entity.Position.X + box.BBSize.X / 2, box.Entity.Position.Y + box.BBSize.Y / 2);
            int colStartX = Math.Max((int)(boxStart.X / map.TileSize), 0);
            int colStartY = Math.Max((int)(boxStart.Y / map.TileSize), 0);
            int colEndX = (int)(boxEnd.X / map.TileSize);
            int colEndY = (int)(boxEnd.Y / map.TileSize);

            for(int x = colStartX; x <= colEndX && x < map.Tiles.GetLength(0); x++)
            {
                for(int y = colStartY; y <= colEndY && y < map.Tiles.GetLength(1); y++)
                {
                    if(map.Tiles[x, y])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static IEnumerable<Collider> BoxOverlapAny(Collider col)
        {
            List<Collider> cols = new List<Collider>();
            foreach(var other in _colliders)
            {
                if(other == col)
                    continue;

                if(BoxOverlap(col, other))
                {
                    cols.Add(other);
                }
            }
            return cols;
        }
    }
}
