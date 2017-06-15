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
        private Texture2D destroyedSprite;

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
            destroyedSprite = MonoGearGame.GetResource<Texture2D>("Sprites/BrokenWillys");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (entered)
            {
                spriteBatch.Draw(playerSprite, Position, playerSprite.Bounds, Color.White, Rotation, new Vector2(playerSprite.Bounds.Size.X, playerSprite.Bounds.Size.Y) / 2, 1, SpriteEffects.None, 0);
            }
            if(destroyed)
            {
                spriteBatch.Draw(destroyedSprite, Position, destroyedSprite.Bounds, Color.White, Rotation, new Vector2(destroyedSprite.Bounds.Size.X, destroyedSprite.Bounds.Size.Y) / 2, 1, SpriteEffects.None, 0);
            }

            if (!destroyed)
            {
                spriteBatch.DrawString(MonoGearGame.GetResource<SpriteFont>("Fonts/Arial"), "HP: " + Health, Position - Vector2.One * 16, Color.Red);
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

            destroyed = true;
            Enabled = false;

            AudioManager.StopPositional(willysSound);
        }
    }
}