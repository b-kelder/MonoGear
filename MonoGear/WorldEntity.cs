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
    public abstract class WorldEntity
    {
        protected Texture2D instanceTexture;

        public Vector2 Size { get; set; }

        /// <summary>
        /// World position. Keep in mind that the object is centered based on Size.
        /// </summary>
        public Vector2 Position { get; set; }
        public float Z { get; set; }

        public Vector2 Forward
        {
            get
            {
                return MathExtensions.AngleToVector(Rotation);
            }
        }

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

        public void Move(Vector2 delta)
        {
            Position += delta;
        }

        protected virtual void LoadContent()
        {
            instanceTexture = ResourceManager.GetManager().GetResource<Texture2D>(TextureAssetName);
            if(instanceTexture != null)
            {
                Size = new Vector2(instanceTexture.Bounds.Size.X, instanceTexture.Bounds.Size.Y);
            }
        }

        public virtual void OnLevelLoaded()
        {
            if(Collider != null)
                Collider.Register();

        }

        public virtual void OnLevelUnloaded()
        {
            if(Collider != null)
                Collider.Deregister();
        }

        public virtual void Update(Input input, GameTime gameTime)
        {
            if (!Enabled)
                return;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if(instanceTexture == null)
                return;

            spriteBatch.Draw(instanceTexture, new Vector2(Position.X, Position.Y), instanceTexture.Bounds, Color.White, Rotation, Size / 2, 1, SpriteEffects.None, 0);
        }
    }
}
