using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    class WorldEntity
    {
        static Texture2D texture;

        public Rectangle Size { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public bool Visible { get; set; }
        public bool Enabled { get; set; }

        public string TextureFile { get; set; }

        public WorldEntity()
        {
            Visible = true;
            Enabled = true;
        }

        public void LoadGraphics(GraphicsDevice graphicsDevice)
        {
            if(texture == null)
            {
                using(var stream = TitleContainer.OpenStream("Content/charactersheet.png"))
                {
                    texture = Texture2D.FromStream(graphicsDevice, stream);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if(!Enabled)
                return;

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(!Visible)
                return;

            Vector2 topLeft = new Vector2(X, Y);
            spriteBatch.Draw(texture, topLeft, Color.White);
        }
    }
}
