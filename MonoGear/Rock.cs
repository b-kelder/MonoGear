using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonoGear
{
    class Rock : WorldEntity
    {
        float Speed { get; set; }
        bool triggered;

        public Rock()
        {
            CircleCollider collider = new CircleCollider(this, 2);
            collider.Trigger = true;

            // Speed in units/sec. Right now 1 unit = 1 pixel
            Random rand = new Random();
            Speed = 200f + rand.Next(-20, 20);
            TextureAssetName = "Sprites/Rock";

            Tag = "TheRock";

            LoadContent();

        }

        public override void Update(Input input, GameTime gameTime)
        {
            if (!Enabled)
                return;

            base.Update(input, gameTime);
            if (!triggered)
            {
                Collider collider;
                var delta = Forward * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Move(delta);
                if (Collider.CollidesAny(out collider))
                {
                    if (collider.Entity.Tag != "Player")
                    {
                        Move(-delta);
                        Speed = 0.0f;

                        foreach (var guard in MonoGearGame.FindEntitiesWithTag("Guard"))
                        {
                            if (Vector2.DistanceSquared(Position, guard.Position) < 100000)
                            {
                                var g = guard as Guard;
                                g.Alert(Position);
                            }
                        }

                        triggered = true;
                    }
                }

                if (Speed > 0)
                    Speed -= 3;
                else
                    Speed = 0;
            }
        }
    }
}
