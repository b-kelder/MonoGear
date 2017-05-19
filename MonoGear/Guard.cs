using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    class Guard : WorldEntityAnimated
    {
        float speed;
        public bool alerted { get; set; }

        public Guard()
        {
            // Speed in units/sec. Right now 1 unit = 1 pixel
            speed = 100.0f;
            TextureAssetName = "Sprites/Guard";

            AnimationLength = 3;
            AnimationCurrentFrame = 1;
            AnimationDelta = 0.05f;
            AnimationPingPong = true;
            AnimationRunning = true;

            Tag = "Guard";

            Z = 100;

            LoadContent();

            Collider = new BoxCollider(this, new Vector2(8));
        }

        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);
            if (!Enabled)
                return;
        }

        public void Alert(Vector2 origin)
        {
            alerted = true;
            PathFinding path = new PathFinding();

            foreach (var pos in path.FindPath(Position, origin))
            {
                MoveTo(pos);
            }
        }

        public void MoveTo(Vector2 position)
        {
            this.Position = position;
        }
    }
}
