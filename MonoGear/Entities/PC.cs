using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using MonoGear.Engine;


namespace MonoGear.Entities
{
    class PC : WorldEntityAnimated
    {
        /// <summary>
        /// Property that contains the list of all connected cameras.
        /// </summary>
        public List<CCTV> ConnectedCameras { get; set; }
        /// <summary>
        /// Property with the progress per click.
        /// </summary>
        public float ProgressPerClick { get; set; }
        /// <summary>
        /// Property with the objective connected to this pc.
        /// </summary>
        public Objective Objective { get; set; }

        private Player player;
        private float hackingProgress;
        private bool inRange;
        private bool hacked;

        /// <summary>
        /// Constructor of the pc class. Creates an instance of a pc.
        /// </summary>
        public PC()
        {
            TextureAssetName = "Sprites/Pc";
            Tag = "PC";

            AnimationLength = 2;
            AnimationCurrentFrame = 1;
            AnimationDelta = 0.50f;
            AnimationPingPong = true;
            AnimationRunning = true;

            ConnectedCameras = new List<CCTV>();
            hackingProgress = 0;
            ProgressPerClick = 25;
            hacked = false;
            LoadContent();
        }

        /// <summary>
        /// Method that adds a CCTV camera to the connected cameras list.
        /// </summary>
        /// <param name="cctv"></param>
        public void AddCCTV(CCTV cctv)
        {
            ConnectedCameras.Add(cctv);
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
        /// Method that draws the pc.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (inRange && !hacked)
            {
                spriteBatch.DrawString(MonoGearGame.GetResource<SpriteFont>("Fonts/Arial"), "PRESS C TO HACK...", Position + new Vector2(-35, 16), Color.White);
                spriteBatch.DrawString(MonoGearGame.GetResource<SpriteFont>("Fonts/Arial"), "PROGRESS: " + hackingProgress.ToString() + "%", Position + new Vector2(-35, 24), Color.White);
            }
        }


        /// <summary>
        /// Method that hacks the pc
        /// </summary>
        public void HackPC()
        {
            //Hack all connected cameras
            foreach (var CCTV in ConnectedCameras)
            {
                CCTV.Hack();
            }
            hacked = true;
        }

        /// <summary>
        /// Method that updates the game
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="gameTime">GameTime</param>
        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);

            // Check if the player is in range
            if (Vector2.Distance(Position, player.Position) < 20)
            {
                inRange = true;
            }
            else
            {
                inRange = false;
            }
            // Check if the interact button is pressed
            if (input.IsButtonPressed(Input.Button.Interact) && inRange)
            {
                hackingProgress += ProgressPerClick;
                var sound = MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Hacking_sound").CreateInstance();
                sound.Volume = 1 * SettingsPage.Volume * SettingsPage.EffectVolume;
                sound.Play();
            }
            // Check if the hacking progress is completed
            if (hackingProgress >= 100)
            {
                // Hack the pc
                HackPC();
                Enabled = false;
                if (Objective != null)
                {
                    // Complete the objective
                    GameUI.CompleteObjective(Objective);
                }
            }
            
        }
    }
}
