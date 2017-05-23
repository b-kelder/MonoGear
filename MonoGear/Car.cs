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
    /// Car
    /// </summary>
    class Car : WorldEntity
    {
        float speed;
        private AudioSource carSound;

        public Car()
        {
            // Speed in units/sec. Right now 1 unit = 1 pixel
            speed = 200.0f;
            TextureAssetName = "Sprites/Car";

            Tag = "Car";

            Z = 100;

            LoadContent();

            Collider = new BoxCollider(this, new Vector2(8));

            carSound = new AudioSource();
            carSound.AddSoundEffect(ResourceManager.GetManager().GetResource<SoundEffect>("Audio/AudioFX/Car_sound"), 700);
            carSound.Position = Position;
            AudioManager.AddAudioSource(carSound);
            carSound.Pause();

        }

        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();
            carSound.PlayAll();
        }

        public override void OnLevelUnloaded()
        {
            base.OnLevelUnloaded();
            carSound.Pause();
        }



        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);
            if (!Enabled)
                return;

            if (Position.Y < -200)
            {
                Move(new Vector2(0, 5000));
            }

            Move(new Vector2(0, -speed * (float)gameTime.ElapsedGameTime.TotalSeconds));

            carSound.Position = Position;
        }
    }
}

