using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGear.Engine.Audio;
using MonoGear.Engine;

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
        private bool exploded;

        /// <summary>
        /// Constructor of the explosion class. Creates an instance of an explosion.
        /// </summary>
        /// 
        private Player player;

        /// <summary>
        /// Constructor of the explosion class. Creates an instance of an explosion.
        /// </summary>
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

            blastRadius = 60;
            maxDamage = 30;

            Z = 100;

            LoadContent();
        }

        /// <summary>
        /// Method that executes when the level is loaded.
        /// </summary>
        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();
            // Find the player
            player = MonoGearGame.FindEntitiesWithTag("Player")[0] as Player;

            //Play the explotion sound on creation
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
            

            // Check if the animation is at its first frame
            if (AnimationCurrentFrame == 1 && !exploded)
            {
                if(player.Enabled)
                {
                    // Calcutate damage based on distance from the explotion
                    var dis = Vector2.Distance(player.Position, Position);
                    if(dis < blastRadius)
                    {
                        player.Health -= maxDamage * (dis / blastRadius);
                    }
                }

                // Find everything that can be destroyed
                var things = MonoGearGame.FindEntitiesOfType<IDestroyable>();
                foreach (var thing in things)
                {
                    // Calcutate damage based on distance from the explotion
                    var dis = Vector2.Distance((thing as WorldEntity).Position, Position);
                    if (dis < blastRadius)
                    {
                        thing.Damage(maxDamage * (dis / blastRadius));
                    } 
                }

                // Note that we have exploded
                exploded = true;
            }
            // Destoy the explotion after its done with the animation
            else if (AnimationCurrentFrame == 14)
            {
                MonoGearGame.DestroyEntity(this);
            }
        }
    }
}
