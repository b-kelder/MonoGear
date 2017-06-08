using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear.Engine
{
    public class Tile
    {
        public enum TileSound
        {
            Default,
            Grass,
            Concrete,
            Water
        }

        public int tilesetId;
        public Rectangle textureRect;
        public Texture2D Texture { get; set; }
        public TileSound Sound { get; set; }
        public bool Walkable { get; set; }

        float speedModifier;
        public float SpeedModifier
        {
            get { return speedModifier; }
            set
            {
                speedModifier = MathHelper.Clamp(value, 0.01f, 100.0f);
            }
        }

        public Tile(Texture2D texture)
        {
            Sound = TileSound.Default;
            Walkable = true;
            Texture = texture;
            SpeedModifier = 1.0f;
        }

        public void Draw(int x, int y, SpriteBatch spriteBatch)
        {
            var destRect = new Rectangle(x * textureRect.Width, y * textureRect.Height, textureRect.Width, textureRect.Height);
            spriteBatch.Draw(Texture, destRect, textureRect, Color.White);

            // Debug
            //var font = MonoGearGame.GetResource<SpriteFont>("Fonts/Arial");
            //spriteBatch.DrawString(font, tilesetId.ToString(), new Vector2(destRect.X, destRect.Y), Color.Red);
        }
    }
}
