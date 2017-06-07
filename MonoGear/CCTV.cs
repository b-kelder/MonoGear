using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    class CCTV : WorldEntity
    {
        public int sightRange { get; set; }
        public int sightFov { get; set; }

        private bool hacked;
        private Player player;

        public CCTV()
        {
            TextureAssetName = "Sprites/birdsheet";
            Tag = "CCTV";

            sightRange = 400;
            sightFov = 45;

            hacked = false;
        }

        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();
            player = MonoGearGame.FindEntitiesWithTag("Player")[0] as Player;
        }

        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);

            if (!hacked)
            {
                var pos = new Vector2();
                if (CanSee(out pos))
                {
                    var guards = MonoGearGame.FindEntitiesWithTag("Guard");
                    foreach (var g in guards)
                    {
                        var guard = g as Guard;
                        if (guard != null)
                        {
                            guard.Alert(pos);
                        }
                    }
                }
            }
        }

        public void Hack()
        {
            hacked = true;
        }

        private bool CanSee(out Vector2 entityPos)
        {
            var dis = Vector2.Distance(Position, player.Position);

            //Check if player is within view range
            if (dis < sightRange)
            {
                //Check to see if the guard is looking at the player
                var degrees = Math.Abs(MathHelper.ToDegrees(Rotation) - (90 + MathHelper.ToDegrees(MathExtensions.AngleBetween(Position, player.Position))));
                if (degrees <= (sightFov / 2) || degrees >= (360 - (sightFov / 2)))
                {
                    //Check to see if nothing blocks view of the player
                    Collider hit;
                    bool tilemap;
                    if (Collider.RaycastAny(Position, player.Position, out hit, out tilemap, Tag))
                    {
                        if (hit != null && hit.Entity.Tag.Equals("Player"))
                        {
                            entityPos = hit.Entity.Position;
                            return true;
                        }
                    }
                }
            }

            entityPos = Vector2.Zero;
            return false;
        }
    }
}
