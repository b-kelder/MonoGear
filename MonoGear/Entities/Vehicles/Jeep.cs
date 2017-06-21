using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGear.Engine;
using MonoGear.Engine.Collisions;
using MonoGear.Engine.Audio;

namespace MonoGear.Entities.Vehicles
{
    /// <summary>
    /// Willys jeep, player controlled vehicle
    /// </summary>
    class Jeep : DrivableVehicle, IDestroyable
    {
        public bool autoenter;
        private PositionalAudio jeepSound;
        private Texture2D playerSprite;
        private Texture2D jeepSprite;
        private Texture2D destoyedSprite;

        /// <summary>
        /// Property with the jeep's health.
        /// </summary>
        public float Health { get; private set; }

        /// <summary>
        /// Constructor of the jeep class. Creates an instance of a jeep.
        /// </summary>
        public Jeep()
        {
            TextureAssetName = "Sprites/Willys";
            Tag = "Willys";
            Speed = 230;
            entered = false;
            stationaryLock = false;

            Z = 1;

            Health = 20;

            Acceleration = 80;
            Braking = 200;
            Steering = 180;
            Drag = 50;

            Collider = new BoxCollider(this, new Vector2(24,24));

            LoadContent();
        }

        /// <summary>
        /// Method that executes when the level is loaded.
        /// </summary>
        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();
            jeepSound = AudioManager.AddPositionalAudio(MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Car_sound"), 0, 300, Position, true);
            playerSprite = MonoGearGame.GetResource<Texture2D>("Sprites/WillysPlayer"); 
            destoyedSprite = MonoGearGame.GetResource<Texture2D>("Sprites/BrokenWillys");
            jeepSprite = MonoGearGame.GetResource<Texture2D>("Sprites/Willys");

            if (autoenter)
            {
                Enter();
            }

        }

        /// <summary>
        /// Method that updates the game
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="gameTime">GameTime</param>
        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);

            float minVolume = 0.75f;
            if(entered)
            {
                if (instanceTexture != playerSprite)
                {
                    instanceTexture = playerSprite;
                }
                jeepSound.Volume = minVolume + (1.0f - minVolume) * Math.Abs(forwardSpeed) / Speed;
            }
            else
            {
                if (instanceTexture != jeepSprite)
                {
                    instanceTexture = jeepSprite;
                }
                jeepSound.Volume = minVolume;
            }

            jeepSound.Position = Position;
        }

        /// <summary>
        /// Method is executed when the jeep is damaged.
        /// </summary>
        /// <param name="damage">The amount of damage taken</param>
        public void Damage(float damage)
        {
            Health -= damage;
            // Check if health is 0 or smaller
            if (Health <= 0)
            {
                // Destroy the jeep
                Destroy();
            }
        }

        /// <summary>
        /// Method that destroys the jeep.
        /// </summary>
        public void Destroy()
        {
            if(entered)
            {
                Exit();   
            }

            instanceTexture = destoyedSprite;
            Enabled = false;
            // Stop the jeep sound
            AudioManager.StopPositional(jeepSound);
        }
    }
}