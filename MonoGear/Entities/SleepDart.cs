using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGear.Engine;
using MonoGear.Engine.Collisions;

namespace MonoGear.Entities
{
    class SleepDart : WorldEntity
    {
        /// <summary>
        /// Property that indicates the speed at which the sleep dart flies.
        /// </summary>
        float Speed { get; set; }
        Collider originCollider;

        /// <summary>
        /// Constructor of the sleep dart class. Creates an instance of a sleep dart.
        /// </summary>
        public SleepDart(Collider originCollider)
        {
            CircleCollider collider = new CircleCollider(this, 2);
            collider.Trigger = true;

            // Speed in units/sec. Right now 1 unit = 1 pixel
            Speed = 250f;
            TextureAssetName = "Sprites/SleepDart";
            Tag = "SleepDart";

            Z = 5;
            LoadContent();

            this.originCollider = originCollider;
        }

        /// <summary>
        /// Method that updates the game.
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
            // Check if the sleep dart collides with anything
            if(Collider.CollidesAny(out collider, out hitTilemap, originCollider))
            {
                Position = pos;
                // Set the speed to 0
                Speed = 0.0f;
                // Check if the sleep dart collides with a guard
                if(collider != null && collider.Entity.Tag.Equals("Guard"))
                {
                    var guard = collider.Entity as Guard;
                    guard.Sleep();
                    var sound = MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/DartHit").CreateInstance();
                    sound.Volume = 1 * SettingsPage.Volume * SettingsPage.EffectVolume;
                    sound.Play();
                    MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/HurtSound").CreateInstance();
                    sound.Volume = 1 * SettingsPage.Volume * SettingsPage.EffectVolume;
                    sound.Play();
                    MonoGearGame.DestroyEntity(this);
                }
                // Disable the sleep dart
                Enabled = false;
            }
        }
    }
}

