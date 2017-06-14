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
    class Willys : WorldEntity
    {
        private PositionalAudio willysSound;
        public float Speed { get; set; }
        private bool on;
        private Player player;
        private Texture2D playerSprite;

        public Willys()
        {
            TextureAssetName = "Sprites/Willys";
            Tag = "Willys jeep";
            Speed = 230;
            on = false;

            Z = 1;

            Collider = new BoxCollider(this, new Vector2(32,48));

            LoadContent();
        }

        /// <summary>
        /// Method that executes when the level is loaded.
        /// </summary>
        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();
            player = MonoGearGame.FindEntitiesWithTag("Player")[0] as Player;
            willysSound = AudioManager.AddPositionalAudio(MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Helicopter Sound Effect"), 1, 300, Position, true);
            playerSprite = MonoGearGame.GetResource<Texture2D>("Sprites/WillysPlayer");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (on)
            {
                spriteBatch.Draw(playerSprite, Position, playerSprite.Bounds, Color.White, Rotation, new Vector2(playerSprite.Bounds.Size.X, playerSprite.Bounds.Size.Y) / 2, 1, SpriteEffects.None, 0);
            }
        }

        /// <summary>
        /// Method that updates the game
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="gameTime">GameTime</param>
        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);

            if (on)
            {
                if (input.IsButtonPressed(Input.Button.Interact))
                {
                    on = false;
                    player.Visible = true;
                    player.Position = Position + new Vector2(0, -30);
                }
                else
                {
                    var dx = 0.0f;
                    var dy = 0.0f;
                    if (input.IsButtonDown(Input.Button.Left))
                        dx -= Speed;
                    if (input.IsButtonDown(Input.Button.Right))
                        dx += Speed;
                    if (input.IsButtonDown(Input.Button.Up))
                        dy -= Speed;
                    if (input.IsButtonDown(Input.Button.Down))
                        dy += Speed;

                    var delta = new Vector2(dx, dy);
                    if (delta.LengthSquared() > Speed * Speed)
                    {
                        delta.Normalize();
                        delta *= Speed;
                    }

                    if (delta.LengthSquared() > 0)
                    {
                        Rotation = MathExtensions.VectorToAngle(delta);
                    }


                    var prevPos = Position;
                    var deltaX = new Vector2(delta.X, 0);
                    var deltaY = new Vector2(0, delta.Y);

                    Position += deltaX * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    Collider ignoreMe;
                    bool meTooThx;

                    if (Collider.CollidesAny(out ignoreMe, out meTooThx, player.Collider))
                    {
                        Position = prevPos;
                    }
                    prevPos = Position;
                    Position += deltaY * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Collider.CollidesAny(out ignoreMe, out meTooThx, player.Collider))
                    {
                        Position = prevPos;
                    }

                    player.Position = Position;
                }
            }
            else
            {
                if (Vector2.Distance(player.Position, Position) < 40)
                {
                    if (input.IsButtonPressed(Input.Button.Interact))
                    {
                        on = true;
                        player.Visible = false;
                    }
                }
            }

            willysSound.Position = Position;
        }
    }
}