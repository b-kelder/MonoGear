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
using Microsoft.Xna.Framework.Media;

namespace MonoGear.Entities.Vehicles
{
    /// <summary>
    /// Willys jeep, player controlled vehicle
    /// </summary>
    ///
    class Jeep : DrivableVehicle
    {
        public bool autoenter;
        private PositionalAudio jeepSound;
        private Texture2D playerSprite;
        private Texture2D jeepSprite;
        private bool creditsMode;

        public Jeep()
        {
            TextureAssetName = "Sprites/Willys";
            Tag = "Willys";
            Speed = 230;
            Entered = false;
            stationaryLock = false;

            Z = 1;

            Health = 20;

            Acceleration = 80;
            Braking = 200;
            Steering = 180;
            Drag = 50;

            Collider = new BoxCollider(this, new Vector2(24, 24));

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
            destroyedSprite = MonoGearGame.GetResource<Texture2D>("Sprites/BrokenWillys");
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

            if (creditsMode)
            {
                Health = 100;
                //Disable game entity's
                if (MonoGearGame.FindEntitiesOfType<GameUI>()[0].Enabled)
                {
                    MonoGearGame.FindEntitiesOfType<GameUI>()[0].Enabled = false;
                    MonoGearGame.FindEntitiesOfType<GameUI>()[0].Visible = false;

                    MonoGearGame.SpawnLevelEntity(new Credits());
                }

                Rotation = MathHelper.ToRadians(90);
                forwardSpeed = Speed;
                jeepSound.Volume = 0;
            }
            else
            {
                if (destroyed)
                {
                    AudioManager.StopPositional(jeepSound);
                }

                float minVolume = 0.75f;
                if (Entered)
                {
                    if (instanceTexture != playerSprite)
                    {
                        instanceTexture = playerSprite;
                    }
                    jeepSound.Volume = minVolume + (1.0f - minVolume) * Math.Abs(forwardSpeed) / Speed;
                    creditsMode = true;
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
        }
    }
}