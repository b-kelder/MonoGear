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

        private List<Vector2> currentPath;
        private int currentPathIndex;

        public Guard()
        {
            // Speed in units/sec. Right now 1 unit = 1 pixel
            speed = 70.0f;
            TextureAssetName = "Sprites/guardsheet";

            AnimationLength = 3;
            AnimationCurrentFrame = 1;
            AnimationDelta = 0.05f;
            AnimationPingPong = true;
            AnimationRunning = false;

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


            if(currentPath != null && currentPathIndex >= 0)
            {
                AnimationRunning = true;
                if(currentPathIndex < currentPath.Count)
                {
                    var target = currentPath[currentPathIndex];
                    if (Vector2.DistanceSquared(Position, target) < 24)
                    {
                        currentPathIndex++;
                    }
                    else
                    {
                        Rotation = MathExtensions.VectorToAngle(target - Position);

                        var delta = MathExtensions.AngleToVector(Rotation) * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        Move(delta);
                    }
                }
                else
                {
                    currentPathIndex = -1;
                }
            }
            
            if(currentPathIndex < 0 || currentPath == null)
            {
                AnimationRunning = false;
                AnimationCurrentFrame = 1;
            }
        }

        public void Alert(Vector2 origin)
        {
            alerted = true;
            Pathfinding pathFinder = new Pathfinding();
            var path = pathFinder.FindPath(Position, origin);
            currentPath = path;
            currentPathIndex = 0;
        }

        public void MoveTo(Vector2 position)
        {
            this.Position = position;
        }
    }
}
