using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Net.Mime;
using static System.Net.Mime.MediaTypeNames;

namespace TileMap
{
    public class Main : Game
    {
        private readonly int physicsFPS = 60;

        private readonly Vector2 screenSize = new Vector2(1280, 720);
        private Vector2 viewportSize = new Vector2(640, 360);
        private int screenScaleFactor = 1280 / 640;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private MouseState _currentMouseState;
        private MouseState _previousMouseState;
        private SpriteFont _font;

        private float deltaTime;
        private float fps;
        private RenderTarget2D viewport;

        private Texture2D texture;
        private TileMap tilemap;
        private Camera camera;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Uncaps the fps
            _graphics.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = (int)screenSize.X;
            _graphics.PreferredBackBufferHeight = (int)screenSize.Y;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("font");

            viewport = new RenderTarget2D(GraphicsDevice, (int)viewportSize.X, (int)viewportSize.Y);

            texture = Content.Load<Texture2D>("grass");
            camera = new Camera(Vector2.Zero);
            tilemap = new TileMap(viewportSize, new Vector2(100, 50), 8, texture, camera);
        }

        protected override void Update(GameTime gameTime)
        {
            deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds * physicsFPS;
            fps = 1 / (deltaTime / physicsFPS);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            // If you do not dispose of the viewport before scaling it, you will get genuinely insane memory leaks, like gigabytes
            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {   
                viewport.Dispose();
                viewportSize.X += 2 * deltaTime;
                viewportSize.Y += (2 / (screenSize.X / screenSize.Y)) * deltaTime;
                if (viewportSize.X > 640)
                {
                    viewportSize = new Vector2(640, 360);
                }
                viewport = new RenderTarget2D(GraphicsDevice, (int)viewportSize.X, (int)viewportSize.Y);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            {
                viewport.Dispose();
                viewportSize.X -= 2 * deltaTime;
                viewportSize.Y -= (2 / (screenSize.X / screenSize.Y)) * deltaTime;
                if (viewportSize.X < 10)
                {
                    viewportSize = new Vector2(10, 10 / screenScaleFactor);
                }
                viewport = new RenderTarget2D(GraphicsDevice, (int)viewportSize.X, (int)viewportSize.Y);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Back))
            {
                viewport.Dispose();
                viewportSize = new Vector2(640, 360);
                viewport = new RenderTarget2D(GraphicsDevice, (int)viewportSize.X, (int)viewportSize.Y);
            }

            camera.Update(deltaTime);
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
