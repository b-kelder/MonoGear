using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGear.Engine.Audio;
using MonoGear.Engine.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoGear.Engine;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGear.Entities.Vehicles
{
    /// <summary>
    /// Player controlled tank. Is used in the credits scene as well without player control.
    /// </summary>
    class Tank : DrivableVehicle
    {
        /// <summary>
        /// Engine sound
        /// </summary>
        private PositionalAudio tankSound;
        /// <summary>
        /// Last time cannon was shot
        /// </summary>
        private float lastShootTime;
        /// <summary>
        /// Indicates if we are in credits mode
        /// </summary>
        public bool creditsMode;

        public float GunCycleTime { get; set; }

        public Tank()
        {
            TextureAssetName = "Sprites/Abrams";
            Tag = "Tank";
            Speed = 135;
            Entered = false;
            stationaryLock = false;

            Z = 2;

            GunCycleTime = 0.8f;

            destroyed = false;

            Health = 100;

            Acceleration = 70;
            Braking = 140;
            Steering = 60;
            Drag = 70;
            ConstantSteering = true;        // Tank can turn at the spot

            Collider = new BoxCollider(this, new Vector2(24, 24));

            LoadContent();
        }

        /// <summary>
        /// Called when level is loaded or entity is added to scene
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
            if(creditsMode)
            {
                // Fire at player jeep once
                if(Vector2.Distance(player.Position, Position) < 250)
                {
                    FireCannon();
                    creditsMode = false;
                }
            }

            // Input and movement handling
            base.Update(input, gameTime);

            if (destroyed)
            {
                AudioManager.StopPositional(tankSound);
            }


            float minVolume = 0.1f;
            if(Entered)
            {
                // Speed based sound
                tankSound.Position = Position;
                tankSound.Volume = minVolume + (0.2f - minVolume) * Math.Abs(forwardSpeed) / Speed;

                // Player controlled cannon
                if(input.IsButtonPressed(Input.Button.Shoot) && lastShootTime + GunCycleTime <= (float)gameTime.TotalGameTime.TotalSeconds)
                {
                    lastShootTime = (float)gameTime.TotalGameTime.TotalSeconds;
                    FireCannon();
                }

                // Player controlled MG
                if (input.IsButtonPressed(Input.Button.Throw))
                {
                    var bullet = new Bullet(Collider);
                    bullet.Rotation = Rotation;

                    bullet.Position = Position + Forward * 18 + Right * 10;

                    MonoGearGame.SpawnLevelEntity(bullet);

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
        /// Fires tank main gun by spaning a missile
        /// </summary>
        public void FireCannon()
        {
            var missile = new Missile(MonoGearGame.FindEntitiesOfType<Player>()[0].Collider);
            missile.Rotation = Rotation;

            missile.Position = Position + Forward * 88;

            MonoGearGame.SpawnLevelEntity(missile);

            var sound = MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Tank_shot").CreateInstance();
            sound.Volume = 0.5f * SettingsPage.Volume * SettingsPage.EffectVolume;
            sound.Play();
        }
    }
}

