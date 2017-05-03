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

        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);

            if(AnimationRunning)
            {
                if(animationCounter >= AnimationDelta)
                {
                    animationCounter -= AnimationDelta;
                    AnimationCurrentFrame = (AnimationCurrentFrame + 1) % AnimationLength;
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
            if(!Visible)
                return;

            Vector2 topLeft = new Vector2(Position.X - 0.5f * Size.X, Position.Y - 0.5f * Size.Y);
            Rectangle sourceRect = new Rectangle(AnimationCurrentFrame * (int)Size.X, 0, (int)Size.X, (int)Size.Y);
            spriteBatch.Draw(instanceTexture, topLeft, sourceRect, Color.White);
        }
    }
}
