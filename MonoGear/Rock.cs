using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonoGear
{
    class Rock : WorldEntity
    {
        float speed { get; set; }
        Collider originCollider;


        public Rock(Collider originCollider)
        {
            CircleCollider collider = new CircleCollider(this, 2);
            collider.Trigger = true;

            // Speed in units/sec. Right now 1 unit = 1 pixel
            Random rand = new Random();
            speed = 200f + rand.Next(-20, 20);
            TextureAssetName = "Sprites/Rock";
            Tag = "TheRock";
            LoadContent();

            this.originCollider = originCollider;
        }

        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);

            Collider collider;
            var pos = Position;
            var delta = Forward * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Move(delta);

            bool hitTilemap;

            if(Collider.CollidesAny(out collider, out hitTilemap, originCollider))
            {
                Position = pos;
                speed = 0.0f;

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

            if(speed > 0)
                speed -= 3;
            else
                speed = 0;
        }
    }
}
