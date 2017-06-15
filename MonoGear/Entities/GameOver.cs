using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using MonoGear.Engine;



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

            Z = 999;

            LoadContent();
        }

        /// <summary>
        /// Method that loads the content.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            if (gameOverSprite == null)
            {
                gameOverSprite = MonoGearGame.GetResource<Texture2D>("Sprites/gameover");
            }
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

        public void EnableGameOver()
        {
            player = MonoGearGame.FindEntitiesWithTag("Player")[0] as Player;
            gameOver = true;
            Position = player.Position;
            Visible = true;
            player.Enabled = false;
            var sound = MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Wasted_sound").CreateInstance();
            sound.Volume = 0.5f * SettingsPage.Volume * SettingsPage.EffectVolume;
            sound.Play();
        }

        public void DisableGameOver()
        {
            player = MonoGearGame.FindEntitiesWithTag("Player")[0] as Player;
            gameOver = false;
            Visible = false;
            player.Enabled = true;

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

