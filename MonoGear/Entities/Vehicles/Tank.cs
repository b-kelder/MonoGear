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
    class Tank : DrivableVehicle
    {
        private PositionalAudio tankSound;
        private float lastShootTime;

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
            ConstantSteering = true;

            Collider = new BoxCollider(this, new Vector2(24, 24));

            LoadContent();
        }

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

            if (destroyed)
            {
                AudioManager.StopPositional(tankSound);
            }

            float minVolume = 0.1f;
            if(Entered)
            {
                tankSound.Position = Position;
                tankSound.Volume = minVolume + (0.2f - minVolume) * Math.Abs(forwardSpeed) / Speed;


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
    }
}

