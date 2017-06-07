using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    class CCTV : WorldEntity
    {
        private static Texture2D fovSprite;

        public int SightRange { get; set; }
        public int SightFOV { get; set; }

        private bool hacked;
        private Player player;

        public CCTV()
        {
            TextureAssetName = "Sprites/birdsheet";
            Tag = "CCTV";

            SightRange = 120;
            SightFOV = 90;

            hacked = false;

            LoadContent();
        }

        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();
            player = MonoGearGame.FindEntitiesWithTag("Player")[0] as Player;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            if(fovSprite == null)
            {
                fovSprite = ResourceManager.GetManager().GetResource<Texture2D>("Sprites/fov100");
            }
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Guard.DrawFOVDebug(spriteBatch, Position, Rotation, player.Position, SightRange, new Color(0, 100, 0, 10));
        }

        public void Hack()
        {
            hacked = true;
        }

        private bool CanSee(out Vector2 entityPos)
        {
            var dis = Vector2.Distance(Position, player.Position);

            //Check if player is within view range
            if (dis < SightRange)
            {
                //Check to see if the guard is looking at the player
                var degrees = Math.Abs(MathHelper.ToDegrees(Rotation) - (90 + MathHelper.ToDegrees(MathExtensions.AngleBetween(Position, player.Position))));
                if (degrees <= (SightFOV / 2) || degrees >= (360 - (SightFOV / 2)))
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
