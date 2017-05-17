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
                    activeCamera.RecalculateOrigin(graphics.GraphicsDevice.Viewport);
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

            // Add background music
            //AudioManager.MusicSet(globalResources.GetResource<Song>("Audio/Music/Main menu theme"));
           //AudioManager.MusicVolume(0.4f);
           // AudioManager.MusicPlay();

            // New Audio Source
            var fountain = new AudioSource();
            fountain.AddSoundEffect(globalResources.GetResource<SoundEffect>("Audio/AudioFX/Water_Fountain_cut"), 150);
            fountain.Position = new Vector2(207, 220);
            AudioManager.AddAudioSource(fountain);
            RegisterGlobalEntity(fountain);

            // Bird sound
            //AudioManager.GlobalAudioPlay(globalResources.GetResource<SoundEffect>("Audio/AudioFX/Bird_sounds").CreateInstance(), true, 0.3f);

            // Water drop sound
            var puddle = new AudioSource();
            puddle.AddSoundEffect(globalResources.GetResource<SoundEffect>("Audio/AudioFX/Water_Drop_Sound"), 10);
            puddle.Position = new Vector2(279, 458);
            AudioManager.AddAudioSource(puddle);
            RegisterGlobalEntity(puddle);

            player = new Player();
            RegisterGlobalEntity(player);

            // Test level
            var lvl = new Level();
            var layer = new LevelLayer()
            {
                layer = 0,
                offset = new Vector2(),
                textureName = "Sprites/map"
            };
            lvl.AddBackgroundLayer(layer);
            var sp = new SpawnPoint(new Vector2(64, 120));
            sp.Tag = "PlayerSpawnPoint";
            lvl.AddEntity(sp);
            var col = new ColliderTestEntity();
            col.Position = new Vector2(100, 100);
            lvl.AddEntity(col);
            var bird = new Bird();
            bird.Position = new Vector2(200, 500);
            lvl.AddEntity(bird);
            bird = new Bird();
            bird.Position = new Vector2(220, 520);
            lvl.AddEntity(bird);
            bird = new Bird();
            bird.Position = new Vector2(180, 520);
            lvl.AddEntity(bird);

            var tilemap = new TilemapCollider(new WorldEntity() {
                Tag = "Tilemap"
            }, new ushort[0, 0], 16);
            tilemap.MapFromString(@"0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 1, 1, 3, 3, 3, 3, 3, 0, 0, 0, 0,
                                    1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 3, 3, 3, 3, 3, 0, 0, 0, 0,
                                    1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 3, 3, 3, 3, 3, 0, 0, 0, 0,
                                    1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 3, 3, 3, 3, 3, 0, 0, 0, 0,
                                    1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 3, 3, 3, 3, 3, 0, 0, 0, 0,
                                    1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 3, 3, 3, 3, 3, 0, 0, 0, 0,
                                    1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 3, 3, 3, 3, 3, 0, 0, 0, 0,
                                    1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 3, 3, 3, 3, 3, 0, 0, 0, 0,
                                    1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 3, 3, 2, 3, 3, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0,
                                    1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0,
                                    1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0,
                                    1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0,
                                    1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                                    ", 35, 42);
            lvl.AddEntity(tilemap.Entity);

            var levelA = lvl;


            // Test level
            lvl = new Level();
            layer = new LevelLayer()
            {
                layer = 0,
                offset = new Vector2(),
                textureName = "Sprites/map"
            };
            lvl.AddBackgroundLayer(layer);
            sp = new SpawnPoint(new Vector2(264, 120));
            sp.Tag = "PlayerSpawnPoint";
            lvl.AddEntity(sp);

            bird = new Bird();
            bird.Position = new Vector2(200, 500);
            lvl.AddEntity(bird);
            bird = new Bird();
            bird.Position = new Vector2(220, 520);
            lvl.AddEntity(bird);
            bird = new Bird();
            bird.Position = new Vector2(180, 520);
            lvl.AddEntity(bird);

            bird = new Bird();
            bird.Position = new Vector2(200, 400);
            lvl.AddEntity(bird);
            bird = new Bird();
            bird.Position = new Vector2(220, 420);
            lvl.AddEntity(bird);
            bird = new Bird();
            bird.Position = new Vector2(180, 420);
            lvl.AddEntity(bird);

            bird = new Bird();
            bird.Position = new Vector2(100, 500);
            lvl.AddEntity(bird);
            bird = new Bird();
            bird.Position = new Vector2(120, 520);
            lvl.AddEntity(bird);
            bird = new Bird();
            bird.Position = new Vector2(80, 520);
            lvl.AddEntity(bird);


            lvl.AddEntity(tilemap.Entity);


            var levelB = lvl;

            // Level transition trigger
            var trans = new WorldBoxTrigger(new Vector2(208, 8), new Vector2(96, 16), (c, prev, current) => 
            {
                foreach(var collider in current)
                {
                    if(prev.Contains(collider))
                    {
                        continue;
                    }

                    if(collider.Entity.Tag == "Player")
                    {
                        LoadLevel(levelB);
                    }
                }
            });
            levelA.AddEntity(trans);

            LoadLevel(levelA);
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
                entity.Update(input, gameTime);
            }
            foreach (var entity in levelEntities)
            {
                entity.Update(input, gameTime);
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
                entity.Draw(spriteBatch);
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
        public static List<WorldEntity> FindEntitiesOfType<T>() where T : WorldEntity
        {
            var list = new List<WorldEntity>();
            list.AddRange(instance.levelEntities.Where(
                ent =>
                {
                    return typeof(T).IsAssignableFrom(ent.GetType());
                }));
            list.AddRange(instance.globalEntities.Where(
                ent =>
                {
                    return typeof(T).IsAssignableFrom(ent.GetType());
                }));

            return list;
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
            }
        }

        // static level stuff
        public static void LoadLevel(Level level)
        {
            instance.nextLevel = level;
        }
    }
}
