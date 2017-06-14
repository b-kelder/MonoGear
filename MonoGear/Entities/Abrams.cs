using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGear.Engine.Audio;
using MonoGear.Engine.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGear.Engine;

namespace MonoGear.Entities
{
    class Abrams : DrivableVehicle
    {
        private PositionalAudio sound;
        private float lastShootTime;
        public float GunCycleTime { get; set; }

        public Abrams()
        {
            TextureAssetName = "Sprites/Abrams";
            Tag = "Abrams M1";
            Speed = 135;
            entered = false;
            stationaryLock = false;

            Z = 1;

            GunCycleTime = 0.8f;

            Acceleration = 70;
            Braking = 140;
            Steering = 60;
            Drag = 70;
            ConstantSteering = true;

            Collider = new BoxCollider(this, new Vector2(24, 24));

            LoadContent();
        }

        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();

            sound = AudioManager.AddPositionalAudio(MonoGearGame.GetResource<SoundEffect>("Audio/AudioFX/Tank Movement"), 0, 300, Position, true);
        }

        public override void Update(Input input, GameTime gameTime)
        {
            base.Update(input, gameTime);

            float minVolume = 0.3f;
            if(entered)
            {
                sound.Position = Position;
                sound.Volume = minVolume + (1.0f - minVolume) * Math.Abs(forwardSpeed) / Speed;


                if(input.IsButtonPressed(Input.Button.Shoot) && lastShootTime + GunCycleTime <= (float)gameTime.TotalGameTime.TotalSeconds)
                {
                    var missile = new Missile(MonoGearGame.FindEntitiesOfType<Player>()[0].Collider);
                    missile.Rotation = Rotation;

                    missile.Position = Position + Forward * 88;

                    MonoGearGame.SpawnLevelEntity(missile);

                    lastShootTime = (float)gameTime.TotalGameTime.TotalSeconds;
                }                   

            }
            else
            {
                sound.Volume = minVolume;
            }
        }
    }
}
