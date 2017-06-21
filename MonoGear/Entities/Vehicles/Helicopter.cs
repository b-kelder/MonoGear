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

namespace MonoGear.Entities.Vehicles
{
    /// <summary>
    /// ApacheRoflCopter
    /// </summary>
    ///
    class Helicopter : WorldEntity, IDestroyable
    {
        private Texture2D props;
        private int rot = 0;
        private PositionalAudio heliSound;
        private float delay;
        private int barrelNr;
        private bool destroyed;
        private Texture2D destoyedSprite;
        private Player player;
        private float speed;

        public float Health { get; private set; }

        public Helicopter()
        {
            TextureAssetName = "Sprites/MyRoflcopter";

            Tag = "Helicopter";

            Z = 10;

            delay = 0;
            barrelNr = 0;
            speed = 240;
            Health = 50;

            LoadContent();
        }

        /// <summary>
        /// Method that executes when the level is loaded.
        /// </summary>
        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();

            player = MonoGearGame.FindEntitiesWithTag("Player")[0] as Player;

            if (props == null)
            {
                props = MonoGearGame.GetResource<Texture2D>("Sprites/Soisoisoisoisoisoisoisoisoisoisoisoisoisoisoisois");
            }
            heliSound = AudioManager.AddPositionalAudio(MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Helicopter Sound Effect"), 1, 300, Position, true);
            heliSound.Volume = 0.3f;
            destoyedSprite = MonoGearGame.GetResource<Texture2D>("Sprites/BrokenRoflcopter");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (!destroyed)
            {
                spriteBatch.Draw(props, Position + (Forward * 16), props.Bounds, Color.White, rot, new Vector2(props.Bounds.Size.X, props.Bounds.Size.Y) / 2, 1, SpriteEffects.None, 0);
                rot += 1;
            }
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

            var target = player.Position;
            Rotation = MathExtensions.VectorToAngle(target - Position);
            var distance = Vector2.Distance(Position, target);
            if (distance > 260)
            {
                // Move towards player
                var delta = MathExtensions.AngleToVector(Rotation) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                delta *= speed;
                Move(delta);
            }

            heliSound.Position = Position;

            if (delay > 0)
                delay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (delay <=0 && distance < 320)
            {
                var missile = new Missile(MonoGearGame.FindEntitiesOfType<Player>()[0].Collider);


                Vector2 vec = new Vector2(18, 0);
                if (barrelNr == 0)
                    vec.Y = 24;
                if (barrelNr == 1)
                    vec.Y = -24;
                if (barrelNr == 2)
                    vec.Y = 18;
                if (barrelNr == 3)
                    vec.Y = -18;

                missile.Position = Position + Forward * vec.X + Right * vec.Y;
                missile.Rotation = MathExtensions.VectorToAngle(player.Position - missile.Position);

                MonoGearGame.SpawnLevelEntity(missile);

                barrelNr++;
                if (barrelNr > 3)
                {
                    barrelNr = 0;
                }

                var sound = MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Helicopter_missile").CreateInstance();
                sound.Volume = 0.5f * SettingsPage.Volume * SettingsPage.EffectVolume;
                sound.Play();

                delay = 2f;
            }
        }

        public void Damage(float damage)
        {
            // Can only be destroyed by Destroy()
        }

        public void Destroy()
        {
            instanceTexture = destoyedSprite;

            destroyed = true;
            Enabled = false;

            AudioManager.StopPositional(heliSound);
        }
    }
}
