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

        protected virtual void LoadContent()
        {
            instanceTexture = ResourceManager.GetManager("Global").GetResource<Texture2D>(TextureAssetName);
        }

        public virtual void Update(Input input, GameTime gameTime)
        {
            if(!Enabled)
                return;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if(!Visible)
                return;

            Vector2 topLeft = new Vector2(Position.X - 0.5f * Size.X, Position.Y - 0.5f * Size.Y);
            spriteBatch.Draw(instanceTexture, topLeft, Color.White);
        }
    }
}
