using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    enum Direction
    {
        North,
        East,
        South,
        West
    }
    /// <summary>
    /// Car
    /// </summary>
    class Car : WorldEntity
    {
        float speed;
        private AudioSource carSound;
        private Direction direction;
        private Dictionary<Vector2, Direction> turns;
        private bool follow;

        public Car(Direction direction, Dictionary<Vector2, Direction> turns)
        {
            this.direction = direction;
            this.turns = turns;
            follow = true;

            Construct();
        }

        public Car(Direction direction)
        {
            this.direction = direction;
            follow = false;

            Construct();
        }

        private void Construct()
        {
            // Speed in units/sec. Right now 1 unit = 1 pixel
            speed = 200.0f;
            TextureAssetName = "Sprites/Car";

            Tag = "Car";

            Z = 100;

            LoadContent();

            Collider = new BoxCollider(this, Size);

            carSound = new AudioSource();
            carSound.AddSoundEffect(ResourceManager.GetManager().GetResource<SoundEffect>("Audio/AudioFX/Car_sound"), 500);
            carSound.Position = Position;
            AudioManager.AddAudioSource(carSound);
            carSound.Pause();
        }

        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();
            carSound.PlayAll();
        }

        public override void OnLevelUnloaded()
        {
            base.OnLevelUnloaded();
            carSound.Pause();
        }

        public void ChangeDirection(Direction newDirection)
        {
            direction = newDirection;
        }

        private void FollowRoute()
        {
            var locations = turns.Keys;
            foreach (var location in locations)
            {
                if (Vector2.DistanceSquared(Position, location) < 25)
                {
                    ChangeDirection(turns[location]);
                }
            }
        }

        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);
            if (!Enabled)
                return;

            if (direction == Direction.East)
            {
                Move(new Vector2(speed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0));
            }
            else if (direction == Direction.West)
            {
                Move(new Vector2(-speed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0));
            }
            else if (direction == Direction.North)
            {
                Move(new Vector2(0, -speed * (float)gameTime.ElapsedGameTime.TotalSeconds));
            }
            else if (direction == Direction.South)
            {
                Move(new Vector2(0, speed * (float)gameTime.ElapsedGameTime.TotalSeconds));
            }

            if (follow)
            {
                FollowRoute();
            }
            else
            {
                if (Position.X > 5000)
                {
                    Move(new Vector2(-6294, 0));
                }
            }

            carSound.Position = Position;
        }
    }
}

