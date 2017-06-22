using Microsoft.Xna.Framework;
using MonoGear.Engine;


namespace MonoGear.Entities
{
    class SpawnPoint : WorldEntity
    {
        /// <summary>
        /// Constructor of the spawn point class.
        /// </summary>
        /// <param name="position">The position of the spawn point</param>
        public SpawnPoint(Vector2 position)
        {
            Position = position;
            Tag = "SpawnPoint";
        }
    }
}
