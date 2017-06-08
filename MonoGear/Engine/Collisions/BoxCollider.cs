using Microsoft.Xna.Framework;

namespace MonoGear.Engine.Collisions
{
    public class BoxCollider : Collider
    {
        /// <summary>
        /// Property for the size of the BoxCollider
        /// </summary>
        public Vector2 Size
        {
            get { return BBSize; }
            set { BBSize = value; }
        }

        /// <summary>
        /// BoxCollider's constructor.
        /// </summary>
        /// <param name="entity">WorldEntity</param>
        /// <param name="size">Size of the BoxCollider</param>
        public BoxCollider(WorldEntity entity, Vector2 size) : base(entity)
        {
            // Set the size property to the given size
            Size = size;
        }

        /// <summary>
        /// Method that checks if there is a collision with another collider.
        /// </summary>
        /// <param name="other">The other collider</param>
        /// <returns>True or false based on if there is a collision or not</returns>
        public override bool Collides(Collider other)
        {
            // Check if there is an overlap between this collider and the other collider
            if(BoxOverlap(this, other))
            {
                var circle = other as CircleCollider;
                // Check if the otehr collider is a CircleCollider
                if(circle != null)
                {
                    return CircleBoxOverlap(circle, this);
                }
                // The two colliders collide with each other; return true
                return true;
            }
            // The two colliders don't collide with each other; return false
            return false;
        }
    }
}
