using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using MonoGear.Engine;
using System;

namespace MonoGear.Entities
{
    class GameOver : WorldEntity
    {
        public bool gameOver;
        private Player player;
        private Texture2D gameOverSprite;

        /// <summary>
        /// Constructor of the game over class. Creates a game over screen.
        /// </summary>
        public GameOver()
        {
            TextureAssetName = "Sprites/blank";
            Tag = "GameOverScreen";
            gameOver = true;
            Visible = false;

            Z = Int32.MaxValue;

            LoadContent();
        }

        /// <summary>
        /// Method that loads the content.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            // Load sprite
            if (gameOverSprite == null)
            {
                gameOverSprite = MonoGearGame.GetResource<Texture2D>("Sprites/gameover");
            }

            // Find the player
            player = MonoGearGame.FindEntitiesWithTag("Player")[0] as Player;
        }

        /// <summary>
        /// Method that draws the game over screen.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // Check if the game is over
            if (gameOver)
            {
                var clipRect = Camera.main.GetClippingRect();
                spriteBatch.Draw(gameOverSprite, clipRect, Color.White);
            }
        }
        /// <summary>
        /// Method used to enable the gameover screen
        /// </summary>
        public void EnableGameOver()
        {
            gameOver = true;
            Position = player.Position;
            Visible = true;
            player.Enabled = false;

            var sound = MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Wasted_sound").CreateInstance();
            sound.Volume = 0.5f * SettingsPage.Volume * SettingsPage.EffectVolume;
            sound.Play();
        }

        /// <summary>
        /// Method used to disable the gameover screen
        /// </summary>
        public void DisableGameOver()
        {
            gameOver = false;
            Visible = false;
            player.Enabled = true;
            player.Visible = true;
            MonoGearGame.FindEntitiesOfType<GameUI>()[0].Enabled = true;
            MonoGearGame.FindEntitiesOfType<GameUI>()[0].Visible = true;
        }

        /// <summary>
        /// Method that updates the game
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="gameTime">GameTime</param>
        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);

            // Check if the game is over
            if (gameOver)
            {
                // Check if space is pressed
                if(input.IsKeyPressed(Keys.Space))
                {
                    // Restart the game
                    MonoGearGame.Restart();
                }
            }
        }
    }
}

