using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MonoGear
{
    class GameOver : WorldEntity
    {
        private bool gameOver;
        private Player player;
        private Texture2D gameOverSprite;

        public GameOver()
        {
            TextureAssetName = "Sprites/blank";
            Tag = "GameOverScreen";
            gameOver = false;
            Visible = false;

            Z = 999;

            LoadContent();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            if (gameOverSprite == null)
            {
                gameOverSprite = ResourceManager.GetManager().GetResource<Texture2D>("Sprites/gameover");
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (gameOver)
            {
                spriteBatch.Draw(gameOverSprite, new Vector2(Position.X, Position.Y - 16), gameOverSprite.Bounds, Color.White, 0, new Vector2(gameOverSprite.Bounds.Size.X, gameOverSprite.Bounds.Size.Y) / 2, 0.3f, SpriteEffects.None, 0);
            }
        }

        public void EnableGameOver()
        {
            player = MonoGearGame.FindEntitiesWithTag("Player")[0] as Player;
            gameOver = true;
            Position = player.Position;
            Visible = true;
            player.Enabled = false;
        }

        public void DisableGameOver()
        {
            gameOver = false;
            Visible = false;
            player.Enabled = true;
        }

        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);

            if (gameOver)
            {
                if(input.IsKeyPressed(Keys.Space))
                {
                    var frame = Window.Current.Content as Frame;
                    frame.Navigate(typeof(MenuPage));
                }
            }
        }
    }
}

