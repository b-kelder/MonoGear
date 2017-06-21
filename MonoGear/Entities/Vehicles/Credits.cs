using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGear.Engine;
using MonoGear.Engine.Audio;
using Microsoft.Xna.Framework.Media;

namespace MonoGear.Entities
{
    /// <summary>
    /// Bird that flies towards the top of the screen.
    /// </summary>
    class Credits : WorldEntity
    {
        Texture2D madeby, wouter, manuel, bram, danny, tom, thomas;
        Texture2D specialThanks;
        Texture2D music, dejaVu, america, kevin;


        /// <summary>
        /// Constructor of the bird class. Creates an instance of a bird.
        /// </summary>
        public Credits()
        {
            // Bird texture
            TextureAssetName = "Sprites/blank";

            Tag = "Credits";

            Z = Int32.MaxValue;

            LoadContent();
        }

        /// <summary>
        /// Method that executes when the level is loaded.
        /// </summary>
        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();

            if (MediaPlayer.State != MediaState.Playing)
            {
                MediaPlayer.Play(MonoGearGame.GetResource<Song>("Audio/Music/America Horse With No Name"));
                MediaPlayer.Volume = 1 * SettingsPage.Volume * SettingsPage.MusicVolume;
            }

            wouter = MonoGearGame.GetResource<Texture2D>("Sprites/Credits/Wouter");
            manuel = MonoGearGame.GetResource<Texture2D>("Sprites/Credits/Manuel");
            bram = MonoGearGame.GetResource<Texture2D>("Sprites/Credits/Bram");
            danny = MonoGearGame.GetResource<Texture2D>("Sprites/Credits/Danny");
            tom = MonoGearGame.GetResource<Texture2D>("Sprites/Credits/Tom");
            thomas = MonoGearGame.GetResource<Texture2D>("Sprites/Credits/Thomas");
            madeby = MonoGearGame.GetResource<Texture2D>("Sprites/Credits/MadeBy");
            specialThanks = MonoGearGame.GetResource<Texture2D>("Sprites/Credits/SpecialThanks");
            music = MonoGearGame.GetResource<Texture2D>("Sprites/Credits/Music");
            dejaVu = MonoGearGame.GetResource<Texture2D>("Sprites/Credits/DejaVu");
            america = MonoGearGame.GetResource<Texture2D>("Sprites/Credits/America");
            kevin = MonoGearGame.GetResource<Texture2D>("Sprites/Credits/Kevin");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.Draw(madeby, new Vector2(4000, 920), Color.White);
            spriteBatch.Draw(wouter, new Vector2(5950, 722), Color.White);
            spriteBatch.Draw(manuel, new Vector2(8000, 920), Color.White);
            spriteBatch.Draw(bram, new Vector2(10000, 722), Color.White);
            spriteBatch.Draw(thomas, new Vector2(12000, 930), Color.White);
            spriteBatch.Draw(tom, new Vector2(14150, 722), Color.White);
            spriteBatch.Draw(danny, new Vector2(16000, 920), Color.White);

            spriteBatch.Draw(music, new Vector2(20000, 722), Color.White);
            spriteBatch.Draw(dejaVu, new Vector2(22000, 920), Color.White);
            spriteBatch.Draw(america, new Vector2(24000, 722), Color.White);
            spriteBatch.Draw(kevin, new Vector2(26000, 920), Color.White);
        }
    }
}

