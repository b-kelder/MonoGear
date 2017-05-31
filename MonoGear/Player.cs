using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Windows.ApplicationModel.Core;
using System.Diagnostics;

namespace MonoGear
{
    class Player : WorldEntityAnimated
    {
        public float Speed { get; set; }
        public int ThrowingDelay { get; set; }
        public bool sneakMode { get; set; }

        private SoundEffectInstance walkingSound;

        private SoundEffectInstance walkingSoundGrass;
        private SoundEffectInstance walkingSoundWater;
        private SoundEffectInstance walkingSoundStone;

        public Player() : base()
        {
            // Speed in units/sec. Right now 1 unit = 1 pixel
            Speed = 100.0f;

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

            walkingSoundGrass = ResourceManager.GetManager().GetResource<SoundEffect>("Audio/AudioFX/Running On Grass").CreateInstance();
            walkingSoundWater = ResourceManager.GetManager().GetResource<SoundEffect>("Audio/AudioFX/Water_Drop_Sound").CreateInstance();
            walkingSoundStone = ResourceManager.GetManager().GetResource<SoundEffect>("Audio/AudioFX/Concrete").CreateInstance();

            walkingSound = walkingSoundGrass;
        }

        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();

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
            if(input.IsKeyDown(Keys.A) || input.IsKeyDown(Keys.Left))
                dx -= Speed;
            if(input.IsKeyDown(Keys.D) || input.IsKeyDown(Keys.Right))
                dx += Speed;
            if(input.IsKeyDown(Keys.W) || input.IsKeyDown(Keys.Up))
                dy -= Speed;
            if(input.IsKeyDown(Keys.S) || input.IsKeyDown(Keys.Down))
                dy += Speed;

            if (input.IsKeyDown(Keys.LeftShift))
            {
                sneakMode = true;
                Speed = 50;
            }
            else
            {
                sneakMode = false;
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
                AudioManager.GlobalAudioStop(walkingSound);
                walkingSound = tilesound;
            }
            if(delta.LengthSquared() > 0)
            {
                Rotation = MathExtensions.VectorToAngle(delta);
                AnimationRunning = true;
                AudioManager.GlobalAudioPlay(walkingSound);
            }
            else
            {
                sneakMode = true;
                AnimationRunning = false;
                AnimationCurrentFrame = 1;
                AudioManager.GlobalAudioStop(walkingSound);
            }

            if (sneakMode)
                AudioManager.GlobalAudioStop(walkingSound);

            if (ThrowingDelay > 0)
                ThrowingDelay -= 1;

            // Throw rock
            if(input.IsKeyPressed(Keys.Space))
            {
                if (ThrowingDelay <= 0)
                {
                    var dwayneThe = new Projectile("Sprites/Rock", "TheRock", MonoGearGame.FindEntitiesOfType<Player>()[0].Collider);
                    dwayneThe.Position = Position;
                    dwayneThe.Rotation = Rotation;
                    MonoGearGame.RegisterLevelEntity(dwayneThe);
                    ThrowingDelay = 45;
                    AudioManager.PlayOnce(ResourceManager.GetManager().GetResource<SoundEffect>("Audio/AudioFX/StoneTrow_sound"), 1);
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
    }
}

