using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace TileMap
{
    public class Main : Game
    {
        private const int PhysicsFPS = 60;

        private readonly Vector2 screenSize = GameSettings.Data.screenSize;
        private readonly Vector2 viewportSize = GameSettings.Data.viewportSize;
        private readonly float screenScaleFactor = GameSettings.Data.screenSize.X / GameSettings.Data.viewportSize.X;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private MouseState _currentMouseState;
        private MouseState _previousMouseState;
        private SpriteFont _fontSmall;
        private SpriteFont _fontMedium;

        private float deltaTime;
        private float fps;
        private RenderTarget2D viewport;

        private Texture2D[] tileTextures;
        private Texture2D playerTexture;
        private TileMap tilemap;
        private Camera camera;
        private Player player;

        private readonly Dictionary<int, string> tileNames = new Dictionary<int, string>
        {
            {0, "Air"},
            {1, "Grass"},
            {2, "Dirt"},
            {3, "Stone"},
            {4, "CobbleStone"},
        };

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
            _fontSmall = Content.Load<SpriteFont>("Fonts/FontSmall");
            _fontMedium = Content.Load<SpriteFont>("Fonts/FontMedium");

            viewport = new RenderTarget2D(GraphicsDevice, (int)viewportSize.X, (int)viewportSize.Y);

            tileTextures = new Texture2D[4];
            for (int i=0; i < tileTextures.Length; i++)
            {
                tileTextures[i] = Content.Load<Texture2D>($"Images/{tileNames[i+1]}");
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
            GraphicsDevice.Clear(new Color(150,255,255));

            // Game stuff
            _spriteBatch.Begin();

            tilemap.Draw(_spriteBatch);
            player.Draw(_spriteBatch, camera.position);

            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            //GraphicsDevice.Clear(Color.CornflowerBlue);

            // Scale everything back up from 640x360
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            _spriteBatch.Draw(viewport, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), Color.White);
            _spriteBatch.DrawString(_fontMedium, ((int)fps).ToString(), new Vector2(5, 0), Color.Black);
            _spriteBatch.DrawString(_fontSmall, ("playerPosition: " + (int)player.position.X) + "," + ((int)player.position.Y), new Vector2(5, 40), Color.Black);
            _spriteBatch.DrawString(_fontSmall, ("cameraPosition: " + (int)camera.position.X) + "," + ((int)camera.position.Y), new Vector2(5, 65), Color.Black);
            _spriteBatch.DrawString(_fontSmall, ("visibleTiles: " + tilemap.visibleTiles), new Vector2(5, 90), Color.Black);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
