using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear.Engine
{
    /// <summary>
    /// Stores tile data
    /// </summary>
    public class Tile
    {
        /// <summary>
        /// Sound type enum
        /// </summary>
        public enum TileSound
        {
            Default,
            Grass,
            Concrete,
            Water
        }

        /// <summary>
        /// Id of tile in Tiled tileset
        /// </summary>
        public int tilesetId;
        /// <summary>
        /// Rect indicating the tile on the tilemap texture
        /// </summary>
        public Rectangle textureRect;
        /// <summary>
        /// Tilemap texture
        /// </summary>
        public Texture2D Texture { get; set; }
        /// <summary>
        /// Tile sound type
        /// </summary>
        public TileSound Sound { get; set; }
        /// <summary>
        /// If tile is walkable
        /// </summary>
        public bool Walkable { get; set; }

        float speedModifier;
        /// <summary>
        /// Speed mofigier
        /// </summary>
        public float SpeedModifier
        {
            get { return speedModifier; }
            set
            {
                speedModifier = MathHelper.Clamp(value, 0.01f, 100.0f);
            }
        }

        /// <summary>
        /// Creates a new tile with the given tilemap texture
        /// </summary>
        /// <param name="texture">Texture</param>
        public Tile(Texture2D texture)
        {
            Sound = TileSound.Default;
            Walkable = true;
            Texture = texture;
            SpeedModifier = 1.0f;
        }

        /// <summary>
        /// Draws the tile at the given tile world position
        /// </summary>
        /// <param name="x">Tile x</param>
        /// <param name="y">Tile y</param>
        /// <param name="spriteBatch">Spritebatch to use for rendering</param>
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
