using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;

using MonoGear.Engine;
using MonoGear.Engine.Collisions;
using MonoGear.Engine.Audio;

namespace MonoGear.Entities
{
    class Missile : WorldEntity
    {
        float speed { get; set; }
        Collider originCollider;
        private float boemInSec;

        public Missile(Collider originCollider)
        {
            CircleCollider collider = new CircleCollider(this, 2);
            collider.Trigger = true;

            speed = 800f;
            boemInSec = 1f;
            TextureAssetName = "Sprites/Missile";
            Tag = "Missile";
            Z = 5;

            LoadContent();

            this.originCollider = originCollider;
        }

        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);
            
            var pos = Position;
            var delta = Forward * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Move(delta);

            if (boemInSec > 0)
                boemInSec -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Collider.CollidesAny() || boemInSec <= 0)
            {
                Position = pos;
                speed = 0.0f;

                MonoGearGame.SpawnLevelEntity(new Explosion() { Position = this.Position });
                MonoGearGame.DestroyEntity(this);
            }
        }
    }
}

