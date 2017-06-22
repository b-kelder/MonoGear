using Microsoft.Xna.Framework;

namespace MonoGear.Engine.Collisions
{
    /// <summary>
    /// Circle shaped collider.
    /// </summary>
    public class CircleCollider : Collider
    {
        private float radius;
        /// <summary>
        /// Collision radius
        /// </summary>
        public float Radius
        {
            get { return radius; }
            set
            {
                radius = value;
                BBSize = new Vector2(radius * 2);
            }
        }

        /// <summary>
        /// Constructor of the circle collider class.
        /// </summary>
        /// <param name="entity">World Entity</param>
        /// <param name="radius">Radius of the collidor</param>
        public CircleCollider(WorldEntity entity, float radius) : base(entity)
        {
            Radius = radius;
        }

        /// <summary>
        /// Check if there is an collision
        /// </summary>
        /// <param name="other">The other object</param>
        /// <returns>True or false based on if there was a collision or not</returns>
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
