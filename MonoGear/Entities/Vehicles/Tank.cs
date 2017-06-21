using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGear.Engine.Audio;
using MonoGear.Engine.Collisions;
using System;
using MonoGear.Engine;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGear.Entities.Vehicles
{
    /// <summary>
    /// Tank, player controlled vehicle
    /// </summary>
    class Tank : DrivableVehicle, IDestroyable
    {
        private PositionalAudio tankSound;
        private float lastShootTime;
        private bool destroyed;
        private Texture2D destroyedSprite;

        public float GunCycleTime { get; set; }
        /// <summary>
        /// Property with the jet's health.
        /// </summary>
        public float Health { get; private set; }

        /// <summary>
        /// Constructor of the tank class. Creates an instance of a tank.
        /// </summary>
        public Tank()
        {
            TextureAssetName = "Sprites/Abrams";
            Tag = "Tank";
            Speed = 135;
            entered = false;
            stationaryLock = false;

            Z = 2;

            GunCycleTime = 0.8f;

            destroyed = false;

            Health = 100;

            Acceleration = 70;
            Braking = 140;
            Steering = 60;
            Drag = 70;
            ConstantSteering = true;

            Collider = new BoxCollider(this, new Vector2(24, 24));

            LoadContent();
        }

        /// <summary>
        /// Method that executes when the level is loaded.
        /// </summary>
        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();

            tankSound = AudioManager.AddPositionalAudio(MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Tank Movement"), 0, 250, Position, true);
            destroyedSprite = MonoGearGame.GetResource<Texture2D>("Sprites/BrokenAbrams");
        }

        /// <summary>
        /// Method that updates the game.
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="gameTime">GameTime</param>
        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);

            float minVolume = 0.1f;
            if(entered)
            {
                tankSound.Position = Position;
                tankSound.Volume = minVolume + (0.2f - minVolume) * Math.Abs(forwardSpeed) / Speed;
                // Check if the shoot button is pressed and the gun cycle time has finished
                if(input.IsButtonPressed(Input.Button.Shoot) && lastShootTime + GunCycleTime <= (float)gameTime.TotalGameTime.TotalSeconds)
                {
                    var missile = new Missile(MonoGearGame.FindEntitiesOfType<Player>()[0].Collider);
                    missile.Rotation = Rotation;

                    missile.Position = Position + Forward * 88;

                    MonoGearGame.SpawnLevelEntity(missile);

                    lastShootTime = (float)gameTime.TotalGameTime.TotalSeconds;

                    var sound = MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Tank_shot").CreateInstance();
                    sound.Volume = 0.5f * SettingsPage.Volume * SettingsPage.EffectVolume;
                    sound.Play();
                }
                // Check if the throw button is pressed
                if (input.IsButtonPressed(Input.Button.Throw))
                {
                    var bullet = new Bullet(Collider);
                    bullet.Rotation = Rotation;

                    bullet.Position = Position + Forward * 18 + Right * 10;

                    MonoGearGame.SpawnLevelEntity(bullet);

                    lastShootTime = (float)gameTime.TotalGameTime.TotalSeconds;

                    var sound = MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Tank_gatling").CreateInstance();
                    sound.Volume = 0.5f * SettingsPage.Volume * SettingsPage.EffectVolume;
                    sound.Play();
                }

            }
            else
            {
                tankSound.Volume = minVolume;
            }
        }

        /// <summary>
        /// Method is executed when the tank is damaged.
        /// </summary>
        /// <param name="damage">The amount of damage taken</param>
        public void Damage(float damage)
        {
            Health -= damage;
            // Check if health is 0 or smaller
            if (Health <= 0)
            {
                // Destroy the tank
                Destroy();
            }
        }

        /// <summary>
        /// Method that destroys the tank.
        /// </summary>
        public void Destroy()
        {
            if (entered)
            {
                Exit();
            }

            destroyed = true;
            Enabled = false;
            instanceTexture = destroyedSprite;
            // Stop the tank sound
            AudioManager.StopPositional(tankSound);
        }
    }
}

