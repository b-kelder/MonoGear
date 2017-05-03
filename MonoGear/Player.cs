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
            if(input.IsKeyDown(Keys.A))
                dx -= Speed;
            if(input.IsKeyDown(Keys.D))
                dx += Speed;
            if(input.IsKeyDown(Keys.W))
                dy -= Speed;
            if(input.IsKeyDown(Keys.S))
                dy += Speed;

            var delta = new Vector3(dx, dy, 0);
            if(delta.LengthSquared() > Speed * Speed)
            {
                delta.Normalize();
                delta *= Speed;
            }

            AnimationRunning = delta.LengthSquared() > 0;

            Position += delta * (float)gameTime.ElapsedGameTime.TotalSeconds;


            Camera.main.Position = new Vector2(Position.X, Position.Y);
        }
    }
}
