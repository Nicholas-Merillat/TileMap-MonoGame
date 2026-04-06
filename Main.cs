using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace TileMap
{
    public class Main : Game
    {
        private const int PhysicsFPS = 60;

        private Vector2 screenSize = GameSettings.Data.screenSize;
        private Vector2 viewportSize = GameSettings.Data.viewportSize;
        private float screenScaleFactor = GameSettings.Data.screenSize.X / GameSettings.Data.viewportSize.X;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private MouseState _currentMouseState;
        private MouseState _previousMouseState;
        private SpriteFont _font;

        private float deltaTime;
        private float fps;
        private RenderTarget2D viewport;

        private Texture2D[] tileTextures;
        private Texture2D playerTexture;
        private TileMap tilemap;
        private Camera camera;
        private Player player;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Uncaps the fps
            _graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = (int)screenSize.X;
            _graphics.PreferredBackBufferHeight = (int)screenSize.Y;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("Fonts/Font");

            viewport = new RenderTarget2D(GraphicsDevice, (int)viewportSize.X, (int)viewportSize.Y);

            tileTextures = new Texture2D[3];
            for (int i=0; i < tileTextures.Length; i++)
            {
                if (i == 0)
                {
                    tileTextures[i] = (Content.Load<Texture2D>("Images/Grass"));
                    continue;
                }
                tileTextures[i] = (Content.Load<Texture2D>("Images/Stone"));
            }

            playerTexture = Content.Load<Texture2D>("Images/UFOMater");
            camera = new Camera(Vector2.Zero);
            tilemap = new TileMap(viewportSize, GameSettings.Data.tileMapSize, GameSettings.Data.tileSize, tileTextures, camera);
            player = new Player(Vector2.Zero, playerTexture, tilemap);
        }

        protected override void Update(GameTime gameTime)
        {
            deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds * PhysicsFPS;
            fps = 1 / (deltaTime / PhysicsFPS);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            player.Update(deltaTime);
            camera.Update(deltaTime, player);
            tilemap.Update(_currentMouseState, screenScaleFactor);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(viewport);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Game stuff
            _spriteBatch.Begin();

            tilemap.Draw(_spriteBatch);
            player.Draw(_spriteBatch, camera.position);

            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Scale everything back up from 640x360
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            _spriteBatch.Draw(viewport, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), Color.White);
            _spriteBatch.DrawString(_font, ((int)fps).ToString(), Vector2.Zero, Color.Yellow);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
