using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGear.Engine;
using MonoGear.Engine.Audio;

namespace MonoGear.Entities.Vehicles
{
    /// <summary>
    /// Helicopter
    /// </summary>
    class Helicopter : WorldEntity, IDestroyable
    {
        private Texture2D props;
        private int rot = 0;
        private PositionalAudio heliSound;
        private float delay;
        private int barrelNr;
        private bool destroyed;
        private Texture2D destoyedSprite;
        private Player player;

        /// <summary>
        /// Property with the helicopter's health.
        /// </summary>
        public float Health { get; private set; }

        /// <summary>
        /// Constructor of the helicopter class. Creates an instance of a helicopter.
        /// </summary>
        public Helicopter()
        {
            TextureAssetName = "Sprites/MyRoflcopter";

            Tag = "Helicopter";

            Z = 10;

            delay = 0;
            barrelNr = 0;

            Health = 50;

            LoadContent();
        }

        /// <summary>
        /// Method that executes when the level is loaded.
        /// </summary>
        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();

            player = MonoGearGame.FindEntitiesWithTag("Player")[0] as Player;

            if (props == null)
            {
                props = MonoGearGame.GetResource<Texture2D>("Sprites/Soisoisoisoisoisoisoisoisoisoisoisoisoisoisoisois");
            }
            heliSound = AudioManager.AddPositionalAudio(MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Helicopter Sound Effect"), 1, 300, Position, true);
            heliSound.Volume = 0.3f;
            destoyedSprite = MonoGearGame.GetResource<Texture2D>("Sprites/BrokenRoflcopter");
        }

        /// <summary>
        /// Method that draws the helicopter
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            // Check if the helicopter isn't destroyed
            if (!destroyed)
            {
                spriteBatch.Draw(props, Position + (Forward * 16), props.Bounds, Color.White, rot, new Vector2(props.Bounds.Size.X, props.Bounds.Size.Y) / 2, 1, SpriteEffects.None, 0);
                rot += 1;
            }
        }

        /// <summary>
        /// Method that executes when the level is unloaded.
        /// </summary>
        public override void OnLevelUnloaded()
        {
            base.OnLevelUnloaded();
        }

        /// <summary>
        /// Method that updates the game
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="gameTime">GameTime</param>
        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);

            Position = player.Position - new Vector2(300, 0);

            heliSound.Position = Position;
            // Check if the delay is greater than 0
            if (delay > 0)
            {
                delay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // Check if the delay is smaller than 0
            if (delay <=0)
            {
                var missile = new Missile(MonoGearGame.FindEntitiesOfType<Player>()[0].Collider);
                missile.Rotation = Rotation;

                Vector2 vec = new Vector2(18, 0);
                if (barrelNr == 0)
                {
                    vec.Y = 24;
                }
                if (barrelNr == 1)
                {
                    vec.Y = -24;
                }
                if (barrelNr == 2)
                {
                    vec.Y = 18;
                }
                if (barrelNr == 3)
                {
                    vec.Y = -18;
                }

                missile.Position = Position + Forward * vec.X + Right * vec.Y;

                MonoGearGame.SpawnLevelEntity(missile);

                barrelNr++;
                if (barrelNr > 3)
                {
                    barrelNr = 0;
                }

                var sound = MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Helicopter_missile").CreateInstance();
                sound.Volume = 0.5f * SettingsPage.Volume * SettingsPage.EffectVolume;
                sound.Play();

                delay = 1;
            }
        }

        /// <summary>
        /// Method is executed when the helicopter is damaged.
        /// </summary>
        /// <param name="damage">The amount of damage taken</param>
        public void Damage(float damage)
        {
            Health -= damage;
            // Check if health is 0 or smaller
            if (Health <= 0)
            {
                // Destroy the helicopter
                Destroy();
            }
        }

        /// <summary>
        /// Method that destroys the helicopter.
        /// </summary>
        public void Destroy()
        {
            instanceTexture = destoyedSprite;

            destroyed = true;
            Enabled = false;
            // Stop the jet sound
            AudioManager.StopPositional(heliSound);
        }
    }
}
