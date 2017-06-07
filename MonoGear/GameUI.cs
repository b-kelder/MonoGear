using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

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

            for (int i = 0; i < player.Health; i++)
            {
                spriteBatch.Draw(ResourceManager.GetManager().GetResource<Texture2D>("Sprites/Heart"), new Vector2(pos, rect.Bottom - 50), Color.White);
                pos += 15;
            }

            if (player.DartCount <= 6)
            {
                pos = rect.Right - 100;
                for (int i = 0; i < player.DartCount; i++)
                {
                    spriteBatch.Draw(ResourceManager.GetManager().GetResource<Texture2D>("Sprites/SleepDart"), new Vector2(pos, rect.Bottom - 32), Color.White);
                    pos += 15;
                }
            }
            else
            {
                spriteBatch.DrawString(ResourceManager.GetManager().GetResource<SpriteFont>("Fonts/Arial"), "Darts: " + player.DartCount, new Vector2(rect.Right - 100, rect.Bottom - 32), Color.Red);
            }
            
            //spriteBatch.DrawString(ResourceManager.GetManager().GetResource<SpriteFont>("Fonts/Arial"), "Health: " + player.Health, new Vector2(rect.Right - 100, rect.Bottom - 50), Color.Red);
        }
    }
}
