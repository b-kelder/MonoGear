using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGear.Engine;
using MonoGear.Engine.Collisions;

namespace MonoGear.Entities
{
    /// <summary>
    /// Player character controlled by the player.
    /// </summary>
    public class Player : WorldEntityAnimated
    {
        /// <summary>
        /// Sprite of a dead player.
        /// </summary>
        private static Texture2D deadSprite;
        /// <summary>
        /// Movement speed
        /// </summary>
        public float Speed { get; set; }
        /// <summary>
        /// Delay between throwing rocks
        /// </summary>
        public int ThrowingDelay { get; set; }
        /// <summary>
        /// If we're in sneak mode
        /// </summary>
        public bool SneakMode { get; private set; }

        private float health;
        private bool wasDead;
        /// <summary>
        /// Player health
        /// </summary>
        public float Health
        {
            get { return health; }
            set
            {
                wasDead = health <= 0;
                health = value;
                if (health <= 0 && !wasDead)
                {
                    var gameOver = MonoGearGame.FindEntitiesWithTag("GameOverScreen")[0] as GameOver;
                    gameOver.EnableGameOver();
                }
            }
        }
        /// <summary>
        /// Amount of sleepdarts
        /// </summary>
        public int DartCount { get; set; }

        private DrivableVehicle currentVehicle;
        /// <summary>
        /// Vehicle the player is driving
        /// </summary>
        public DrivableVehicle CurrentVehicle
        {
            get
            {
                return currentVehicle;
            }
            set
            {
                walkingSound.Stop();
                currentVehicle = value;
            }
        }

        // Sounds
        private SoundEffectInstance walkingSound;
        private SoundEffectInstance walkingSoundGrass;
        private SoundEffectInstance walkingSoundWater;
        private SoundEffectInstance walkingSoundStone;

        public Player() : base()
        {
            // Speed in units/sec. Right now 1 unit = 1 pixel
            Speed = 100.0f;

            Z = 2;

            TextureAssetName = "Sprites/Person";

            AnimationLength = 3;
            AnimationCurrentFrame = 1;
            AnimationDelta = 0.1f;
            AnimationPingPong = true;

            Tag = "Player";

            LoadContent();

            Collider = new BoxCollider(this, new Vector2(8));
        }

        /// <summary>
        /// Loads content
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            if (deadSprite == null)
            {
                deadSprite = MonoGearGame.GetResource<Texture2D>("Sprites/Dead");
            }

            // Load sounds
            walkingSoundGrass = MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Running On Grass").CreateInstance();
            walkingSoundWater = MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Water_Drop_Sound").CreateInstance();
            walkingSoundStone = MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Concrete").CreateInstance();
            walkingSoundGrass.Volume = 1 * SettingsPage.Volume * SettingsPage.EffectVolume;
            walkingSoundWater.Volume = 1 * SettingsPage.Volume * SettingsPage.EffectVolume;
            walkingSoundStone.Volume = 1 * SettingsPage.Volume * SettingsPage.EffectVolume;

            // Default walking sound
            walkingSound = walkingSoundGrass;
        }

        /// <summary>
        /// Called when level is loaded
        /// </summary>
        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();

            // Find spawnpoint and place player on it
            var ents = MonoGearGame.FindEntitiesWithTag("PlayerSpawnPoint");
            if (ents.Count > 0)
            {
                Position = new Vector2(ents[0].Position.X, ents[0].Position.Y);
            }
        }

        /// <summary>
        /// Called once per frame
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="gameTime">gametime</param>
        public override void Update(Input input, GameTime gameTime)
        {
            // Animation done by parent class
            base.Update(input, gameTime);

            // Movement delta
            var dx = 0.0f;
            var dy = 0.0f;

            // Use analog sticks or keyboard for movement depending on if we have a gamepad connected
            if (input.PadConnected())
            {
                var sticks = input.GetGamepadState().ThumbSticks;

                dx += sticks.Left.X * Speed;
                dy += sticks.Left.Y * Speed;
            }
            else
            {
                if (input.IsButtonDown(Input.Button.Left))
                {
                    dx -= Speed;
                }
                if (input.IsButtonDown(Input.Button.Right))
                {
                    dx += Speed;
                }
                if (input.IsButtonDown(Input.Button.Up))
                {
                    dy -= Speed;
                }
                if (input.IsButtonDown(Input.Button.Down))
                {
                    dy += Speed;
                }
            }

            // Sneak mode
            if (input.IsButtonDown(Input.Button.Sneak))
            {
                SneakMode = true;
                Speed = 50;
            }
            else
            {
                SneakMode = false;
                Speed = 100;
            }

            // Clamp movement speed
            var delta = new Vector2(dx, dy);
            if (delta.LengthSquared() > Speed * Speed)
            {
                delta.Normalize();
                delta *= Speed;
            }

            // Get correct tile sound
            var tilevalue = MonoGearGame.GetCurrentLevel().GetTile(Position)?.Sound;
            SoundEffectInstance tilesound;
            switch (tilevalue)
            {
                case Tile.TileSound.Grass:
                    tilesound = walkingSoundGrass;
                    break;
                case Tile.TileSound.Water:
                    tilesound = walkingSoundWater;
                    break;
                case Tile.TileSound.Concrete:
                    tilesound = walkingSoundStone;
                    break;
                default:
                    tilesound = walkingSoundStone;
                    break;
            }

            if (tilesound != null && tilesound != walkingSound)
            {
                // stop old sound
                walkingSound.Stop();
                walkingSound = tilesound;
            }

            if (delta.LengthSquared() > 0)
            {
                // Moving
                Rotation = MathExtensions.VectorToAngle(delta);
                AnimationRunning = true;
                walkingSound.Play();
            }
            else
            {
                // Standing still
                SneakMode = true;
                AnimationRunning = false;
                AnimationCurrentFrame = 1;
                walkingSound.Stop();
            }

            // Sneaking is silent
            if (SneakMode)
            {
                walkingSound.Stop();
            }
                
            // Reduce delay per frame
            if (ThrowingDelay > 0)
            {
                ThrowingDelay -= 1;
            }
                

            // Throw rock
            if (input.IsButtonPressed(Input.Button.Throw))
            {
                if (ThrowingDelay <= 0)
                {
                    // Spawn rock and play sound
                    var rock = new Rock(MonoGearGame.FindEntitiesOfType<Player>()[0].Collider);
                    rock.Position = Position;
                    rock.Rotation = Rotation;
                    MonoGearGame.SpawnLevelEntity(rock);
                    ThrowingDelay = 45;
                    var sound = MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/StoneTrow_sound").CreateInstance();
                    sound.Volume = 1 * SettingsPage.Volume * SettingsPage.EffectVolume;
                    sound.Play();
                }
            }

            // Shoot sleep dart
            if (input.IsButtonPressed(Input.Button.Shoot))
            {
                if (DartCount > 0)
                {
                    // Spawn dart and play sound
                    var sleepDart = new SleepDart(MonoGearGame.FindEntitiesOfType<Player>()[0].Collider);
                    sleepDart.Position = Position;
                    sleepDart.Rotation = Rotation;
                    MonoGearGame.SpawnLevelEntity(sleepDart);
                    DartCount--;
                    var sound = MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Blowgun").CreateInstance();
                    sound.Volume = 1 * SettingsPage.Volume * SettingsPage.EffectVolume;
                    sound.Play();
                }
            }


            // Check collisions
            if (input.IsKeyDown(Keys.N))
            {
                // Noclip mode for debugging
                Position += delta * (float)gameTime.ElapsedGameTime.TotalSeconds * 10;
            }
            else
            {
                // Check collisions per axis
                var prevPos = Position;
                var deltaX = new Vector2(delta.X, 0);
                var deltaY = new Vector2(0, delta.Y);

                Position += deltaX * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (Collider.CollidesAny())
                {
                    // Reset if we hit anything
                    Position = prevPos;
                }
                prevPos = Position;
                Position += deltaY * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (Collider.CollidesAny())
                {
                    // Reset if we hit anything
                    Position = prevPos;
                }
            }

            // Camera tracks player
            Camera.main.Position = new Vector2(Position.X, Position.Y);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            //Draw deadSprite
            if (health <= 0 && !wasDead)
            {
                spriteBatch.Draw(deadSprite, new Vector2(Position.X, Position.Y + 1), deadSprite.Bounds, Color.White, 0, new Vector2(deadSprite.Bounds.Size.X, deadSprite.Bounds.Size.Y) / 2, 1, SpriteEffects.None, 0);
            }
        }

    }
}

