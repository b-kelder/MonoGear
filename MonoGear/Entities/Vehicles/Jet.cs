using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using MonoGear.Engine;
using MonoGear.Engine.Collisions;
using MonoGear.Engine.Audio;
using System.Diagnostics;

namespace MonoGear.Entities.Vehicles
{
    /// <summary>
    /// Willys jeep, player controlled vehicle
    /// </summary>
    ///
    class Jet : DrivableVehicle, IDestroyable
    {
        private PositionalAudio willysSound;
        private Texture2D destoyedSprite;

        public float Health { get; private set; }

        public Jet()
        {
            TextureAssetName = "Sprites/F-35";
            Tag = "Jet";
            Speed = 1500;
            entered = false;
            stationaryLock = false;

            Z = 10;

            Health = 20;

            Acceleration = 150;
            Braking = 0;
            Steering = 100;
            Drag = 60;

            LoadContent();
        }

        /// <summary>
        /// Method that executes when the level is loaded.
        /// </summary>
        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();
            willysSound = AudioManager.AddPositionalAudio(MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Car_sound"), 0, 300, Position, true);
            destoyedSprite = MonoGearGame.GetResource<Texture2D>("Sprites/BrokenWillys");
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
                willysSound.Position = Position;
                willysSound.Volume = minVolume + (1.0f - minVolume) * Math.Abs(forwardSpeed) / Speed;
            }
            else
            {
                willysSound.Volume = minVolume;
            }

            willysSound.Position = Position;
        }

        public void Damage(float damage)
        {
            Health -= damage;

            if (Health <= 0)
            {
                Destroy();
            }
        }

        public void Destroy()
        {
            if(entered)
            {
                Exit();   
            }

            instanceTexture = destoyedSprite;
            Enabled = false;

            AudioManager.StopPositional(willysSound);
        }
    }
}