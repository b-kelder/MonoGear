using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGear
{
    class Guard : WorldEntityAnimated
    {
        private static Texture2D alertSprite;
        private static Texture2D searchSprite;

        enum State
        {
            Idle,
            Alerted,
            Patrolling,
            ToPatrol,
            ToAlert,
            Searching,
        }

        float walkSpeed;
        float runSpeed;

        float hearingRange;
        float viewRange;
        float viewAngle;
        private Player player;
        private Vector2 playerPos;

        public List<Vector2> PatrolPath
        {
            get
            {
                return patrolPath;
            }
            set
            {
                patrolPath = value;
                patrolPathIndex = 0;

                if (state == State.Patrolling)
                {
                    StartPatrol();
                }
            }
        }
        private int patrolPathIndex;

        private List<Vector2> patrolPath;
        private List<Vector2> currentPath;
        private int currentPathIndex;

        private float searchTime;
        private float searchStartTime;

        State state;

        public Guard()
        {
            // Speed in units/sec. Right now 1 unit = 1 pixel
            walkSpeed = 60.0f;
            runSpeed = 90.0f;
            searchTime = 2.5f;  // sec

            hearingRange = 75f;
            viewRange = 450.0f;
            viewAngle = 90f;

            TextureAssetName = "Sprites/Guard";

            AnimationLength = 3;
            AnimationCurrentFrame = 1;
            AnimationDelta = 0.05f;
            AnimationPingPong = true;
            AnimationRunning = false;

            Tag = "Guard";

            Z = 100;

            LoadContent();

            Collider = new BoxCollider(this, new Vector2(8));
        }

        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();
            player = MonoGearGame.FindEntitiesWithTag("Player")[0] as Player;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            if (alertSprite == null)
            {
                alertSprite = ResourceManager.GetManager().GetResource<Texture2D>("Sprites/Alert");
            }
            if (searchSprite == null)
            {
                searchSprite = ResourceManager.GetManager().GetResource<Texture2D>("Sprites/Searching");
            }
        }

        public override void Update(Input input, GameTime gameTime)
        {
            if (!Enabled)
                return;

            base.Update(input, gameTime);

            // Follow current path

            if (currentPath != null && currentPathIndex >= 0)
            {
                AnimationRunning = true;
                if (currentPathIndex < currentPath.Count && state != State.ToAlert)
                {
                    var target = currentPath[currentPathIndex];
                    if (Vector2.DistanceSquared(Position, target) < 24)
                    {
                        currentPathIndex++;
                        if (state == State.Patrolling)  // Loop path when patrolling
                        {
                            if (currentPathIndex >= currentPath.Count)
                            {
                                currentPathIndex = 0;
                            }
                        }
                    }
                    else
                    {
                        Rotation = MathExtensions.VectorToAngle(target - Position);

                        var delta = MathExtensions.AngleToVector(Rotation) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (state == State.Alerted)
                        {
                            delta *= runSpeed;
                            AnimationDelta = 0.05f;
                        }
                        else
                        {
                            delta *= walkSpeed;
                            AnimationDelta = 0.1f;
                        }
                        Move(delta);
                    }
                }
                else
                {
                    // Reached end of path or waiting for a new one
                    currentPathIndex = -1;

                    if (state == State.Alerted)
                    {
                        StartSearch(gameTime);
                    }
                    else if (state == State.ToPatrol)
                    {
                        currentPath = PatrolPath;
                        currentPathIndex = patrolPathIndex;
                        state = State.Patrolling;
                    }
                }
            }

            // State stuff
            if (state == State.Idle)
            {
                StartPatrol();

                AnimationRunning = false;
                AnimationCurrentFrame = 1;
            }
            else if (state == State.Searching)
            {
                // Wait a little bit at the spot when 'searching'
                if (gameTime.TotalGameTime.TotalSeconds >= searchStartTime + searchTime)
                {
                    state = State.Idle;
                }
                else
                {
                    AnimationRunning = false;
                    AnimationCurrentFrame = 1;
                }
            }
            if (input.IsKeyPressed(Keys.G))
            {
                var bullet = new Projectile("Sprites/Bullet", "Bullet", MonoGearGame.FindEntitiesOfType<Guard>()[0].Collider);
                bullet.Position = Position;
                bullet.Rotation = Rotation;
                MonoGearGame.RegisterLevelEntity(bullet);
                AudioManager.PlayOnce(ResourceManager.GetManager().GetResource<SoundEffect>("Audio/AudioFX/Gunshot"), 1);
            }

            if (CanSee(out playerPos) && state != State.Alerted && state != State.ToAlert)
            {
                state = State.ToAlert;
                Alert(playerPos);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (!Visible)
                return;

            // TODO: Draw question mark (or something like that) when state == Seaching
            if (state == State.ToAlert || state == State.Alerted)
            {
                spriteBatch.Draw(alertSprite, new Vector2(Position.X, Position.Y - 16), alertSprite.Bounds, Color.White, 0, new Vector2(alertSprite.Bounds.Size.X, alertSprite.Bounds.Size.Y) / 2, 1, SpriteEffects.None, 0);
            }
            else if (state == State.Searching)
            {
                spriteBatch.Draw(searchSprite, new Vector2(Position.X, Position.Y - 16), searchSprite.Bounds, Color.White, 0, new Vector2(searchSprite.Bounds.Size.X, searchSprite.Bounds.Size.Y) / 2, 1, SpriteEffects.None, 0);
            }
        }

        private void StartSearch(GameTime gameTime)
        {
            searchStartTime = (float)gameTime.TotalGameTime.TotalSeconds;
            state = State.Searching;
        }

        /// <summary>
        /// Starts guard patrol route if possible, changes state
        /// </summary>
        public void StartPatrol()
        {
            if (PatrolPath == null)
            {
                state = State.Idle;
                return;
            }

            state = State.ToPatrol;
            GoTo(PatrolPath[patrolPathIndex]);
        }

        /// <summary>
        /// Alerts a guard to the specified position and changes state
        /// </summary>
        /// <param name="origin"></param>
        public async void Alert(Vector2 origin)
        {
            if (state == State.Patrolling)
            {
                patrolPathIndex = currentPathIndex;
            }

            state = State.ToAlert;

            AudioManager.PlayOnce(ResourceManager.GetManager().GetResource<SoundEffect>("Audio/AudioFX/Guard_Alert_Sound"), 1);
            await Task.Delay(1000);

            Task.Run(() =>
            {
                Pathfinding.FindPath(Position, origin, (path) =>
                {
                    currentPath = path;
                    currentPathIndex = 0;
                    state = path != null ? State.Alerted : State.Idle;
                });
            });
        }

        /// <summary>
        /// Makes the guard move to the specified target without changing state
        /// </summary>
        /// <param name="target"></param>
        public void GoTo(Vector2 target)
        {
            Task.Run(() =>
            {
                Pathfinding.FindPath(Position, target, (path) =>
                {
                    currentPath = path;
                    currentPathIndex = 0;
                });
            });
        }

        public void MoveTo(Vector2 position)
        {
            this.Position = position;
        }

        public bool CanSee(out Vector2 entityPos)
        {
            var dis = Vector2.Distance(Position, player.Position);

            //Check if player is within view range
            if (dis < viewRange)
            {
                //Check to see if the guard is looking at the player
                var degrees = Math.Abs(MathHelper.ToDegrees(Rotation) - (90 + MathHelper.ToDegrees(MathExtensions.AngleBetween(Position, player.Position))));
                if (degrees <= (viewAngle / 2) || degrees >= (360 - (viewAngle / 2)))
                {
                    //Check to see if nothing blocks view of the player
                    Collider hit;
                    Collider.RaycastAny(Position, player.Position, out hit, Tag);              
                    if (hit.Entity.Tag.Equals("Player"))
                    {
                        entityPos = hit.Entity.Position;
                        return true;
                    }
                }
            }

            if (dis < hearingRange && !player.sneakMode)
            {
                entityPos = player.Position;
                return true;
            }

            entityPos = new Vector2();
            return false;
        }
    }
}
