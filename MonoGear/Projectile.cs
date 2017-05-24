using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonoGear
{
    class Projectile : WorldEntity
    {
        float Speed { get; set; }
        Collider playerCol;
        private string textureAssetName;

        public Projectile(string textureAssetName)
        {
            CircleCollider collider = new CircleCollider(this, 2);
            collider.Trigger = true;

            // Speed in units/sec. Right now 1 unit = 1 pixel
            Random rand = new Random();
            Speed = 200f + rand.Next(-20, 20);
            TextureAssetName = textureAssetName;

            Tag = "TheRock";

            LoadContent();

            playerCol = MonoGearGame.FindEntitiesOfType<Player>()[0].Collider;
        }

        public override void Update(Input input, GameTime gameTime)
        {
            if (!Enabled)
                return;

            base.Update(input, gameTime);
            Collider collider;
            var pos = Position;
            var delta = Forward * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Move(delta);

            if(Collider.CollidesAny(out collider, playerCol))
            {
                Position = pos;
                Speed = 0.0f;

                var entities = MonoGearGame.FindEntitiesOfType<Guard>();

                foreach(var guard in entities)
                {
                    if(Vector2.DistanceSquared(Position, guard.Position) < 100000000)
                    {
                        guard.Alert(Position);
                    }
                }

                Enabled = false;
            }

            if(Speed > 0)
                Speed -= 3;
            else
                Speed = 0;
        }
    }
}
