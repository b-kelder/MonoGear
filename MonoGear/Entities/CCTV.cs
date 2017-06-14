using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGear.Engine;
using MonoGear.Engine.Collisions;

namespace MonoGear.Entities
{
    class CCTV : WorldEntityAnimated
    {
        /// <summary>
        /// Sight range of the CCTV camera
        /// </summary>
        public int SightRange { get; set; }
        public int SightFOV { get; set; }

        private bool hacked;
        private Player player;

        /// <summary>
        /// Constructor of the car class. Creates an instance of a CCTV camera.
        /// </summary>
        public CCTV()
        {
            TextureAssetName = "Sprites/CameraOn";
            Tag = "CCTV";

            SightRange = 120;
            SightFOV = 90;

            hacked = false;

            AnimationLength = 2;
            AnimationCurrentFrame = 1;
            AnimationDelta = 0.50f;
            AnimationPingPong = true;
            AnimationRunning = true;

            LoadContent();
        }

        /// <summary>
        /// Method that executes when the level is loaded.
        /// </summary>
        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();
            player = MonoGearGame.FindEntitiesWithTag("Player")[0] as Player;
        }

        /// <summary>
        /// Method that updates the game
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="gameTime">GameTime</param>
        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);
            // Check if the camera is not hacked
            if (!hacked)
            {
                var pos = new Vector2();
                // Check if the CCTV camera can see the player
                if (CanSee(out pos))
                {
                    var guards = MonoGearGame.FindEntitiesWithTag("Guard");
                    // Alert all guards in the vicinity
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
            else
            {
                // Disable the CCTV camera if it's hacked
                Enabled = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if(!hacked)
            {
                Guard.DrawFOVDebug(spriteBatch, Position, Rotation, player.Position, SightRange, new Color(0, 100, 0, 10));
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
