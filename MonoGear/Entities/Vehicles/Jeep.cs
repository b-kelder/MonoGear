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
        public bool creditsMode;

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
            jeepSound = AudioManager.AddPositionalAudio(MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Car"), 0, 300, Position, true);
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
            if(creditsMode)
            {
                //Disable game entities
                if(MonoGearGame.FindEntitiesOfType<GameUI>()[0].Enabled)
                {
                    MonoGearGame.FindEntitiesOfType<GameUI>()[0].Enabled = false;
                    MonoGearGame.FindEntitiesOfType<GameUI>()[0].Visible = false;

                    MonoGearGame.SpawnLevelEntity(new Credits());

                    instanceTexture = playerSprite;
                    Health = 1;
                }

                // Move right on the screen
                Position += Speed * new Vector2(1, 0) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Rotation = MathHelper.ToRadians(90);
                forwardSpeed = Speed;
                jeepSound.Volume = 0;
                player.Position = Position;

                // Camera tracking
                float endXPos = 28000;
                if(Position.X < endXPos)
                {
                    Camera.main.Position = Position;
                }

                if(input.IsButtonPressed(Input.Button.Right))
                {
                    Speed *= 2;
                }
                if(input.IsButtonPressed(Input.Button.Left))
                {
                    Speed /= 2;
                }

                return;
            }

            // Handles driving and input
            base.Update(input, gameTime);

            if(destroyed)
            {
                AudioManager.StopPositional(jeepSound);
            }

            float minVolume = 0.75f;
            if(Entered)
            {
                if(instanceTexture != playerSprite)
                {
                    instanceTexture = playerSprite;
                }
                jeepSound.Volume = minVolume + (1.0f - minVolume) * Math.Abs(forwardSpeed) / Speed;
            }
            else
            {
                if(instanceTexture != jeepSprite)
                {
                    instanceTexture = jeepSprite;
                }
                jeepSound.Volume = minVolume;
            }

            jeepSound.Position = Position;
        }

        public override void Destroy()
        {
            if(Entered)
            {
                Exit();
            }

            instanceTexture = destroyedSprite;
            Enabled = false;
            destroyed = true;

            if(creditsMode)
            {
                // Car is off-screen, restart
                var go = new GameOver();
                MonoGearGame.SpawnLevelEntity(go);
                Exit();
                go.EnableGameOver();
                player.Enabled = false;
                player.Visible = false;
            }
        }
    }
}