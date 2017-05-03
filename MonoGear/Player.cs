using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGear
{
    class Player : WorldEntity
    {
        public float Speed { get; set; }

        public Player() : base()
        {
            // Speed in units/sec. Right now 1 unit = 1 pixel
            Speed = 600.0f;
            TextureAssetName = "Sprites/s_generator";

            LoadContent();
        }

        public override void Update(Input input, GameTime gameTime)
        {
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

            Position += delta * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
