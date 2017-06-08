using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGear
{
    class PC : WorldEntityAnimated
    {
        public List<CCTV> connectedCameras { get; set; }
        public float progressPerClick { get; set; }

        private Player player;
        private float hackingProgress;
        private bool inRange;
        private bool hacked;

        public PC()
        {
            TextureAssetName = "Sprites/Pc";
            Tag = "PC";

            AnimationLength = 2;
            AnimationCurrentFrame = 1;
            AnimationDelta = 0.50f;
            AnimationPingPong = true;
            AnimationRunning = true;

            connectedCameras = new List<CCTV>();
            hackingProgress = 0;
            progressPerClick = 25;
            hacked = false;
            LoadContent();
        }

        public void AddCCTV(CCTV cctv)
        {
            connectedCameras.Add(cctv);
        }

	    public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();
            player = MonoGearGame.FindEntitiesWithTag("Player")[0] as Player;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (inRange && !hacked)
            {
                spriteBatch.DrawString(ResourceManager.GetManager().GetResource<SpriteFont>("Fonts/Arial"), "PRESS C TO HACK...", Position + new Vector2(-35, 16), Color.White);
                spriteBatch.DrawString(ResourceManager.GetManager().GetResource<SpriteFont>("Fonts/Arial"), "PROGRESS: " + hackingProgress.ToString() + "%", Position + new Vector2(-35, 24), Color.White);
            }
        }

        public void HackPC()
        {
            //Hack all connected camera's
            foreach (var CCTV in connectedCameras)
            {
                CCTV.Hack();
            }
            hacked = true;
        }

        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);

            if (Vector2.Distance(Position, player.Position) < 20)
                inRange = true;
            else
                inRange = false;

            if (input.IsButtonPressed(Input.Button.Interact) && inRange)
            {
                hackingProgress += progressPerClick;
            }
            if (hackingProgress >= 100)
            {
                HackPC();
                Enabled = false;
            }
        }
    }
}
