using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    public abstract class Collider
    {
        /// <summary>
        /// Contains all colliders
        /// </summary>
        private static HashSet<Collider> _colliders = new HashSet<Collider>();
        /// <summary>
        /// Collider used for raycasting
        /// </summary>
        private static BoxCollider _raycastCollider = new BoxCollider(new Bird(), Vector2.One);  // Just need a WorldEntity that has no collider for this

        private static BoxCollider _tilemapCollider = new BoxCollider(new Bird(), Vector2.Zero);

        public bool Trigger { get; set; }
        public bool Active { get; set; }
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
            Active = true;
            Trigger = false;

            Register();
        }

        public void Register()
        {
            _colliders.Add(this);
        }

        public void Deregister()
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

        public virtual bool CollidesAny(out Collider other)
        {
            var colliders = BoxOverlapAny(this);
            if(colliders.Count() != 0)
            {
                foreach(var col in colliders)
                {
                    if(Collides(col))
                    {
                        other = col;
                        return true;
                    }
                }
            }
            other = null;
            return false;
        }

        public virtual bool CollidesAny(out Collider other, Collider ignored)
        {
            var colliders = BoxOverlapAny(this);
            if(colliders.Count() != 0)
            {
                foreach(var col in colliders)
                {
                    if(col == ignored)
                    {
                        continue;
                    }

                    if(Collides(col))
                    {
                        other = col;
                        return true;
                    }
                }
            }
            other = null;
            return false;
        }

        protected static bool BoxOverlap(Collider a, Collider b)
        {
            if(
                a.Entity.Position.X + a.BBSize.X / 2 < b.Entity.Position.X - b.BBSize.X / 2 ||    // a < b
                a.Entity.Position.Y + a.BBSize.Y / 2 < b.Entity.Position.Y - b.BBSize.Y / 2 ||   // a ^ b
                a.Entity.Position.X - a.BBSize.X / 2 > b.Entity.Position.X + b.BBSize.X / 2 ||   // a > b
                a.Entity.Position.Y - a.BBSize.Y / 2 > b.Entity.Position.Y + b.BBSize.Y / 2)     // a v b
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

        public static bool RaycastAny(Vector2 from, Vector2 to, out Collider firstHit, string ignoreTag, float delta = 8.0f)
        {
            Vector2 deltaVec = (to - from);
            deltaVec.Normalize();
            deltaVec *= delta;

            _raycastCollider.Active = false;
            _raycastCollider.Entity.Enabled = false;
            _raycastCollider.Entity.Position = from;

            float prevDistance = float.MaxValue;
            float distance = Vector2.DistanceSquared(_raycastCollider.Entity.Position, to);

            while(distance < prevDistance)
            {
                if(_raycastCollider.CollidesAny(out firstHit) && firstHit.Entity.Tag != ignoreTag)
                {
                    return true;
                }
                _raycastCollider.Entity.Move(deltaVec);
                prevDistance = distance;
                distance = Vector2.DistanceSquared(_raycastCollider.Entity.Position, to);
            }

            firstHit = null;
            return false;
        }

        public static bool CircleTilemapOverlap(CircleCollider circle, Level level)
        {
            // I'm not doing this, everything's a box now
            return BoxTilemapOverlap(circle, level);
        }

        public static bool BoxTilemapOverlap(Collider box, Level level)
        {
            var boxStart = new Vector2(box.Entity.Position.X - box.BBSize.X / 2, box.Entity.Position.Y - box.BBSize.Y / 2);
            var boxEnd = new Vector2(box.Entity.Position.X + box.BBSize.X / 2, box.Entity.Position.Y + box.BBSize.Y / 2);
            int colStartX = Math.Max((int)(boxStart.X / level.TileWidth), 0);
            int colStartY = Math.Max((int)(boxStart.Y / level.TileHeight), 0);
            int colEndX = (int)(boxEnd.X / level.TileWidth);
            int colEndY = (int)(boxEnd.Y / level.TileHeight);

            for(int x = colStartX; x <= colEndX && x < level.Width; x++)
            {
                for(int y = colStartY; y <= colEndY && y < level.Height; y++)
                {
                    if(!level.Tiles[x + y * level.Width].Walkable)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Returns if the collider is the dummy used for indicating a tilemap collision.
        /// </summary>
        /// <param name="col">Collider to check</param>
        /// <returns>True if it is the tilemap collider dummy</returns>
        public static bool IsTilemap(Collider col)
        {
            return col == _tilemapCollider;
        }

        /// <summary>
        /// Checks collider list and returns any colliders the given collider has a box overlap with.
        /// Does not return inactive or trigger colliders or colliders who's entity is disabled.
        /// Used in the first stage for collision checks to gather the colliders to do more precise collision detection against.
        /// </summary>
        /// <param name="col">Collider to check</param>
        /// <returns>Colliders that collide</returns>
        public static IEnumerable<Collider> BoxOverlapAny(Collider col)
        {
            List<Collider> cols = new List<Collider>();
            foreach(var other in _colliders)
            {
                if(other == col || other.Active == false || other.Entity.Enabled == false || other.Trigger || other == _raycastCollider || other == _tilemapCollider)
                    continue;

                if(BoxOverlap(col, other))
                {
                    cols.Add(other);
                }
            }

            // Always 'collide' with the tilemap
            cols.Add(_tilemapCollider);

            return cols;
        }
    }
}
