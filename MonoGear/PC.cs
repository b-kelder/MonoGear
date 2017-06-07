using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    class PC : WorldEntity
    {
        public List<CCTV> connectedCameras { get; set; }
        public float progressPerClick { get; set; }

        private float hackingProgress;

        public PC()
        {
            TextureAssetName = "Sprites/birdsheet";
            Tag = "PC";

            connectedCameras = new List<CCTV>();
            hackingProgress = 0;
            progressPerClick = 1;
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

            if (input.IsButtonPressed(Input.Button.Interact))
            {
                hackingProgress += progressPerClick;
            }

            if (hackingProgress >= 100)
            {
                HackPC();
            }
        }
    }
}
