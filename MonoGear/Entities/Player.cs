using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Windows.ApplicationModel.Core;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using MonoGear.Engine;
using MonoGear.Engine.Collisions;
using MonoGear.Engine.Audio;

namespace MonoGear.Entities
{
    public class Player : WorldEntityAnimated
    {
        private static Texture2D deadSprite;
        public float Speed { get; set; }
        public int ThrowingDelay { get; set; }
        public bool SneakMode { get; set; }

        private float health;
        private bool wasDead;
        public float Health {
            get { return health; }
            set
            {
                wasDead = health <= 0;
                health = value;
                if(health <= 0 && !wasDead)
                {
                    var gameOver = MonoGearGame.FindEntitiesWithTag("GameOverScreen")[0] as GameOver;
                    gameOver.EnableGameOver();
                }
            }
        }

        public int DartCount { get; set; }

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

        protected override void LoadContent()
        {
            base.LoadContent();

            if (deadSprite == null)
            {
                deadSprite = MonoGearGame.GetResource<Texture2D>("Sprites/Dead");                
            }

            walkingSoundGrass = MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Running On Grass").CreateInstance();
            walkingSoundWater = MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Water_Drop_Sound").CreateInstance();
            walkingSoundStone = MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Concrete").CreateInstance();
            walkingSoundGrass.Volume = 1 * SettingsPage.Volume * SettingsPage.EffectVolume;
            walkingSoundWater.Volume = 1 * SettingsPage.Volume * SettingsPage.EffectVolume;
            walkingSoundStone.Volume = 1 * SettingsPage.Volume * SettingsPage.EffectVolume;

            walkingSound = walkingSoundGrass;
        }

        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();

            // Give health and items
            Health = 5.0f;
            DartCount = 100;

            var ents = MonoGearGame.FindEntitiesWithTag("PlayerSpawnPoint");
            if(ents.Count > 0)
            {
                Position = new Vector2(ents[0].Position.X, ents[0].Position.Y);
            }
        }

        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);

            var dx = 0.0f;
            var dy = 0.0f;
            if(input.IsButtonDown(Input.Button.Left))
                dx -= Speed;
            if(input.IsButtonDown(Input.Button.Right))
                dx += Speed;
            if(input.IsButtonDown(Input.Button.Up))
                dy -= Speed;
            if(input.IsButtonDown(Input.Button.Down))
                dy += Speed;

            if (input.IsKeyDown(Keys.LeftShift))
            {
                SneakMode = true;
                Speed = 50;
            }
            else
            {
                SneakMode = false;
                Speed = 100;
            }

            if (input.IsKeyDown(Keys.G))
            {
                var gameOver = MonoGearGame.FindEntitiesWithTag("GameOverScreen")[0] as GameOver;
                gameOver.EnableGameOver();
            }

            var delta = new Vector2(dx, dy);
            if(delta.LengthSquared() > Speed * Speed)
            {
                delta.Normalize();
                delta *= Speed;
            }

            var tilevalue = MonoGearGame.GetCurrentLevel().GetTile(Position)?.Sound;
            SoundEffectInstance tilesound;
            switch(tilevalue)
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

            if(tilesound != null && tilesound != walkingSound)
            {
                // stop old sound
                walkingSound.Stop();
                walkingSound = tilesound;
            }
            if(delta.LengthSquared() > 0)
            {
                Rotation = MathExtensions.VectorToAngle(delta);
                AnimationRunning = true;
                walkingSound.Play();
            }
            else
            {
                SneakMode = true;
                AnimationRunning = false;
                AnimationCurrentFrame = 1;
                walkingSound.Stop();
            }

            if (SneakMode)
                walkingSound.Play();

            if (ThrowingDelay > 0)
                ThrowingDelay -= 1;

            // Throw rock
            if(input.IsButtonPressed(Input.Button.Throw))
            {
                if (ThrowingDelay <= 0)
                {
                    var dwayneThe = new Rock(MonoGearGame.FindEntitiesOfType<Player>()[0].Collider);
                    dwayneThe.Position = Position;
                    dwayneThe.Rotation = Rotation;
                    MonoGearGame.SpawnLevelEntity(dwayneThe);
                    ThrowingDelay = 45;
                    var sound = MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/StoneTrow_sound").CreateInstance();
                    sound.Volume = 1 * SettingsPage.Volume * SettingsPage.EffectVolume;
                    sound.Play();
                }
            }

            // Shoot sleep dart
            if(input.IsButtonPressed(Input.Button.Shoot))
            {
                if(DartCount > 0)
                {
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
                Position += delta * (float)gameTime.ElapsedGameTime.TotalSeconds *  10;

            }
            else
            {
                var prevPos = Position;
                var deltaX = new Vector2(delta.X, 0);
                var deltaY = new Vector2(0, delta.Y);

                Position += deltaX * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (Collider.CollidesAny())
                {
                    Position = prevPos;
                }
                prevPos = Position;
                Position += deltaY * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (Collider.CollidesAny())
                {
                    Position = prevPos;
                }
            }
            

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

