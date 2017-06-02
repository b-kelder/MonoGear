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
            spriteBatch.DrawString(ResourceManager.GetManager().GetResource<SpriteFont>("Fonts/Arial"), "Health: " + player.Health, new Vector2(rect.Right - 100, rect.Bottom - 50), Color.Red);
            spriteBatch.DrawString(ResourceManager.GetManager().GetResource<SpriteFont>("Fonts/Arial"), "Darts: " + player.DartCount, new Vector2(rect.Right - 100, rect.Bottom - 32), Color.Red);
        }
    }
}
