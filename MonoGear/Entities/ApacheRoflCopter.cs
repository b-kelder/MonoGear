using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using MonoGear.Engine;
using MonoGear.Engine.Collisions;
using MonoGear.Engine.Audio;

namespace MonoGear.Entities
{
    /// <summary>
    /// ApacheRoflCopter
    /// </summary>
    ///
    class ApacheRoflCopter : DrivableVehicle
    {
        private Texture2D props;
        private int rot = 0;
        private PositionalAudio heliSound;
        private float delay;
        private int barrelNr;

        public ApacheRoflCopter()
        {
            TextureAssetName = "Sprites/MyRoflcopter";

            Tag = "I SEXUALY IDENTIFY AS AN APACHE HELICOPTER";

            Z = 100;

            delay = 0;
            barrelNr = 0;

            Speed = 400;
            entered = false;
            stationaryLock = false;

            Acceleration = 200;
            Braking = 220;
            Steering = 180;
            Drag = 50;

            Rotation = MathHelper.ToRadians(90);

            LoadContent();
        }

        /// <summary>
        /// Method that executes when the level is loaded.
        /// </summary>
        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();

            if (props == null)
            {
                props = MonoGearGame.GetResource<Texture2D>("Sprites/Soisoisoisoisoisoisoisoisoisoisoisoisoisoisoisois");
            }
            heliSound = AudioManager.AddPositionalAudio(MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Helicopter Sound Effect"), 1, 300, Position, true);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.Draw(props, Position + (Forward * 16), props.Bounds, Color.White, rot, new Vector2(props.Bounds.Size.X, props.Bounds.Size.Y) / 2, 1, SpriteEffects.None, 0);
            rot += 1;
        }

        /// <summary>
        /// Method that executes when the level is unloaded.
        /// </summary>
        public override void OnLevelUnloaded()
        {
            base.OnLevelUnloaded();
        }

        /// <summary>
        /// Method that updates the game
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="gameTime">GameTime</param>
        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);

            heliSound.Position = Position;

            if (delay > 0)
                delay -= 1;

            if (delay <=0 && input.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Q))
            {
                var missile = new Missile(MonoGearGame.FindEntitiesOfType<Player>()[0].Collider);
                missile.Rotation = Rotation;

                Vector2 vec = new Vector2(18, 0);
                if (barrelNr == 0)
                    vec.Y = 24;
                if (barrelNr == 1)
                    vec.Y = -24;
                if (barrelNr == 2)
                    vec.Y = 18;
                if (barrelNr == 3)
                    vec.Y = -18;

                missile.Position = Position + vec;

                MonoGearGame.SpawnLevelEntity(missile);

                barrelNr++;
                if (barrelNr > 3)
                {
                    barrelNr = 0;
                }

                var sound = MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Helicopter_missile").CreateInstance();
                sound.Volume = 1 * SettingsPage.Volume * SettingsPage.EffectVolume;
                sound.Play();

                delay = 10;
            }
        }
    }
}