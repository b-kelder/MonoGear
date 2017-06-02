﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    class SleepDart : WorldEntity
    {
        float speed { get; set; }
        Collider originCollider;

        public SleepDart(Collider originCollider)
        {
            CircleCollider collider = new CircleCollider(this, 2);
            collider.Trigger = true;

            // Speed in units/sec. Right now 1 unit = 1 pixel
            Random rand = new Random();
            speed = 250f + rand.Next(-20, 20);
            TextureAssetName = "Sprites/Rock";
            Tag = "SleepDart";
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

            if (Collider.CollidesAny(out collider, originCollider))
            {
                Position = pos;
                speed = 0.0f;

                if (collider.Entity.Tag.Equals("Guard"))
                {
                    collider.Entity.Enabled = false;
                }

                Enabled = false;
            }

            //if (speed > 0)
            //    speed -= 3;
            //else
            //    speed = 0;
        }
    }
}
