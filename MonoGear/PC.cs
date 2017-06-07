using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    class PC : WorldEntityAnimated
    {
        public List<CCTV> connectedCameras { get; set; }
        public float progressPerClick { get; set; }

        private Player player;
        private float hackingProgress;

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
            progressPerClick = 1;
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

        public void HackPC()
        {
            //Hack all connected camera's
            foreach (var CCTV in connectedCameras)
            {
                CCTV.Hack();
            }
        }

        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);

            if (input.IsButtonPressed(Input.Button.Interact) && Vector2.Distance(Position, player.Position) < 20)
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
