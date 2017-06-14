using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using MonoGear.Engine;
using MonoGear.Engine.Collisions;
using MonoGear.Engine.Audio;

namespace MonoGear.Entities
{
    /// <summary>
    /// Willys jeep, player controlled vehicle
    /// </summary>
    ///
    class Willys : WorldEntity
    {
        private PositionalAudio willysSound;
        public float Speed { get; set; }
        private bool on;
        private bool destroyed;
        private Player player;
        private Texture2D playerSprite;
        private Texture2D destroyedSprite;

        private float forwardSpeed;

        public float Acceleration { get; set; }
        public float Braking { get; set; }
        public float Steering { get; set; }
        bool stationaryLock;

        public Willys()
        {
            TextureAssetName = "Sprites/Willys";
            Tag = "Willys jeep";
            Speed = 230;
            on = false;
            destroyed = false;
            stationaryLock = false;

            Z = 1;

            Acceleration = 80;
            Braking = 200;
            Steering = 180;

            Collider = new BoxCollider(this, new Vector2(24,24));

            LoadContent();
        }

        /// <summary>
        /// Method that executes when the level is loaded.
        /// </summary>
        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();
            player = MonoGearGame.FindEntitiesWithTag("Player")[0] as Player;
            willysSound = AudioManager.AddPositionalAudio(MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Car_sound"), 0, 300, Position, true);
            playerSprite = MonoGearGame.GetResource<Texture2D>("Sprites/WillysPlayer");
            destroyedSprite = MonoGearGame.GetResource<Texture2D>("Sprites/BrokenWillys");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (on)
            {
                spriteBatch.Draw(playerSprite, Position, playerSprite.Bounds, Color.White, Rotation, new Vector2(playerSprite.Bounds.Size.X, playerSprite.Bounds.Size.Y) / 2, 1, SpriteEffects.None, 0);
            }
            if(destroyed)
            {
                spriteBatch.Draw(destroyedSprite, Position, destroyedSprite.Bounds, Color.White, Rotation, new Vector2(destroyedSprite.Bounds.Size.X, destroyedSprite.Bounds.Size.Y) / 2, 1, SpriteEffects.None, 0);
            }
        }

        public void Enter()
        {
            on = true;
            player.Visible = false;
            player.Enabled = false;
        }

        public void Exit()
        {
            stationaryLock = false;
            on = false;
            player.Visible = true;
            player.Enabled = true;
            player.Position = Position + Right * -30;
        }

        /// <summary>
        /// Method that updates the game
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="gameTime">GameTime</param>
        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);

            if (on)
            {
                willysSound.Volume = 1;
                if (input.IsButtonPressed(Input.Button.Interact))
                {
                    Exit();
                }
                else
                {

                    if(!stationaryLock)
                    {
                        if(forwardSpeed != 0)
                        {
                            if(input.IsButtonDown(Input.Button.Left))
                            {
                                Rotation -= MathHelper.ToRadians(Steering) * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)Math.Sqrt(forwardSpeed / Speed);
                            }
                            if(input.IsButtonDown(Input.Button.Right))
                            {
                                Rotation += MathHelper.ToRadians(Steering) * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)Math.Sqrt(forwardSpeed / Speed);
                            }
                        }

                        if(forwardSpeed > 0)
                        {
                            var sd = 0.0f;
                            if(input.IsButtonDown(Input.Button.Up))
                            {
                                sd += Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }
                            if(input.IsButtonDown(Input.Button.Down))
                            {
                                sd -= Braking * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }


                            if(forwardSpeed + sd < 0)
                            {
                                stationaryLock = true;
                                forwardSpeed = 0;
                            }
                            else
                            {
                                forwardSpeed += sd;
                            }

                            // Clamp speed
                            if(forwardSpeed > Speed)
                            {
                                forwardSpeed = Speed;
                            }
                        }
                        else if(forwardSpeed < 0)
                        {
                            // Reversing
                            var sd = 0.0f;
                            if(input.IsButtonDown(Input.Button.Up))
                            {
                                sd += Braking * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }
                            if(input.IsButtonDown(Input.Button.Down))
                            {
                                sd -= Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }

                            if(forwardSpeed + sd > 0)
                            {
                                stationaryLock = true;
                                forwardSpeed = 0;
                            }
                            else
                            {
                                forwardSpeed += sd;
                            }

                            // Clamp speed
                            if(forwardSpeed < -Speed)
                            {
                                forwardSpeed = -Speed;
                            }
                        }
                        else
                        {
                            // Standing still
                            var sd = 0.0f;
                            if(input.IsButtonDown(Input.Button.Up))
                            {
                                sd += Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }
                            if(input.IsButtonDown(Input.Button.Down))
                            {
                                sd -= Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }

                            forwardSpeed += sd;
                        }
                    }
                    else
                    {
                        if(input.IsButtonReleased(Input.Button.Up) || input.IsButtonReleased(Input.Button.Down))
                        {
                            stationaryLock = false;
                        }
                    }

                    var delta = Forward * forwardSpeed;
                    if (delta.LengthSquared() > Speed * Speed)
                    {
                        delta.Normalize();
                        delta *= Speed;
                    }


                    var prevPos = Position;
                    var deltaX = new Vector2(delta.X, 0);
                    var deltaY = new Vector2(0, delta.Y);

                    Position += deltaX * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    Collider hitCollider;
                    bool hitTilemap;

                    if (Collider.CollidesAny(out hitCollider, out hitTilemap, player.Collider))
                    {
                        Position = prevPos;
                        forwardSpeed = 0;
                    }
                    prevPos = Position;
                    Position += deltaY * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Collider.CollidesAny(out hitCollider, out hitTilemap, player.Collider))
                    {
                        Position = prevPos;
                        forwardSpeed = 0;
                    }

                    player.Position = Position;
                    Camera.main.Position = Position;
                }
            }
            else
            {
                willysSound.Volume = 0;
                if (Vector2.Distance(player.Position, Position) < 40)
                {
                    if (input.IsButtonPressed(Input.Button.Interact))
                    {
                        Enter();
                    }
                }
            }

            willysSound.Position = Position;
        }
    }
}