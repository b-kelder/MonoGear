using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGear.Engine.Collisions;
using MonoGear.Network;

namespace MonoGear.Engine
{
    public abstract class WorldEntity
    {
        protected const string NETWORK_NAME_POSITION = "_pos";
        protected const string NETWORK_NAME_ROTATION = "_rot";

        protected Texture2D instanceTexture;
        /// <summary>
        /// Size of the entity
        /// </summary>
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

        public Vector2 Right
        {
            get
            {
                return MathExtensions.AngleToVector(Rotation + MathHelper.PiOver2);
            }
        }

        /// <summary>
        /// Entity's rotation
        /// </summary>
        public float Rotation { get; set; }
        /// <summary>
        /// Entitiys collider
        /// </summary>
        public Collider Collider { get; set; }
        /// <summary>
        /// Indicates if the entity is visible or not
        /// </summary>
        public bool Visible { get; set; }
        /// <summary>
        /// Indicates if the entity is enabled or not
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// Entity's texture name
        /// </summary>
        public string TextureAssetName { get; set; }
        /// <summary>
        /// Entity's tag name
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Network representation of this WorldEntity
        /// </summary>
        public NetworkEntity NetworkEntity { get; protected set; }

        /// <summary>
        /// Constructor of the world entity class.
        /// </summary>
        public WorldEntity()
        {
            Visible = true;
            Enabled = true;
            Size = new Vector2(0, 0);
            NetworkEntity = new NetworkEntity(this);
            // Setup default network sync values
            NetworkEntity.SetSyncValue(NETWORK_NAME_POSITION, Position);
            NetworkEntity.SetSyncValue(NETWORK_NAME_ROTATION, Rotation);
        }

        /// <summary>
        /// Method that moves the entity.
        /// </summary>
        /// <param name="delta">Delta</param>
        public void Move(Vector2 delta)
        {
            Position += delta;
        }

        protected virtual void LoadContent()
        {
            instanceTexture = MonoGearGame.GetResource<Texture2D>(TextureAssetName);
            if(instanceTexture != null)
            {
                Size = new Vector2(instanceTexture.Bounds.Size.X, instanceTexture.Bounds.Size.Y);
            }
        }

        /// <summary>
        /// Method that executes when the level is loaded.
        /// </summary>
        public virtual void OnLevelLoaded()
        {
            if (Collider != null)
            {
                Collider.Register();
            }

        }

        /// <summary>
        /// Method that executes when the level is unloaded.
        /// </summary>
        public virtual void OnLevelUnloaded()
        {
            if (Collider != null)
            {
                Collider.Deregister();
            }
        }

        /// <summary>
        /// Method that updates the game.
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="gameTime">GameTime</param>
        public virtual void Update(Input input, GameTime gameTime)
        {
            if (!Enabled)
            {
                return;
            }

            // TODO: Maybe move this to some post update method? Idk
            if(NetManager.IsNetworkGame && NetworkEntity.Enabled)
            {
                // Handle basic network syncing
                if(NetworkEntity.IsAuthorative)
                {
                    // Position
                    NetworkEntity.SetSyncValue(NETWORK_NAME_POSITION, Position);

                    // Rotation
                    NetworkEntity.SetSyncValue(NETWORK_NAME_ROTATION, Rotation);
                }
                else
                {
                    // TODO: Use (and update) HasChanged?
                    // Position
                    Position = NetworkEntity.GetSyncVector2(NETWORK_NAME_POSITION);

                    // Rotation
                    Rotation = NetworkEntity.GetSyncFloat(NETWORK_NAME_ROTATION);
                }
            }
        }

        /// <summary>
        /// Method that draws the entity/
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (instanceTexture == null)
            {
                return;
            }

            spriteBatch.Draw(instanceTexture, new Vector2(Position.X, Position.Y), instanceTexture.Bounds, Color.White, Rotation, Size / 2, 1, SpriteEffects.None, 0);
        }
    }
}
