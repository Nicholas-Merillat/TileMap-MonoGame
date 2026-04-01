using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Net.Mime;

namespace TileMap
{
    public class Game1 : Game
    {
        private readonly Vector2 screenSize = new Vector2(1280, 720);
        private Vector2 viewportSize = new Vector2(640, 360);
        private int screenScaleFactor = 1280 / 640;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        private RenderTarget2D viewport;

        private Texture2D texture;
        private TileMap tilemap;
        private Camera camera;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
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

            viewport = new RenderTarget2D(GraphicsDevice, (int)viewportSize.X, (int)viewportSize.Y);

            texture = Content.Load<Texture2D>("grass");
            camera = new Camera(Vector2.Zero);
            tilemap = new TileMap(screenSize, new Vector2(100, 50), 8, texture, camera);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            //if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            //{
            //    viewportSize.X -= 2;
            //    viewportSize.Y -= 2 / (screenSize.X / screenSize.Y);
            //    viewport = new RenderTarget2D(GraphicsDevice, (int)viewportSize.X, (int)viewportSize.Y);
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            //{
            //    viewportSize.X += 2;
            //    viewportSize.Y += 2 / (screenSize.X / screenSize.Y);
            //    viewport = new RenderTarget2D(GraphicsDevice, (int)viewportSize.X, (int)viewportSize.Y);
            //}

            camera.Update();
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
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
