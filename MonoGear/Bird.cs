using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    /// <summary>
    /// Bird that flies towards the top of the screen.
    /// </summary>
    class Bird : WorldEntityAnimated
    {
        float speed;

        public Bird()
        {
            // Speed in units/sec. Right now 1 unit = 1 pixel
            speed = 100.0f;
            TextureAssetName = "Sprites/birdsheet";

            AnimationLength = 3;
            AnimationCurrentFrame = 1;
            AnimationDelta = 0.05f;
            AnimationPingPong = true;
            AnimationRunning = true;

            Tag = "ObeseHummingbird";

            Z = 100;

            LoadContent();

            Collider = new BoxCollider(this, new Vector2(8));
        }

        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);
            if(!Enabled)
                return;

            if(Position.Y < -200)
            {
                Move(new Vector2(0, 1000));
            }

            Move(new Vector2(0, -speed * (float)gameTime.ElapsedGameTime.TotalSeconds));
        }
    }
}
