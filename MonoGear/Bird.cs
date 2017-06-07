using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    /// <summary>
    /// Bird that flies towards the top of the screen.
    /// </summary>
    class Bird : WorldEntityAnimated
    {
        float speed;
        private AudioSource birdSound;

        public float YResetValue { get; set; }

        public Bird()
        {
            // Speed in units/sec. Right now 1 unit = 1 pixel
            speed = 100.0f;
            TextureAssetName = "Sprites/birdsheet";

            AnimationLength = 3;
            AnimationCurrentFrame = 1;
            AnimationDelta = 0.05f;
            AnimationPingPong = true;
            AnimationRunning = true;

            Tag = "ObeseHummingbird";

            Z = 100;

            LoadContent();

            birdSound = new AudioSource();
            birdSound.AddSoundEffect(ResourceManager.GetManager().GetResource<SoundEffect>("Audio/AudioFX/Car_sound"), 210);
            birdSound.Position = Position;
            AudioManager.AddAudioSource(birdSound);
            birdSound.Pause();    
        }

        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();
            birdSound.PlayAll();
        }

        public override void OnLevelUnloaded()
        {
            base.OnLevelUnloaded();
            birdSound.Pause();
        }



        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);

            if(Position.Y < -200)
            {
                Move(new Vector2(0, YResetValue));
            }

            Move(new Vector2(0, -speed * (float)gameTime.ElapsedGameTime.TotalSeconds));

            birdSound.Position = Position;
        }
    }
}
