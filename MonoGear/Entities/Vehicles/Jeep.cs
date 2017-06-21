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
        /// <summary>
        /// Indicates player should enter on level start
        /// </summary>
        public bool autoenter;
        /// <summary>
        /// Engine sound
        /// </summary>
        private PositionalAudio jeepSound;
        /// <summary>
        /// Sprite of jeep with player
        /// </summary>
        private Texture2D playerSprite;
        /// <summary>
        /// Sprite of empty jeep
        /// </summary>
        private Texture2D jeepSprite;
        /// <summary>
        /// Used to indicate that we are in credit mode (the credits scene)
        /// </summary>
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

            // Must be called to ensure content is loaded
            LoadContent();
        }

        /// <summary>
        /// Method that executes when the level is loaded.
        /// </summary>
        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();

            jeepSound = AudioManager.AddPositionalAudio(MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Car"), 0, 300, Position, true);
            jeepSound.Volume = 0.1f;

            playerSprite = MonoGearGame.GetResource<Texture2D>("Sprites/WillysPlayer");
            destroyedSprite = MonoGearGame.GetResource<Texture2D>("Sprites/BrokenWillys");
            jeepSprite = MonoGearGame.GetResource<Texture2D>("Sprites/Willys");

            // Enter on level load, for example heli chase scene
            if(autoenter)
            {
                Enter();
            }
        }

        /// <summary>
        /// Method called every frame
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="gameTime">GameTime</param>
        public override void Update(Input input, GameTime gameTime)
        {
            if(creditsMode)
            {
                //One time changes
                if(MonoGearGame.FindEntitiesOfType<GameUI>()[0].Enabled)
                {
                    // Disable UI, spawn credits and set other things
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
                // Required for tank range detection
                player.Position = Position;

                // Camera tracking
                if(Position.X < 28000)
                {
                    Camera.main.Position = Position;
                }

                return;
            }

            // Handles driving and input
            base.Update(input, gameTime);

            // Speed based volume
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
            // Throw the player out
            if(Entered)
            {
                Exit();
            }

            instanceTexture = destroyedSprite;
            Enabled = false;
            destroyed = true;
            AudioManager.StopPositional(jeepSound);

            if(creditsMode)
            {
                // Car has exploded, show restart screen
                var go = new GameOver();
                MonoGearGame.SpawnLevelEntity(go);
                Exit();
                go.EnableGameOver();
                player.Enabled = false;                 // Ensure camera doesn't snap to player next frame
                player.Visible = false;                 // Ensure player remains invisible
            }
        }
    }
}