using MonoGear.Engine;
using System;
using Microsoft.Xna.Framework;
using MonoGear.Engine.Collisions;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGear.Entities
{
    public class DrivableVehicle : WorldEntity, IDestroyable
    {
        protected bool entered;
        protected Player player;

        protected float forwardSpeed;

        /// <summary>
        /// Property for the speed of the vehicle.
        /// </summary>
        public float Speed { get; set; }
        /// <summary>
        /// Property for the acceleration of the vehicle.
        /// </summary>
        public float Acceleration { get; set; }
        /// <summary>
        /// Property for the braking of the vehicle.
        /// </summary>
        public float Braking { get; set; }
        /// <summary>
        /// Property for the steering of the vehicle.
        /// </summary>
        public float Steering { get; set; }
        /// <summary>
        /// Property for the drag of the vehicle.
        /// </summary>
        public float Drag { get; set; }
        protected bool stationaryLock;
        /// <summary>
        /// Property that indicates wether or not the vehicle has constant steering.
        /// </summary>
        public bool ConstantSteering { get; set; }
        /// <summary>
        /// Property that indicates the objective.
        /// </summary>
        public Objective objective { get; set; }
        public float Health { get; protected set; }
        protected Texture2D destroyedSprite;
        protected bool destroyed;

        /// <summary>
        /// Method that executes when the level is loaded.
        /// </summary>
        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();
            player = MonoGearGame.FindEntitiesWithTag("Player")[0] as Player;
            destroyed = false;
        }

        /// <summary>
        /// Method that executes when the player enters the vehicle.
        /// </summary>
        public void Enter()
        {
            entered = true;
            // Make the player invisible
            player.CurrentVehicle = this;
            player.Visible = false;
            // Disable the player
            player.Enabled = false;
            if (objective != null)
            {
                GameUI.CompleteObjective(objective);
            }
        }

        /// <summary>
        /// Method that executes when the player exits the vehicle.
        /// </summary>
        public void Exit()
        {
            stationaryLock = false;
            entered = false;
            forwardSpeed = 0;
            // Make the player visible
            player.CurrentVehicle = null;
            player.Visible = true;
            // Re-enable the player
            player.Enabled = true;
            player.Position = Position + Right * -30;
        }

        /// <summary>
        /// Method that updates the game.
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="gameTime">GameTime</param>
        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);
            // Check if the player has entered the vehicle
            if (entered)
            {
                // Check if the interact button is pressed
                if (input.IsButtonPressed(Input.Button.Interact))
                {
                    // Exit the vehicle
                    Exit();
                }
                else
                {
                    if (!stationaryLock)
                    {
                        if (ConstantSteering)
                        {
                            if (input.IsButtonDown(Input.Button.Left))
                            {
                                var sticks = input.GetGamepadState().ThumbSticks;

                                Rotation += sticks.Left.X * MathHelper.ToRadians(Steering) * (float)gameTime.ElapsedGameTime.TotalSeconds * MathExtensions.Sign(forwardSpeed);
                            }
                            if (input.IsButtonDown(Input.Button.Right))
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
                        }
                        else
                        {
                            // Speed based steering for consistent turning circle
                            if (forwardSpeed != 0)
                            {
                                // Rotate if we're not standing still
                                if (input.IsButtonDown(Input.Button.Left))
                                {
                                    var sticks = input.GetGamepadState().ThumbSticks;

                                    Rotation += sticks.Left.X * MathHelper.ToRadians(Steering) * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)Math.Sqrt(Math.Abs(forwardSpeed) / Speed) * Math.Sign(forwardSpeed);
                                }
                                else
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
                        }

                        // Check if the vehicle's forward speed is greater than 0
                        if (forwardSpeed > 0)
                        {
                            // Moving forward
                            var sd = 0.0f;
                            if (input.IsButtonDown(Input.Button.Up))
                            {
                                sd += Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }
                            if (input.IsButtonDown(Input.Button.Down))
                            {
                                sd -= Braking * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }

                            // Add drag if no input is present
                            if (sd == 0)
                            {
                                sd -= Drag * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }

                            // Check if we should stop
                            if (forwardSpeed + sd < 0)
                            {
                                stationaryLock = true;
                                forwardSpeed = 0;
                            }
                            else
                            {
                                forwardSpeed += sd;
                            }

                            // Clamp speed
                            if (forwardSpeed > Speed)
                            {
                                forwardSpeed = Speed;
                            }
                        }
                        else if (forwardSpeed < 0)
                        {
                            // Reversing
                            var sd = 0.0f;
                            if (input.IsButtonDown(Input.Button.Up))
                            {
                                sd += Braking * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }
                            if (input.IsButtonDown(Input.Button.Down))
                            {
                                sd -= Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }

                            // Add drag if no input is present
                            if (sd == 0)
                            {
                                sd += Drag * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }

                            // Check if we should stop
                            if (forwardSpeed + sd > 0)
                            {
                                stationaryLock = true;
                                forwardSpeed = 0;
                            }
                            else
                            {
                                forwardSpeed += sd;
                            }

                            // Clamp speed
                            if (forwardSpeed < -Speed)
                            {
                                forwardSpeed = -Speed;
                            }
                        }
                        else
                        {
                            // Standing still
                            var sd = 0.0f;
                            if (input.IsButtonDown(Input.Button.Up))
                            {
                                sd += Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }
                            if (input.IsButtonDown(Input.Button.Down))
                            {
                                sd -= Acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }

                            forwardSpeed += sd;
                        }
                    }
                    else
                    {
                        if (input.IsButtonUp(Input.Button.Up) && input.IsButtonUp(Input.Button.Down))
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
                    // Check if the collider is not null
                    if (Collider != null)
                    {
                        Collider hitCollider;
                        bool hitTilemap;

                        Position += deltaX * (float)gameTime.ElapsedGameTime.TotalSeconds;
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
                if (player.Enabled && Vector2.Distance(player.Position, Position) < 40)
                {
                    // Check if the interact button is pressed
                    if (input.IsButtonPressed(Input.Button.Interact))
                    {
                        // Enter the vehicle
                        Enter();
                    }
                }
            }
        }

        public void Damage(float damage)
        {
            Health -= damage;

            if (Health <= 0)
            {
                Destroy();
            }
        }

        public void Destroy()
        {
            if (entered)
            {
                Exit();
            }

            instanceTexture = destroyedSprite;
            Enabled = false;
        }
    }
}
