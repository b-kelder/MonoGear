using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGear.Engine;
using MonoGear.Engine.Audio;

namespace MonoGear.Entities
{
    /// <summary>
    /// Bird that flies towards the top of the screen.
    /// </summary>
    class Explosion : WorldEntityAnimated
    {
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

        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);

            if (AnimationCurrentFrame == 14)
            {
                Visible = false;
                Enabled = false;
            }
        }
    }
}
