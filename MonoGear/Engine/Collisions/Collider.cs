using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using MonoGear.Entities;

namespace MonoGear.Engine.Collisions
{
    /// <summary>
    /// Base class for Colliders
    /// </summary>
    public abstract class Collider
    {
        /// <summary>
        /// Contains all colliders
        /// </summary>
        private static HashSet<Collider> _colliders;
        /// <summary>
        /// Collider used for raycasting since 
        /// </summary>
        private static BoxCollider _raycastCollider;

        public bool Trigger { get; set; }
        public bool Active { get; set; }
        public Vector2 BBSize { get; protected set; }
        public WorldEntity Entity { get; protected set; } 

        /// <summary>
        /// The static contructor of the collider class
        /// </summary>
        static Collider()
        {
            _colliders = new HashSet<Collider>();
        }

        /// <summary>
        /// The contructor of the collider class
        /// </summary>
        /// <param name="entity"></param>
        public Collider(WorldEntity entity)
        {
            // Throw exeption if we have no entity
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

        /// <summary>
        /// Method to register a collider and use it in collision checking
        /// </summary>
        public void Register()
        {
            _colliders.Add(this);
        }

        /// <summary>
        /// Method to deregister a collider and prevent it from being used in collision checking
        /// </summary>
        public void Deregister()
        {
            _colliders.Remove(this);
        }

        /// <summary>
        /// Method that checks if a collider collides with a specific collider
        /// </summary>
        /// <param name="other"></param>
        /// <returns>True if the collision is detected</returns>
        public abstract bool Collides(Collider other);

        /// <summary>
        /// Method that checks if there is a collision with any collider
        /// </summary>
        /// <returns>True if a collision is detected</returns>
        public virtual bool CollidesAny()
        {
            // Check if we could be colliding
            var colliders = BoxOverlapAny(this);
            if(colliders.Count() != 0)
            {
                // Run through all colliders that might collide and check if we do
                foreach(var col in colliders)
                {
                    if(Collides(col))
                    {
                        return true;
                    }
                }
            }

            // Tilemap collision
            var circle = this as CircleCollider;
            if(circle != null)
            {
                return CircleTilemapOverlap(circle, MonoGearGame.GetCurrentLevel());
            }
            else
            {
                return BoxTilemapOverlap(this, MonoGearGame.GetCurrentLevel());
            }
        }

        /// <summary>
        /// Method that checks if there is a collision with any collider
        /// </summary>
        /// <param name="other">Returns the other collider</param>
        /// <param name="hitTilemap">Returns if the collision is with the tilemap</param>
        /// <returns>True if a collision is detected</returns>
        public virtual bool CollidesAny(out Collider other, out bool hitTilemap)
        {
            // Check if we could be colliding
            var colliders = BoxOverlapAny(this);
            if(colliders.Count() != 0)
            {
                // Run through all colliders that might collide and check if we do
                foreach (var col in colliders)
                {
                    if(Collides(col))
                    {
                        other = col;
                        hitTilemap = false;
                        return true;
                    }
                }
            }
            other = null;

            // Tilemap collision
            var circle = this as CircleCollider;
            if(circle != null)
            {
                hitTilemap = CircleTilemapOverlap(circle, MonoGearGame.GetCurrentLevel());
            }
            else
            {
                hitTilemap = BoxTilemapOverlap(this, MonoGearGame.GetCurrentLevel());
            }

            return hitTilemap;
        }

        /// <summary>
        /// Method that checks if there is a collision with any collider
        /// </summary>
        /// <param name="other">Returns the other collider</param>
        /// <param name="hitTilemap">Returns if the collision is with the tilemap</param>
        /// <param name="ignored">The collider to ignore</param>
        /// <returns>True if a collision is detected</returns>
        public virtual bool CollidesAny(out Collider other, out bool hitTilemap, Collider ignored)
        {
            // Check if we could be colliding
            var colliders = BoxOverlapAny(this);
            if(colliders.Count() != 0)
            {
                // Run through all colliders that might collide and check if we do
                foreach (var col in colliders)
                {
                    // Check if we need to ignore the collider
                    if(col == ignored)
                    {
                        continue;
                    }

                    if(Collides(col))
                    {
                        other = col;
                        hitTilemap = false;
                        return true;
                    }
                }
            }
            other = null;

            // Tilemap collision
            var circle = this as CircleCollider;
            if(circle != null)
            {
                hitTilemap = CircleTilemapOverlap(circle, MonoGearGame.GetCurrentLevel());
            }
            else
            {
                hitTilemap = BoxTilemapOverlap(this, MonoGearGame.GetCurrentLevel());
            }

            return hitTilemap;
        }

        /// <summary>
        /// Check if there is a overlap between two colliders
        /// </summary>
        /// <param name="a">Collider a</param>
        /// <param name="b">Collider b</param>
        /// <returns>Returns true if an overlap is found</returns>
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

        /// <summary>
        /// Check if there is a circle overlap between two circlecolliders
        /// </summary>
        /// <param name="a">Collider a</param>
        /// <param name="b">Collider b</param>
        /// <returns>Returns true if an overlap is found</returns>
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

        /// <summary>
        /// Check if there is an overlap between a circlecollider and a collider
        /// </summary>
        /// <param name="a">Collider a</param>
        /// <param name="b">Collider b</param>
        /// <returns>Returns true if an overlap is found</returns>
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

        /// <summary>
        /// Method used to cast a ray from a point to a point
        /// </summary>
        /// <param name="from">Where to shoot from</param>
        /// <param name="to">Where to shoot to</param>
        /// <param name="firstHit">The first item to be hit by the ray</param>
        /// <param name="hitTilemap">True if we hit the tilemap</param>
        /// <param name="ignoreTag">What tag to ignore when checking collisions</param>
        /// <param name="delta">The delta used</param>
        /// <returns>Returns true if a collision was detected</returns>
        public static bool RaycastAny(Vector2 from, Vector2 to, out Collider firstHit, out bool hitTilemap, string ignoreTag, float delta = 8.0f)
        {
            Vector2 deltaVec = (to - from);
            deltaVec.Normalize();
            deltaVec *= delta;

            //Setup the ray if it hase not been done before
            if(_raycastCollider == null)
            {
                _raycastCollider = new BoxCollider(new Bird(), Vector2.One);  // Just need a WorldEntity that has no collider for this
                _raycastCollider.Active = false;
                _raycastCollider.Entity.Enabled = false;
                
            }
            _raycastCollider.Entity.Position = from;

            float prevDistance = float.MaxValue;
            float distance = Vector2.DistanceSquared(_raycastCollider.Entity.Position, to);

            // If current distance is closer than we have been before
            while(distance < prevDistance)
            {
                // Check if we collide with anything
                if(_raycastCollider.CollidesAny(out firstHit, out hitTilemap))
                {
                    // If we did not hit an entity we should ignore
                    if(hitTilemap || firstHit.Entity.Tag != ignoreTag)
                    {
                        return true;
                    }
                }

                // Keep moving the ray forward and update the prev/current distance
                _raycastCollider.Entity.Move(deltaVec);
                prevDistance = distance;
                distance = Vector2.DistanceSquared(_raycastCollider.Entity.Position, to);
            }

            // If nothing was hit, return false
            firstHit = null;
            hitTilemap = false;
            return false;
        }

        /// <summary>
        /// Check if there is an overlap between a circle collider and the tilemap
        /// </summary>
        /// <param name="circle">Collider a</param>
        /// <param name="level">The tilemap</param>
        /// <returns>Returns true if an overlap is found</returns>
        public static bool CircleTilemapOverlap(CircleCollider circle, Level level)
        {
            // I'm not doing this, everything's a box now
            return BoxTilemapOverlap(circle, level);
        }

        /// <summary>
        /// Check if there is an overlap between a circle collider and the tilemap
        /// </summary>
        /// <param name="box">Collider a</param>
        /// <param name="level">The tilemap</param>
        /// <returns>Returns true if an overlap is found</returns>
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
                    if((level.Tiles[x + y * level.Width]?.Walkable) != true)
                    {
                        return true;
                    }
                }
            }

            return false;
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
                if(other == col || other.Active == false || other.Entity.Enabled == false || other.Trigger || other == _raycastCollider)
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
