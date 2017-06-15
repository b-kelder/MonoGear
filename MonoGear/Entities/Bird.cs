﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGear.Engine;
using MonoGear.Engine.Audio;

namespace MonoGear.Entities
{
    /// <summary>
    /// Bird that flies towards the top of the screen.
    /// </summary>
    class Bird : WorldEntityAnimated, IDestroyable
    {
        float speed;

        public float YResetValue { get; set; }
        public float Health { get; private set; }

        PositionalAudio birdSound;
        private Texture2D destroyedSprite;
        private bool destroyed;

        /// <summary>
        /// Constructor of the bird class. Creates an instance of a bird.
        /// </summary>
        public Bird()
        {
            // Speed in units/sec. Right now 1 unit = 1 pixel
            speed = 100.0f;
            // Bird texture
            TextureAssetName = "Sprites/birdsheet";

            AnimationLength = 3;
            AnimationCurrentFrame = 1;
            AnimationDelta = 0.05f;
            AnimationPingPong = true;
            AnimationRunning = true;

            destroyed = false;

            Tag = "ObeseHummingbird";
            Health = 1;

            Z = 100;

            LoadContent();
        }

        /// <summary>
        /// Method that executes when the level is loaded.
        /// </summary>
        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();
            birdSound = AudioManager.AddPositionalAudio(MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Bird_sounds"), 1, 210, Position,true);
            birdSound.Volume = 0.2f;

            destroyedSprite = MonoGearGame.GetResource<Texture2D>("Sprites/DeadBird");
        }

        /// <summary>
        /// Method that executes when the level is unloaded.
        /// </summary>
        public override void OnLevelUnloaded()
        {
            base.OnLevelUnloaded();
        }

        /// <summary>
        /// Method that updates the game
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="gameTime">GameTime</param>
        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);
            //Check if the Y position is lower than -200
            if(Position.Y < -200)
            {
                // Reset the bird's position
                Move(new Vector2(0, YResetValue));
            }
            // Move the bird
            Move(new Vector2(0, -speed * (float)gameTime.ElapsedGameTime.TotalSeconds));
            // Set the bird's position to the new position
            birdSound.Position = Position;
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
            instanceTexture = destroyedSprite;

            AnimationRunning = false;

            destroyed = true;
            Enabled = false;
        }
    }
}

