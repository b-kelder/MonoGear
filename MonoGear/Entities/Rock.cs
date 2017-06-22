using Microsoft.Xna.Framework;
using System;
using MonoGear.Engine;
using MonoGear.Engine.Collisions;

namespace MonoGear.Entities
{
    class Rock : WorldEntity
    {
        /// <summary>
        /// Property for the speed at which the rock flies.
        /// </summary>
        float Speed { get; set; }
        Collider originCollider;

        /// <summary>
        /// Contructor of the rock class. Creates an instance of a rock.
        /// </summary>
        /// <param name="originCollider">Origin collider</param>
        public Rock(Collider originCollider)
        {
            CircleCollider collider = new CircleCollider(this, 2);
            collider.Trigger = true;

            // Speed in units/sec. Right now 1 unit = 1 pixel
            Random rand = new Random();
            Speed = 200f + rand.Next(-20, 20);
            TextureAssetName = "Sprites/Rock";
            Tag = "TheRock";

            Z = 5;
            LoadContent();

            this.originCollider = originCollider;
        }

        /// <summary>
        /// Method that updates the game
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="gameTime">GameTime</param>
        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);

            Collider collider;
            var pos = Position;
            var delta = Forward * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Move(delta);

            bool hitTilemap;

            // Check if the rock collides with anything
            if(Collider.CollidesAny(out collider, out hitTilemap, originCollider))
            {
                Position = pos;
                Speed = 0.0f;

                var entities = MonoGearGame.FindEntitiesOfType<Guard>();

                // Loop through all guards
                foreach(var guard in entities)
                {
                    // Check if the guard is in range
                    if(Vector2.Distance(Position, guard.Position) < 150)
                    {
                        guard.Interest(Position);
                    }
                }

                Enabled = false;
            }

            if (Speed > 0)
            {
                Speed -= 3;
            }
            else
            {
                Speed = 0;
            }
        }
    }
}
