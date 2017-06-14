using Microsoft.Xna.Framework.Graphics;
using MonoGear.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGear.Engine.Collisions;

namespace MonoGear.Entities
{
    public class DrivableVehicle : WorldEntity
    {
        protected bool entered;
        protected Player player;

        protected float forwardSpeed;

        public float Speed { get; set; }
        public float Acceleration { get; set; }
        public float Braking { get; set; }
        public float Steering { get; set; }
        public float Drag { get; set; }
        protected bool stationaryLock;
        public bool ConstantSteering { get; set; }

        /// <summary>
        /// Method that executes when the level is loaded.
        /// </summary>
        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();
            player = MonoGearGame.FindEntitiesWithTag("Player")[0] as Player;
        }

        public void Enter()
        {
            entered = true;
            player.Visible = false;
            player.Enabled = false;
        }

        public void Exit()
        {
            stationaryLock = false;
            entered = false;
            forwardSpeed = 0;
            player.Visible = true;
            player.Enabled = true;
            player.Position = Position + Right * -30;
        }

        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);

            if(entered)
            {
                if(input.IsButtonPressed(Input.Button.Interact))
                {
                    Exit();
                }
                else
                {
                    if(!stationaryLock)
                    {
                        if(ConstantSteering)
                        {
                            if(input.IsButtonDown(Input.Button.Left))
                            {
                                Rotation -= MathHelper.ToRadians(Steering) * (float)gameTime.ElapsedGameTime.TotalSeconds * MathExtensions.Sign(forwardSpeed);
                            }
                            if(input.IsButtonDown(Input.Button.Right))
                            {
                                Rotation += MathHelper.ToRadians(Steering) * (float)gameTime.ElapsedGameTime.TotalSeconds * MathExtensions.Sign(forwardSpeed);
                            }
                        }
                        else
                        {
                            // Speed based steering for consistent turning circle
                            if(forwardSpeed != 0)
                            {
                                // Rotate if we're not standing still
                                if(input.IsButtonDown(Input.Button.Left))
                                {
                                    Rotation -= MathHelper.ToRadians(Steering) * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)Math.Sqrt(Math.Abs(forwardSpeed) / Speed) * Math.Sign(forwardSpeed);
                                }
                                if(input.IsButtonDown(Input.Button.Right))
                                {
                                    Rotation += MathHelper.ToRadians(Steering) * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)Math.Sqrt(Math.Abs(forwardSpeed) / Speed) * Math.Sign(forwardSpeed);
                                }
                            }
                        }


                        if(forwardSpeed > 0)
                        {
                            // Moving forward
                            var sd = 0.0f;
                            if(input.IsButtonDown(Input.Button.Up))
                            {
                                sd += Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }
                            if(input.IsButtonDown(Input.Button.Down))
                            {
                                sd -= Braking * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }

                            // Add drag if no input is present
                            if(sd == 0)
                            {
                                sd -= Drag * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }

                            // Check if we should stop
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

                            // Add drag if no input is present
                            if(sd == 0)
                            {
                                sd += Drag * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }

                            // Check if we should stop
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
                        if(input.IsButtonUp(Input.Button.Up) && input.IsButtonUp(Input.Button.Down))
                        {
                            stationaryLock = false;
                        }
                    }

                    var delta = Forward * forwardSpeed;
                    if(delta.LengthSquared() > Speed * Speed)
                    {
                        delta.Normalize();
                        delta *= Speed;
                    }


                    var prevPos = Position;
                    var deltaX = new Vector2(delta.X, 0);
                    var deltaY = new Vector2(0, delta.Y);

                    

                    

                    if(Collider != null)
                    {
                        Collider hitCollider;
                        bool hitTilemap;

                        Position += deltaX * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if(Collider.CollidesAny(out hitCollider, out hitTilemap, player.Collider))
                        {
                            Position = prevPos;
                            forwardSpeed = 0;
                        }
                        prevPos = Position;
                        Position += deltaY * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if(Collider.CollidesAny(out hitCollider, out hitTilemap, player.Collider))
                        {
                            Position = prevPos;
                            forwardSpeed = 0;
                        }
                    }
                    else
                    {
                        Position += delta * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    

                    player.Position = Position;
                    Camera.main.Position = Position;
                }
            }
            else
            {
                if(player.Enabled && Vector2.Distance(player.Position, Position) < 40)
                {
                    if(input.IsButtonPressed(Input.Button.Interact))
                    {
                        Enter();
                    }
                }
            }
        }
    }
}
