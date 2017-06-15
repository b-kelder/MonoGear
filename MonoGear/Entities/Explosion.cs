using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGear.Engine.Audio;
using MonoGear.Engine;
using System.Collections.Generic;

namespace MonoGear.Entities
{
    /// <summary>
    /// Bird that flies towards the top of the screen.
    /// </summary>
    class Explosion : WorldEntityAnimated
    {
        private float blastRadius;
        private float maxDamage;
        private PositionalAudio sound;

        /// <summary>
        /// Constructor of the explosion class. Creates an instance of an explosion.
        /// </summary>
        /// 
        private Player player;
        public Explosion()
        {
            // Bird texture
            TextureAssetName = "Sprites/boem";

            AnimationLength = 16;
            AnimationCurrentFrame = 1;
            AnimationDelta = 0.05f;
            AnimationPingPong = false;
            AnimationRunning = true;

            Tag = "BOEM!";

            blastRadius = 50;
            maxDamage = 10;

            Z = 100;

            LoadContent();
        }

        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();
            player = MonoGearGame.FindEntitiesWithTag("Player")[0] as Player;
            sound = AudioManager.AddPositionalAudio(MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Explosion"), 1, 1200, Position, false);
            sound.Position = Position;
            sound.Volume = 0.8f;
        }

        /// <summary>
        /// Method that updates the game
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="gameTime">GameTime</param>
        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);
            

            // Check if the animation is at its last frame
            if (AnimationCurrentFrame == 1)
            {
                /*if(player.Enabled)
                {
                    
                    if(dis < blastRadius)
                    {
                        player.Health -= maxDamage * (dis / blastRadius * 100);
                    }
                }*/

                var things = MonoGearGame.FindEntitiesOfType<IDestroyable>();
                foreach (var thing in things)
                {
                    var dis = Vector2.Distance(thing.GetEntity().Position, Position);
                    if (dis < blastRadius)
                    {
                        thing.Damage(maxDamage * (dis / blastRadius * 100));
                    } 
                }
                
                
            }
            else if (AnimationCurrentFrame == 14)
            {
                MonoGearGame.DestroyEntity(this);
            }
        }
    }
}
