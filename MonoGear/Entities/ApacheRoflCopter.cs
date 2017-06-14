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
    class ApacheRoflCopter : WorldEntity
    {
        private Texture2D props;
        private int rot = 0;
        private PositionalAudio heliSound;

        public ApacheRoflCopter()
        {
            TextureAssetName = "Sprites/MyRoflcopter";

            Tag = "I SEXUALY IDENTIFY AS AN APACHE HELICOPTER";

            Z = 100;

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

            if (input.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Q))
            {
                var sleepDart = new Missile(MonoGearGame.FindEntitiesOfType<Player>()[0].Collider);
                sleepDart.Position = Position;
                sleepDart.Rotation = Rotation;
                MonoGearGame.SpawnLevelEntity(sleepDart);
                var sound = MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Helicopter_missile").CreateInstance();
                sound.Volume = 1 * SettingsPage.Volume * SettingsPage.EffectVolume;
                sound.Play();
            }
        }
    }
}