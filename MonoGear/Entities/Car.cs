using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.Threading.Tasks;
using MonoGear.Engine;
using MonoGear.Engine.Collisions;
using MonoGear.Engine.Audio;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGear.Entities
{
    /// <summary>
    /// Car
    /// </summary>
    class Car : WorldEntity, IDestroyable
    {
        float speed;
        private List<Vector2> currentPath;
        private int currentPathIndex;
        public bool LoopPath { get; set; }
        public float Health { get; private set; }

        private PositionalAudio carSound;
        private Texture2D destroyedSprite;

        /// <summary>
        /// Constructor of the car class. Creates an instance of a car at a given position.
        /// </summary>
        /// <param name="position">The position of the car</param>
        /// <param name="currentPath">The path the car should follow</param>
        /// <param name="textureAssetName">The texture of the car</param>
        public Car(Vector2 position, List<Vector2> currentPath, string textureAssetName)
        {
            Position = position;

            // Speed in units/sec. Right now 1 unit = 1 pixel
            speed = 200.0f;
            TextureAssetName = textureAssetName;

            Tag = "Car";

            this.currentPath = currentPath;
            LoopPath = true;

            Z = 1;
            Health = 15;

            LoadContent();

            Collider = new BoxCollider(this, Size);
        }

        public async void GoTo(Vector2 origin)
        {
            Task.Run(() =>
            {
                Pathfinding.FindPath(Position, origin, (path) =>
                {
                    currentPath = path;
                    currentPathIndex = 0;
                });
            });
        }

        /// <summary>
        /// Method that executes when the level is loaded.
        /// </summary>
        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();
            // Add a sound effect to the car
            carSound = AudioManager.AddPositionalAudio(MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Deja Vu"), 0.3f, 500, Position, true);

            destroyedSprite = MonoGearGame.GetResource<Texture2D>("Sprites/BrokenCar");
        }

        /// <summary>
        /// Method that executes when the level is loaded.
        /// </summary>
        public override void OnLevelUnloaded()
        {
            base.OnLevelUnloaded();
        }

        /// <summary>
        /// Method that sets the path of the car
        /// </summary>
        /// <param name="path">The path</param>
        public void SetPath(List<Vector2> path)
        {
            currentPath = path;
            currentPathIndex = 0;
        }

        /// <summary>
        /// Method that updates the game
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="gameTime">GameTime</param>
        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);

            // Check if the car has a path
            if(currentPath != null && currentPathIndex >= 0)
            {
                // Check if the car hasn't finished its path yet
                if(currentPathIndex < currentPath.Count)
                {
                    var target = currentPath[currentPathIndex];
                    if(Vector2.DistanceSquared(Position, target) < 24)
                    {
                        currentPathIndex++;
                        if(LoopPath && currentPathIndex >= currentPath.Count)
                        {
                            currentPathIndex = 0;
                        }
                    }
                    else
                    {
                        Rotation = MathExtensions.VectorToAngle(target - Position);

                        var delta = MathExtensions.AngleToVector(Rotation) * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        Move(delta);
                    }
                }
                else
                {
                    currentPathIndex = -1;
                }
            }
            // Set the sound of the car to the position of the car
            carSound.Position = Position;
        }

        public void Damage(float damage)
        {
            Health -= damage;

            if (Health <= 0)
            {
                Destroy();
            }
        }

        public void Destroy()
        {
            AudioManager.StopPositional(carSound);
            instanceTexture = destroyedSprite;
            Enabled = false;
        }
    }
}


