using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGear
{
    /// <summary>
    /// Car
    /// </summary>
    class Car : WorldEntity
    {
        float speed;
        private AudioSource carSound;
        private List<Vector2> currentPath;
        private int currentPathIndex;
        public bool LoopPath { get; set; }

        public Car(Vector2 position, List<Vector2> currentPath, string textureAssetName)
        {
            Position = position;

            // Speed in units/sec. Right now 1 unit = 1 pixel
            speed = 200.0f;
            TextureAssetName = textureAssetName;

            Tag = "Car";

            this.currentPath = currentPath;
            LoopPath = true;

            Z = 100;

            LoadContent();

            Collider = new BoxCollider(this, Size);

            carSound = new AudioSource();
            carSound.AddSoundEffect(MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Deja Vu"), 500);
            carSound.Position = Position;
            AudioManager.AddAudioSource(carSound);
            carSound.Pause();
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

        public void SetPath(List<Vector2> path)
        {
            currentPath = path;
            currentPathIndex = 0;
        }

        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);

            if(currentPath != null && currentPathIndex >= 0)
            {
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

            carSound.Position = Position;
        }
    }
}

