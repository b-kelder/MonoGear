using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGear
{
    class Player : WorldEntityAnimated
    {
        public float Speed { get; set; }

        public Player() : base()
        {
            // Speed in units/sec. Right now 1 unit = 1 pixel
            Speed = 100.0f;
            TextureAssetName = "Sprites/guardsheet";

            AnimationLength = 3;
            AnimationDelta = 0.2f;

            LoadContent();
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

            var delta = new Vector3(dx, dy, 0);
            if(delta.LengthSquared() > Speed * Speed)
            {
                delta.Normalize();
                delta *= Speed;
            }

            if(delta.LengthSquared() > 0)
            {
                Rotation = (float)(Math.Atan2(delta.Y, delta.X) - Math.PI * 0.5);
                AnimationRunning = true;
            }
            else
            {
                AnimationRunning = false;
            }

            Position += delta * (float)gameTime.ElapsedGameTime.TotalSeconds;

            Camera.main.Position = new Vector2(Position.X, Position.Y);
        }
    }
}
