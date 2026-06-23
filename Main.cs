using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace TileMap
{
    public class Main : Game
    {
        private const int PhysicsFPS = 60;

        private float screenScaleFactor;
        private bool fullscreenKeyPressed;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        private SpriteFont _fontSmall;
        private SpriteFont _fontMedium;

        private RenderTarget2D viewport;
        private float deltaTime;
        private float fps;

        private Texture2D[] tileTextures;
        private Texture2D playerTexture;
        private Texture2D cursorTexture;

        private TileMap tilemap;
        private Camera camera;
        private Player player;
        private Entity entity;

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
            screenScaleFactor = GameSettings.Data.ScreenSize.X / GameSettings.Data.WindowSize.X;
            _graphics.PreferredBackBufferWidth = (int)GameSettings.Data.WindowSize.X;
            _graphics.PreferredBackBufferHeight = (int)GameSettings.Data.WindowSize.Y;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _fontSmall = Content.Load<SpriteFont>("Fonts/FontSmall");
            _fontMedium = Content.Load<SpriteFont>("Fonts/FontMedium");

            viewport = new RenderTarget2D(GraphicsDevice, (int)GameSettings.Data.ViewportSize.X, (int)GameSettings.Data.ViewportSize.Y);

            tileTextures = new Texture2D[6];
            for (int i=0; i < tileTextures.Length; i++)
            {
                tileTextures[i] = Content.Load<Texture2D>($"Images/{(Tile.Block)i+1}");
            }

            playerTexture = Content.Load<Texture2D>("Images/UFOMater");
            cursorTexture = Content.Load<Texture2D>("Images/White");

            camera = new Camera(Vector2.Zero);
            tilemap = new TileMap(GameSettings.Data.TileMapSize, tileTextures, camera, cursorTexture);
            player = new Player(Vector2.Zero, playerTexture, tilemap);
            entity = new Entity(new Vector2(100, 100), cursorTexture, tilemap);
        }

        protected override void Update(GameTime gameTime)
        {
            deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds * PhysicsFPS;
            fps = 1 / (deltaTime / PhysicsFPS);

            InputManager.Update();

            if (Keyboard.GetState().IsKeyDown(Keys.LeftAlt))
            {
                if (!fullscreenKeyPressed)
                {
                    fullscreenKeyPressed = true;
                    if (_graphics.IsFullScreen)
                    {
                        _graphics.PreferredBackBufferWidth = (int)GameSettings.Data.WindowSize.X;
                        _graphics.PreferredBackBufferHeight = (int)GameSettings.Data.WindowSize.Y;
                    }
                    else
                    {
                        _graphics.PreferredBackBufferWidth = (int)GameSettings.Data.ScreenSize.X;
                        _graphics.PreferredBackBufferHeight = (int)GameSettings.Data.ScreenSize.Y;
                    }
                    screenScaleFactor = _graphics.PreferredBackBufferWidth / GameSettings.Data.ViewportSize.X;
                    _graphics.ToggleFullScreen();
                    _graphics.ApplyChanges();
                }
            }
            else
            {
                fullscreenKeyPressed = false;
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            player.Update(deltaTime);
            entity.Update(deltaTime);
            camera.Update(deltaTime, player);
            tilemap.Update(screenScaleFactor);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(viewport);
            GraphicsDevice.Clear(new Color(150,255,255));

            // Game stuff
            _spriteBatch.Begin(blendState: BlendState.AlphaBlend);

            tilemap.Draw(_spriteBatch);
            player.Draw(_spriteBatch, camera.position);
            entity.Draw(_spriteBatch, camera.position);

            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(new Color(150, 255, 255));

            // Scale everything back up from 640x360
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);

            _spriteBatch.Draw(viewport, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), Color.White);
            _spriteBatch.DrawString(_fontMedium, ((int)fps).ToString(), new Vector2(5, 0), Color.Black);
            _spriteBatch.DrawString(_fontSmall, ("playerPosition: " + (int)player.position.X) + " , " + ((int)player.position.Y), new Vector2(5, 40), Color.Black);
            _spriteBatch.DrawString(_fontSmall, ("cameraPosition: " + (int)camera.position.X) + " , " + ((int)camera.position.Y), new Vector2(5, 65), Color.Black);
            _spriteBatch.DrawString(_fontSmall, ("renderedTiles: " + tilemap.renderedTiles), new Vector2(5, 90), Color.Black);
            _spriteBatch.DrawString(_fontSmall, ("activeBlock: " + tilemap.activeBlock).ToString(), new Vector2(5, 115), Color.Black);
            _spriteBatch.DrawString(_fontSmall, ("placingWalls: " + tilemap.placingWalls), new Vector2(5, 140), Color.Black);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
