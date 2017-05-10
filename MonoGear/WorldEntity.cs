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
    public class WorldEntity
    {
        protected Texture2D instanceTexture;

        public Vector2 Size { get; set; }

        /// <summary>
        /// World position. Keep in mind that the object is centered based on Size.
        /// </summary>
        public Vector3 Position { get; set; }

        public float Rotation { get; set; }

        public Collider Collider { get; set; }

        public bool Visible { get; set; }
        public bool Enabled { get; set; }

        public string TextureAssetName { get; set; }
        public string Tag { get; set; }

        public WorldEntity()
        {
            Visible = true;
            Enabled = true;
            Size = new Vector2(0, 0);
        }

        protected virtual void LoadContent()
        {
            instanceTexture = ResourceManager.GetManager("Global").GetResource<Texture2D>(TextureAssetName);
            if(instanceTexture != null)
            {
                Size = new Vector2(instanceTexture.Bounds.Size.X, instanceTexture.Bounds.Size.Y);
            }
        }

        public virtual void OnLevelLoaded()
        {

        }

        public virtual void Update(Input input, GameTime gameTime)
        {
            if(!Enabled)
                return;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if(!Visible || instanceTexture == null)
                return;

            spriteBatch.Draw(instanceTexture, new Vector2(Position.X, Position.Y), instanceTexture.Bounds, Color.White, Rotation, Size / 2, 1, SpriteEffects.None, 0);
        }
    }
}
