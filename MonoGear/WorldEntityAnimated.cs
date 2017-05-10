using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonoGear
{
    class WorldEntityAnimated : WorldEntity
    {
        private float animationCounter;

        public int AnimationLength { get; set; }
        public float AnimationDelta { get; set; }
        public int AnimationCurrentFrame { get; set; }
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

        public WorldEntityAnimated()
        {
            animDeltaSign = 1;
        }

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

        protected override void LoadContent()
        {
            base.LoadContent();

            Size = new Vector2(Size.X / AnimationLength, Size.Y);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(!Visible || instanceTexture == null)
                return;

            Rectangle sourceRect = new Rectangle(AnimationCurrentFrame * (int)Size.X, 0, (int)Size.X, (int)Size.Y);
            spriteBatch.Draw(instanceTexture, new Vector2(Position.X, Position.Y), sourceRect, Color.White, Rotation, Size / 2, 1, SpriteEffects.None, 0);
        }
    }
}
