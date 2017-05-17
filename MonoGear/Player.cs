using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace MonoGear
{
    class Player : WorldEntityAnimated
    {
        public float Speed { get; set; }
        private SoundEffectInstance walkingSound;

        private SoundEffectInstance walkingSoundNormal;
        private SoundEffectInstance walkingSoundWater;

        public Player() : base()
        {
            // Speed in units/sec. Right now 1 unit = 1 pixel
            Speed = 100.0f;
            TextureAssetName = "Sprites/guardsheet";

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

            walkingSoundNormal = ResourceManager.GetManager().GetResource<SoundEffect>("Audio/AudioFX/Running On Grass").CreateInstance();
            walkingSoundWater = ResourceManager.GetManager().GetResource<SoundEffect>("Audio/AudioFX/Water_Drop_Sound").CreateInstance();

            walkingSound = walkingSoundNormal;
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
                        tilesound = walkingSoundNormal;
                        break;
                    case 2:
                        tilesound = walkingSoundWater;
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
                AudioManager.GlobalAudioPlay(walkingSound, true);
            }
            else
            {
                AnimationRunning = false;
                AnimationCurrentFrame = 1;
                AudioManager.GlobalAudioStop(walkingSound);
            }


            // Raycast test
            if(input.IsKeyPressed(Keys.Space))
            {
                Collider hit;
                if(Collider.RaycastAny(Position, Position + (Forward * 32), out hit, "Player"))
                {
                    if(hit.Entity.Tag == "Fountain")
                    {
                        AudioManager.PlayOnce(ResourceManager.GetManager().GetResource<SoundEffect>("Audio/AudioFX/Guard_Alert_Sound"), 1);
                    }
                }
            }

            // Check collisions
            var prevPos = Position;
            var deltaX = new Vector2(delta.X, 0);
            var deltaY = new Vector2(0, delta.Y);

            Position += deltaX * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(Collider.CollidesAny())
            {
                Position = prevPos;
            }
            prevPos = Position;
            Position += deltaY * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(Collider.CollidesAny())
            {
                Position = prevPos;
            }

            Camera.main.Position = new Vector2(Position.X, Position.Y);
        }
    }
}

