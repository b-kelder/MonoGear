using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace MonoGear
{
    class Rock : WorldEntity
    {
        float Speed { get; set; }

        bool triggered;

        public Rock()
        {
            CircleCollider collider = new CircleCollider(this, 2);
            collider.Trigger = true;

            // Speed in units/sec. Right now 1 unit = 1 pixel
            Speed = 200f;
            TextureAssetName = "Sprites/Rock";

            Tag = "TheRock";

            LoadContent();

        }

        public override void Update(Input input, GameTime gameTime)
        {
            if (!Enabled)
                return;

            base.Update(input, gameTime);

            if(!triggered)
            {
                Collider collider;
                if(Collider.CollidesAny(out collider))
                {
                    if(collider.Entity.Tag != "Player")
                    {
                        Speed = 0.0f;

                        foreach(var guard in MonoGearGame.FindEntitiesWithTag("Guard"))
                        {
                            var g = guard as Guard;
                            g.Alert(Position);
                        }

                        triggered = true;
                        //TODO GUARDS ALERTED U FUCKED UP BRAH
                    }
                }

                Move(Forward * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds);

                if(Speed > 0)
                    Speed -= 3;
                else
                    Speed = 0;
            }
        }
    }
}
