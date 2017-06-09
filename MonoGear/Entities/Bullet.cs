using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using System;
using MonoGear.Engine;
using MonoGear.Engine.Collisions;

namespace MonoGear.Entities
{
    class Bullet : WorldEntity
    {
        float speed { get; set; }
        Collider originCollider;


        public Bullet(Collider originCollider)
        {
            CircleCollider collider = new CircleCollider(this, 2);
            collider.Trigger = true;

            // Speed in units/sec. Right now 1 unit = 1 pixel
            Random rand = new Random();
            speed = 350f;
            TextureAssetName = "Sprites/Bullet";
            Tag = ".45 ACP";
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

                if(!hitTilemap && collider.Entity.Tag == "Player")
                {
                    var player = collider.Entity as Player;
                    player.Health -= 1.0f;
                }

                Enabled = false;
            }

            if(speed > 0)
                speed -= 1;
            else
                speed = 0;
        }
    }
}
