using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

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
            if(!Enabled)
                return;

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
                

            var delta = new Vector2(dx, dy);
            if(delta.LengthSquared() > Speed * Speed)
            {
                delta.Normalize();
                delta *= Speed;
            }

            var tilemap = MonoGearGame.FindEntitiesWithTag("Tilemap");
            var col = tilemap[0].Collider as TilemapCollider;
            int tilevalue = col.GetTileValue(Position);
            if(tilevalue != -1)
            {
                SoundEffectInstance tilesound;
                switch(tilevalue)
                {
                    case 0:
                        tilesound = walkingSoundGrass;
                        break;
                    case 2:
                        tilesound = walkingSoundWater;
                        break;
                    case 3:
                        tilesound = walkingSoundStone;
                        break;
                    default:
                        tilesound = null;
                        break;
                }

                if(tilesound != null && tilesound != walkingSound)
                {
                    // stop old sound
                    AudioManager.GlobalAudioStop(walkingSound);
                    walkingSound = tilesound;
                }
             
            }

            if(delta.LengthSquared() > 0)
            {
                Rotation = MathExtensions.VectorToAngle(delta);
                AnimationRunning = true;
                AudioManager.GlobalAudioPlay(walkingSound);
            }
            else
            {
                AnimationRunning = false;
                AnimationCurrentFrame = 1;
                AudioManager.GlobalAudioStop(walkingSound);
            }

            if (sneakMode)
                AudioManager.GlobalAudioStop(walkingSound);

            if (ThrowingDelay > 0)
                ThrowingDelay -= 1;

            // Raycast test
            if(input.IsKeyPressed(Keys.Space))
            {
                if (ThrowingDelay <= 0)
                {
                    var dwayneThe = new Rock();
                    dwayneThe.Position = Position;
                    dwayneThe.Rotation = Rotation;
                    MonoGearGame.RegisterLevelEntity(dwayneThe);
                    ThrowingDelay = 45;
                    AudioManager.PlayOnce(ResourceManager.GetManager().GetResource<SoundEffect>("Audio/AudioFX/StoneTrow_sound"), 1);
                }
            }

            // Check collisions
            if(input.IsKeyDown(Keys.N))
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

