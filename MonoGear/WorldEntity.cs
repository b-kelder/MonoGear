using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    abstract class WorldEntity
    {
        private Texture2D instanceTexture;

        public Vector2 Size { get; set; }

        public Vector3 Position { get; set; }

        public bool Visible { get; set; }
        public bool Enabled { get; set; }

        public string TextureAssetName { get; set; }

        public WorldEntity()
        {
            Visible = true;
            Enabled = true;
            Size = new Vector2(0, 0);
        }

        // Need something better than this. Maybe use some sort of sprite manager instead.
        public virtual void LoadContent(ContentManager content)
        {
            instanceTexture = content.Load<Texture2D>(TextureAssetName);
        }

        public virtual void Update(Input input, GameTime gameTime)
        {
            if(!Enabled)
                return;
        }

        public virtual void Draw(Viewport viewport, SpriteBatch spriteBatch)
        {
            if(!Visible)
                return;

            Vector2 topLeft = new Vector2(Position.X - 0.5f * Size.X, Position.Y - 0.5f * Size.Y);
            spriteBatch.Draw(instanceTexture, viewport.Translate(topLeft), Color.White);
        }
    }
}
