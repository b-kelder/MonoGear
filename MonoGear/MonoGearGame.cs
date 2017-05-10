using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace MonoGear
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MonoGearGame : Game
    {
        ResourceManager globalResources;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        List<WorldEntity> activeEntities;
        Input input;
        Camera activeCamera;
        Level activeLevel;
        AudioManager audioManager;

        public MonoGearGame()
        {
            audioManager = new AudioManager();
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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

            activeEntities = new List<WorldEntity>();
            activeCamera = new Camera(graphics.GraphicsDevice.Viewport);
            activeCamera.Zoom = 2;
            globalResources = new ResourceManager("Global");

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

            // TODO: use this.Content to load your game content here
            globalResources.LoadResources(Content);

            // Test crap
            activeLevel = new Level();
            var layer = new LevelLayer()
            {
                layer = 0,
                offset = new Vector2(),
                textureName = "Sprites/map"
            };
            activeLevel.AddBackgroundLayer(layer);

            var player = new Player();
            var testDing = new SoundSource(new List<SoundEffectInstance>() { Content.Load<SoundEffect>("Sound/SoundFX/Running_On_Concrete").CreateInstance() });

            activeEntities.Add(player);
            activeEntities.Add(testDing);

            //Background music
            Song song = Content.Load<Song>("Sound/Music/Main menu theme");
            //MediaPlayer.Play(song);
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
            input.Update();

            // TODO: Add your update logic here
            foreach(var entity in activeEntities)
            {
                entity.Update(input, gameTime);
            }
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            var matrix = activeCamera.GetViewMatrix();
            spriteBatch.Begin(transformMatrix: matrix);

            activeLevel.DrawBackground(spriteBatch);

            foreach(var entity in activeEntities)
            {
                entity.Draw(spriteBatch);
            }

            activeLevel.DrawForeground(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
