using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonoGear.Engine
{
    public abstract class WorldEntityAnimated : WorldEntity
    {
        private float animationCounter;
        /// <summary>
        /// The length of the animation
        /// </summary>
        public int AnimationLength { get; set; }
        /// <summary>
        /// The delta of teh animation
        /// </summary>
        public float AnimationDelta { get; set; }
        /// <summary>
        /// The animation's current frame
        /// </summary>
        public int AnimationCurrentFrame { get; set; }
        /// <summary>
        /// Property that indicates wether or not the animation is running.
        /// </summary>
        public bool AnimationRunning { get; set; }
        public bool AnimationPingPong
        {
            get { return pingpong; }
            set
            {
                pingpong = value;
                animDeltaSign = 1;
            }
        }
        private int animDeltaSign;
        private bool pingpong;

        /// <summary>
        /// Constructor of the world entity animated class.
        /// </summary>
        public WorldEntityAnimated()
        {
            animDeltaSign = 1;
        }

        /// <summary>
        /// Method that updates the game.
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="gameTime">GameTime</param>
        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);

            if(AnimationRunning)
            {
                if(animationCounter >= AnimationDelta)
                {
                    animationCounter -= AnimationDelta;
                    AnimationCurrentFrame = (AnimationCurrentFrame + animDeltaSign);
                    if(AnimationCurrentFrame > AnimationLength - 1)
                    {
                        if(AnimationPingPong)
                        {
                            AnimationCurrentFrame = Math.Max(0, AnimationCurrentFrame - 1);
                            animDeltaSign *= -1;
                        }
                        else
                        {
                            AnimationCurrentFrame = 0;
                        }
                    }
                    else if(AnimationCurrentFrame < 0)
                    {
                        if(AnimationPingPong)
                        {
                            AnimationCurrentFrame = Math.Min(AnimationLength - 1, AnimationCurrentFrame + 1);
                            animDeltaSign *= -1;
                        }
                        else
                        {
                            AnimationCurrentFrame = AnimationLength - 1;
                        }
                    }
                }
                animationCounter += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        /// <summary>
        /// Method that executes when the level is loaded.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            Size = new Vector2(Size.X / AnimationLength, Size.Y);
        }

        /// <summary>
        /// Method that draws the entity.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (instanceTexture == null)
            {
                return;
            }

            Rectangle sourceRect = new Rectangle(AnimationCurrentFrame * (int)Size.X, 0, (int)Size.X, (int)Size.Y);
            spriteBatch.Draw(instanceTexture, new Vector2(Position.X, Position.Y), sourceRect, Color.White, Rotation, Size / 2, 1, SpriteEffects.None, 0);
        }
    }
}
