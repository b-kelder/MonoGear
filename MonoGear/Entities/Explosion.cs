using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGear.Engine;

namespace MonoGear.Entities
{
    /// <summary>
    /// Bird that flies towards the top of the screen.
    /// </summary>
    class Explosion : WorldEntityAnimated
    {
        /// <summary>
        /// Constructor of the explosion class. Creates an instance of an explosion.
        /// </summary>
        public Explosion()
        {
            // Bird texture
            TextureAssetName = "Sprites/boem";

            AnimationLength = 16;
            AnimationCurrentFrame = 1;
            AnimationDelta = 0.05f;
            AnimationPingPong = false;
            AnimationRunning = true;

            Tag = "BOEM!";

            Z = 100;
            var sound = MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Explosion").CreateInstance();
            sound.Volume = 1 * SettingsPage.Volume * SettingsPage.EffectVolume;
            sound.Play();

            LoadContent();
        }

        /// <summary>
        /// Method that updates the game
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="gameTime">GameTime</param>
        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);

            // Check if the animation is at its last frame
            if (AnimationCurrentFrame == 14)
            {
                MonoGearGame.DestroyEntity(this);
            }
        }
    }
}
