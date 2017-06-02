using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace MonoGear
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MonoGearGame : Game
    {
        static MonoGearGame instance;

        /// <summary>
        /// Main resource manager. Name is "Global"
        /// </summary>
        ResourceManager globalResources;
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

        public MonoGearGame()
        {
            // Required for static entity/level related methods
            instance = this;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Window.ClientSizeChanged += (s, e) =>
            {
                if (activeCamera != null)
                {
                    activeCamera.UpdateViewport(graphics.GraphicsDevice.Viewport);
                }
            };
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            input = new Input();

            levelEntities = new HashSet<WorldEntity>();
            globalEntities = new HashSet<WorldEntity>();
            activeCamera = new Camera(graphics.GraphicsDevice.Viewport);
            // TODO: Make zoom based on resolution? Or see if we can change resolution otherwise.
            activeCamera.Zoom = 2;
            globalResources = new ResourceManager();
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load global resources
            globalResources.LoadResources(Content);

            // Load game over screen
            var gameOver = new GameOver();
            RegisterGlobalEntity(gameOver);

            // Background music sould be in a level??
            /*AudioManager.MusicSet(globalResources.GetResource<Song>("Audio/Music/Main menu theme"));
            AudioManager.MusicVolume(0.4f);
            AudioManager.MusicPlay();*/

            // GLOBALS
            player = new Player();
            RegisterGlobalEntity(player);
            var pf = new Pathfinding();
            RegisterGlobalEntity(pf);
			
            var level = Level.LoadLevel("WhiteHouse");
            LoadLevel(level);
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
            if(nextLevel != null)
            {
                LoadLevel();
            }

            input.Update();

            // Level reload by pressing L
            if (input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.L))
            {
                LoadLevel(activeLevel);
            }

            // Globals go first
            foreach (var entity in globalEntities)
            {
                if(entity.Enabled)
                {
                    entity.Update(input, gameTime);
                }
            }
            foreach (var entity in levelEntities)
            {
                if(entity.Enabled)
                {
                    entity.Update(input, gameTime);
                }
            }

            AudioManager.UpdateAudioSourceAudio(player);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            var matrix = activeCamera.GetViewMatrix();
            spriteBatch.Begin(transformMatrix: matrix);

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

            activeLevel.DrawForeground(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        // Static entity stuff
        /// <summary>
        /// Adds an entity to the level list.
        /// </summary>
        /// <param name="entity">Entity to add</param>
        public static void RegisterLevelEntity(WorldEntity entity)
        {
            entity.OnLevelLoaded();
            instance.levelEntities.Add(entity);
        }

        /// <summary>
        /// Adds an entity to the global list.
        /// </summary>
        /// <param name="entity">Entity to add</param>
        public static void RegisterGlobalEntity(WorldEntity entity)
        {
            entity.OnLevelLoaded();
            instance.globalEntities.Add(entity);
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
        public static List<T> FindEntitiesOfType<T>() where T : WorldEntity
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

        public static Level GetCurrentLevel()
        {
            return instance.activeLevel;
        }

        private void LoadLevel()
        {
            if(nextLevel != null)
            {
                activeLevel = nextLevel;
                nextLevel = null;

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

                // Force GC
                GC.Collect();
            }
        }

        // static level stuff
        public static void LoadLevel(Level level)
        {
            instance.nextLevel = level;
        }
    }
}
