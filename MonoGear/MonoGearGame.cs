using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using Windows.UI.Xaml.Controls;
using System.Xml.Serialization;
using System.IO;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.ViewManagement;
using Windows.Graphics.Display;

using MonoGear.Engine;
using MonoGear.Engine.Audio;
using MonoGear.Entities;
using Microsoft.Xna.Framework.Media;

namespace MonoGear
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MonoGearGame : Game
    {
        /// <summary>
        /// Game instance
        /// </summary>
        static MonoGearGame instance;
        /// <summary>
        /// GDM
        /// </summary>
        GraphicsDeviceManager graphics;
        /// <summary>
        /// Main sprite batch
        /// </summary>
        SpriteBatch spriteBatch;
        /// <summary>
        /// Global entity set. Stays between levels and updates first in Update().
        /// </summary>
        HashSet<WorldEntity> globalEntities;
        /// <summary>
        /// Level entity set, gets wiped on level change.
        /// </summary>
        HashSet<WorldEntity> levelEntities;
        /// <summary>
        /// Input object
        /// </summary>
        Input input;
        /// <summary>
        /// Active camera used for rendering
        /// </summary>
        Camera activeCamera;
        /// <summary>
        /// Active level, used for background rendering
        /// </summary>
        Level activeLevel;
        /// <summary>
        /// If set will be loaded the next frame
        /// </summary>
        Level nextLevel;
        /// <summary>
        /// Active Player
        /// </summary>
        Player player;
        LevelListData levelList;

        Queue<WorldEntity> spawnQueueLocal;
        Queue<WorldEntity> spawnQueueGlobal;

        Queue<WorldEntity> destroyQueue;
                           
        public MonoGearGame()
        {
            // Required for static entity/level related methods
            instance = this;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Event that triggers when the client size is changed
            Window.ClientSizeChanged += (s, e) =>
            {
                if (activeCamera != null)
                {
                    // Update viewport size
                    activeCamera.UpdateViewport(graphics.GraphicsDevice.Viewport);
                }
            };
        }

        /// <summary>
        /// Method that restarts the level.
        /// </summary>
        public static void Restart()
        {
            // Check if the instance is not null
            if(instance != null)
            {
                LoadLevel(instance.levelList.Start());
            }
        }

        /// <summary>
        /// Method that starts the next level.
        /// </summary>
        public static void NextLevel()
        {
            // Check if the instance is not null
            if(instance != null)
            {
                // Check if the current level is not the last level
                if(instance.levelList.LastLevel() != instance.activeLevel.Name)
                {
                    // Load the next level
                    LoadLevel(instance.levelList.NextLevel());
                }
            }
        }

        /// <summary>
        /// Method that updates the game's difficulty
        /// </summary>
        private void UpdateDifficulty()
        {
            var sightRange = 295f;
            var runSpeed = 90.0f;
            var walkSpeed = 35.0f;

            var dif = SettingsPage.Difficulty;
            if (dif.Equals(DifficultyLevels.Intern))
            {
                player.DartCount = 5;
                player.Health = 5;
            }
            else if(dif.Equals(DifficultyLevels.Professional))
            {
                player.DartCount = 2;
                player.Health = 3;
                sightRange += 10;
                runSpeed += 10;
                walkSpeed += 10;
            }
            else if(dif.Equals(DifficultyLevels.Veteran))
            {
                player.DartCount = 1;
                player.Health = 2;
                sightRange += 20;
                runSpeed += 20;
                walkSpeed += 20;
            }
            else if(dif.Equals(DifficultyLevels.JamesBond))
            {
                player.DartCount = 0;
                player.Health = 1;
                sightRange += 30;
                runSpeed += 30;
                walkSpeed += 30;
            }

            var guards = FindEntitiesWithTag("Guard");
            // Set the stats for each guard based on the selected difficulty
            foreach (var guard in guards)
            {
                var g = guard as Guard;
                if (g != null)
                {
                    g.SightRange = sightRange;
                    g.RunSpeed = runSpeed;
                    g.WalkSpeed = walkSpeed;
                }
            }
            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Create objects
            input = new Input();
            levelEntities = new HashSet<WorldEntity>();
            globalEntities = new HashSet<WorldEntity>();
            spawnQueueGlobal = new Queue<WorldEntity>();
            spawnQueueLocal = new Queue<WorldEntity>();
            destroyQueue= new Queue<WorldEntity>();

            // Set up camera
            activeCamera = new Camera(graphics.GraphicsDevice.Viewport);
            // Zoom based on screen size
            activeCamera.Zoom = graphics.GraphicsDevice.Viewport.Height / 320;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load level list
            Task.Run(async () =>
            {
                var sf = await Package.Current.InstalledLocation.TryGetItemAsync(@"Content\Levels\LevelList.xml") as StorageFile;

                // Parse xml, if it's malformed we can't do anything with the game anyway so we just crash
                using(var stream = await sf.OpenStreamForReadAsync())
                {
                    XmlSerializer xml = new XmlSerializer(typeof(LevelListData));
                    levelList = xml.Deserialize(stream) as LevelListData;
                }
            }).Wait();

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load game over screen
            var gameOver = new GameOver();
            RegisterGlobalEntity(gameOver);

            // Create some global Entities
            player = new Player();
            RegisterGlobalEntity(player);
            var ui = new GameUI();
            RegisterGlobalEntity(ui);
            var pf = new Pathfinding();
            RegisterGlobalEntity(pf);
			
            // Load first level in the list
            LoadLevel(levelList.Start());
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Load level if we must
            if(nextLevel != null)
            {
                LoadLevel();
            }

            // Update input state
            input.Update();

            // Let NetManager update
            Network.NetManager.OnUpdate(destroyQueue, spawnQueueLocal, spawnQueueGlobal, levelEntities, globalEntities);

            // Destroy entities
            while(destroyQueue.Count > 0)
            {
                var entity = destroyQueue.Dequeue();
                entity.OnLevelUnloaded();

                globalEntities.Remove(entity);
                levelEntities.Remove(entity);
            }

            // Register newly spawned entities
            while(spawnQueueGlobal.Count > 0)
            {
                RegisterGlobalEntity(spawnQueueGlobal.Dequeue());
            }

            while(spawnQueueLocal.Count > 0)
            {
                RegisterLevelEntity(spawnQueueLocal.Dequeue());
            }

            // Update entities, globals go first
            foreach(var entity in globalEntities)
            {
                if(entity.Enabled)
                {
                    entity.Update(input, gameTime);
                }
            }
            foreach(var entity in levelEntities)
            {
                if(entity.Enabled)
                {
                    entity.Update(input, gameTime);
                }
            }

            // Update audio
            AudioManager.UpdatePositionalAudio(player);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Clear screen
            GraphicsDevice.Clear(Color.Black);

            // Get camera translation matrix for zoom and offsets
            var matrix = activeCamera.GetViewMatrix();
            spriteBatch.Begin(transformMatrix: matrix, blendState:BlendState.AlphaBlend, samplerState:SamplerState.PointClamp);

            // Draw tiles and background overlays
            activeLevel.DrawTiles(spriteBatch, activeCamera);
            activeLevel.DrawBackground(spriteBatch);

            // Sort entities based on z order
            var renderEntities = new List<WorldEntity>();
            renderEntities.AddRange(levelEntities);
            renderEntities.AddRange(globalEntities);
            renderEntities.Sort((ent, other) =>
            {
                return ent.Z.CompareTo(other.Z);
            });

            foreach(var entity in renderEntities)
            {
                if(entity.Visible)
                {
                    entity.Draw(spriteBatch);
                }
            }

            // Draw foreground overlays
            activeLevel.DrawForeground(spriteBatch);
            spriteBatch.End();


            base.Draw(gameTime);
        }

        /// <summary>
        /// Spawns a level entity.
        /// </summary>
        /// <param name="entity">Entity to spawn</param>
        public static void SpawnLevelEntity(WorldEntity entity)
        {
            instance.spawnQueueLocal.Enqueue(entity);
        }

        /// <summary>
        /// Spawns a global entity.
        /// </summary>
        /// <param name="entity">Entity to spawn</param>
        public static void SpawnGlobalEntity(WorldEntity entity)
        {
            instance.spawnQueueGlobal.Enqueue(entity);
        }

        /// <summary>
        /// Queues an entity for destruction next frame.
        /// </summary>
        /// <param name="entity">Entity to destroy</param>
        public static void DestroyEntity(WorldEntity entity)
        {
            instance.destroyQueue.Enqueue(entity);
        }

        // Static entity stuff
        /// <summary>
        /// Adds an entity to the level list.
        /// </summary>
        /// <param name="entity">Entity to add</param>
        private void RegisterLevelEntity(WorldEntity entity)
        {
            entity.OnLevelLoaded();
            levelEntities.Add(entity);
        }

        /// <summary>
        /// Adds an entity to the global list.
        /// </summary>
        /// <param name="entity">Entity to add</param>
        private void RegisterGlobalEntity(WorldEntity entity)
        {
            entity.OnLevelLoaded();
            globalEntities.Add(entity);
        }

        /// <summary>
        /// Returns a list of all entities that have the given tag.
        /// </summary>
        /// <param name="tag">Tag to find</param>
        /// <returns>List</returns>
        public static List<WorldEntity> FindEntitiesWithTag(string tag)
        {
            var list = new List<WorldEntity>();
            list.AddRange(instance.levelEntities.Where(
                ent =>
                {
                    return ent.Tag == tag;
                }));
            list.AddRange(instance.globalEntities.Where(
                ent =>
                {
                    return ent.Tag == tag;
                }));

            return list;
        }

        /// <summary>
        /// Returns a list of all entities of type T.
        /// </summary>
        /// <typeparam name="T">Type of entity.</typeparam>
        /// <returns>List</returns>
        public static List<T> FindEntitiesOfType<T>()
        {
            var list = new List<T>();
            list.AddRange(instance.levelEntities.Where(
                ent =>
                {
                    return typeof(T).IsAssignableFrom(ent.GetType());
                }).Cast<T>());
            list.AddRange(instance.globalEntities.Where(
                ent =>
                {
                    return typeof(T).IsAssignableFrom(ent.GetType());
                }).Cast<T>());

            return list;
        }

        /// <summary>
        /// Method that returns the current level.
        /// </summary>
        /// <returns>The current level</returns>
        public static Level GetCurrentLevel()
        {
            return instance.activeLevel;
        }

        /// <summary>
        /// Method that loads a level
        /// </summary>
        private void LoadLevel()
        {
            // Check if the next level is not null
            if(nextLevel != null)
            {
                activeLevel = nextLevel;
                nextLevel = null;

                var gameOver = FindEntitiesWithTag("GameOverScreen")[0] as GameOver;
                var hp = player.Health;
                var darts = player.DartCount;

                // Tell entities that they should stop
                foreach(var e in levelEntities)
                {
                    e.OnLevelUnloaded();
                }

                // Update entities
                instance.levelEntities.Clear();
                var ents = activeLevel.GetEntities();
                foreach(var e in ents)
                {
                    levelEntities.Add(e);
                }

                // Do OnLevelLoaded for all entities
                foreach(var e in instance.levelEntities)
                {
                    e.OnLevelLoaded();
                }

                foreach(var e in instance.globalEntities)
                {
                    e.OnLevelLoaded();
                }

                // Clear local spawn queue to prevent them from appearing in the new level
                spawnQueueLocal.Clear();

                // Update difficulty affected variables in entities
                UpdateDifficulty();

                // Check if this was a reset (gameOver true) or a transition
                if (!gameOver.gameOver)
                {
                    player.Health = hp;
                    player.DartCount = darts;
                }
                else
                {
                    gameOver.DisableGameOver();
                }
                
                // Force GC for performance reasons
                GC.Collect();
            }
        }

        /// <summary>
        /// Sets a level to be loaded
        /// </summary>
        /// <param name="levelName">Name of the level</param>
        public static void LoadLevel(string levelName)
        {
            // Clear audio before loading level
            AudioManager.ClearPositionalAudio();
            AudioManager.ClearGlobalAudio();

            instance.nextLevel = Level.LoadLevel(levelName);
        }

        /// <summary>
        /// Returns a resource from the content manager.
        /// </summary>
        /// <typeparam name="T">Type of resource</typeparam>
        /// <param name="name">Name of the resource</param>
        /// <returns>Resource</returns>
        public static T GetResource<T>(string name)
        {
            return instance.Content.Load<T>(name);
        }
    }
}
