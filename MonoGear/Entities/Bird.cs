using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
        PositionalAudio birdSound;

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

            Tag = "ObeseHummingbird";

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

        public void Damage()
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }
    }
}
