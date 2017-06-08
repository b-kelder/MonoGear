using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace MonoGear
{
    public class GameUI : WorldEntity
    {
        Player player;

        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();

            player = MonoGearGame.FindEntitiesWithTag("Player")[0] as Player;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var rect = Camera.main.GetClippingRect();
            var pos = rect.Right - 100;
            var rows = Math.Ceiling(player.Health / 5);
            var healthToDraw = player.Health;
            var h = 5.0f;

            for (int j = 0; j < rows; j++)
            {
                if (healthToDraw < 5)
                    h = healthToDraw;

                for (int i = 0; i < h; i++)
                {
                    spriteBatch.Draw(MonoGearGame.GetResource<Texture2D>("Sprites/Heart"), new Vector2(pos, rect.Bottom - (50 + (j * 18))), Color.White);
                    pos += 15;
                }
                pos = rect.Right - 100;
                healthToDraw -= 5;
            }

            if (player.DartCount <= 6)
            {
                pos = rect.Right - 100;
                for (int i = 0; i < player.DartCount; i++)
                {
                    spriteBatch.Draw(MonoGearGame.GetResource<Texture2D>("Sprites/SleepDart"), new Vector2(pos, rect.Bottom - 32), Color.White);
                    pos += 15;
                }
            }
            else
            {
                spriteBatch.DrawString(MonoGearGame.GetResource<SpriteFont>("Fonts/Arial"), "Darts: " + player.DartCount, new Vector2(rect.Right - 100, rect.Bottom - 32), Color.Red);
            }
            //spriteBatch.DrawString(MonoGearGame.GetResource<SpriteFont>("Fonts/Arial"), "Health: " + player.Health, new Vector2(rect.Right - 100, rect.Bottom - 50), Color.Red);
        }
    }
}
