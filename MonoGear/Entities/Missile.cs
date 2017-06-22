using Microsoft.Xna.Framework;
using MonoGear.Engine;
using MonoGear.Engine.Collisions;

namespace MonoGear.Entities
{
    class Missile : WorldEntity
    {
        /// <summary>
        /// Property that indicates the speed at which the missile flies.
        /// </summary>
        float Speed { get; set; }
        Collider originCollider;
        private float boemInSec;

        /// <summary>
        /// Constructor of the missile class. Creates an instance of a missile.
        /// </summary>
        public Missile(Collider originCollider)
        {
            CircleCollider collider = new CircleCollider(this, 2);
            collider.Trigger = true;

            Speed = 800f;
            boemInSec = 1f;
            TextureAssetName = "Sprites/Missile";
            Tag = "Missile";
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
            
            var pos = Position;
            var delta = Forward * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Move(delta);

            if (boemInSec > 0)
                boemInSec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Collider.CollidesAny() || boemInSec <= 0)
            {
                Position = pos;
                Speed = 0.0f;

                MonoGearGame.SpawnLevelEntity(new Explosion() { Position = this.Position });
                MonoGearGame.DestroyEntity(this);
            }
        }
    }
}

