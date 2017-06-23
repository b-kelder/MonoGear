using Microsoft.Xna.Framework;
using System;
using MonoGear.Engine;
using MonoGear.Engine.Collisions;

namespace MonoGear.Entities
{
    class Bullet : WorldEntity
    {
        float Speed { get; set; }
        Collider originCollider;
        private float maxDamage;

        /// <summary>
        /// Constructor of the bullet class. Creates an instance of a bullet.
        /// </summary>
        /// <param name="originCollider">The bullet's origin collider</param>
        public Bullet(Collider originCollider)
        {
            CircleCollider collider = new CircleCollider(this, 1);
            collider.Trigger = true;

            Z = 5;

            // Speed in units/sec. Right now 1 unit = 1 pixel
            Random rand = new Random();
            Speed = 350f;
            TextureAssetName = "Sprites/Bullet";
            Tag = ".50 HEIAP";
            LoadContent();

            maxDamage = 3;

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

            // Check if the bullet collides with anything
            if (Collider.CollidesAny(out collider, out hitTilemap, originCollider))
            {
                Position = pos;
                // Set the speed to 0
                Speed = 0.0f;

                // Check if the bullet collides with a player
                if (!hitTilemap && collider.Entity.Tag == "Player")
                {
                    var player = collider.Entity as Player;
                    // Decrease the player's health by 1
                    player.Health -= 1.0f;
                }

                // Find everything that can be destroyed
                var things = MonoGearGame.FindEntitiesOfType<IDestroyable>();
                foreach (var thing in things)
                {
                    // Damage the thing if it gets hit
                    var dis = thing as WorldEntity;
                    if (!hitTilemap && collider.Entity == dis)
                    {
                        thing.Damage(maxDamage);
                    }
                }

                // Destroy if we hit something
                MonoGearGame.DestroyEntity(this);
            }
        }
    }
}
