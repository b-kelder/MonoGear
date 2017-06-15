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

namespace MonoGear.Entities
{
    /// <summary>
    /// Willys jeep, player controlled vehicle
    /// </summary>
    ///
    class Willys : DrivableVehicle, IDestroyable
    {
        private PositionalAudio willysSound;
        private bool destroyed;
        private Texture2D playerSprite;
        private Texture2D willysSprite;
        private Texture2D destoyedSprite;

        public float Health { get; private set; }

        public Willys()
        {
            TextureAssetName = "Sprites/Willys";
            Tag = "Willys";
            Speed = 230;
            entered = false;
            destroyed = false;
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
            willysSound = AudioManager.AddPositionalAudio(MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Car_sound"), 0, 300, Position, true);
            playerSprite = MonoGearGame.GetResource<Texture2D>("Sprites/WillysPlayer"); 
            destoyedSprite = MonoGearGame.GetResource<Texture2D>("Sprites/BrokenWillys");
            willysSprite = MonoGearGame.GetResource<Texture2D>("Sprites/Willys");

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
                willysSound.Position = Position;
                willysSound.Volume = minVolume + (1.0f - minVolume) * Math.Abs(forwardSpeed) / Speed;
            }
            else
            {
                if (instanceTexture != willysSprite)
                {
                    instanceTexture = willysSprite;
                }
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

            destroyed = true;
            Enabled = false;

            AudioManager.StopPositional(willysSound);
        }
    }
}