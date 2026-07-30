using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TileMap
{
    public class Main : Game
    {   
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SpriteFont _font;

        private TimeSpan _tickAccumulator;
        private readonly TimeSpan tickTargetTime;

        private RenderTarget2D viewport;
        private Vector2 screenSize;
        private Vector2 screenScaleFactor;
        
        private float deltaTime;
        private float fps;

        private Texture2D[] tileTextures;
        private Texture2D white;

        private Level level;

        private TextLabel fpsText;
        private TextLabel tickRateText;
        private TextLabel playerPosText;
        private TextLabel cameraPosText;
        private TextLabel renderedTilesText;
        private TextLabel activeBlockText;
        private TextLabel placingWallsText;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            _tickAccumulator = TimeSpan.Zero;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            tickTargetTime = TimeSpan.FromSeconds(1.0 / (double)GameSettings.Data.TickRate);

            // FPS cap
            _graphics.SynchronizeWithVerticalRetrace = GameSettings.Data.VSync;

            if (GameSettings.Data.MaxFps <= 0)
            {
                IsFixedTimeStep = false;
            }
            else
            {
                IsFixedTimeStep = true;
                TargetElapsedTime = TimeSpan.FromSeconds(1d / (double)GameSettings.Data.MaxFps);
            }
        }

        protected override void Initialize()
        {
            UpdateScreen(false);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("Fonts/Arial");

            white = new Texture2D(GraphicsDevice, 1, 1);
            white.SetData(new Color[] {Color.White});

            viewport = new RenderTarget2D(GraphicsDevice, (int)GameSettings.Data.ViewportSize.X, (int)GameSettings.Data.ViewportSize.Y);

            tileTextures = new Texture2D[Enum.GetNames(typeof(Tile.Block)).Length - 1];
            for (int i=0; i < tileTextures.Length; i++)
            {
                tileTextures[i] = Content.Load<Texture2D>($"Images/{(Tile.Block)i+1}");
            }

            level = new Level("level", tileTextures, white, white);

            fpsText = new TextLabel("", new Vector2(5, 0), 12, _font);
            tickRateText = new TextLabel("", new Vector2(5, 25), 6, _font);
            playerPosText = new TextLabel("", new Vector2(5, 35), 6, _font);
            cameraPosText = new TextLabel("", new Vector2(5, 45), 6, _font);
            renderedTilesText = new TextLabel("", new Vector2(5, 55), 6, _font);
            activeBlockText = new TextLabel("", new Vector2(5, 65), 6, _font);
            placingWallsText = new TextLabel("", new Vector2(5, 75), 6, _font);
        }

        protected override void Update(GameTime gameTime)
        {   
            _tickAccumulator += gameTime.ElapsedGameTime;

            deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds * GameSettings.Data.TickRate;
            fps = 1 / (deltaTime / GameSettings.Data.TickRate);

            InputManager.Update();
            level.Input();

            if (InputManager.IsKeyPressed(Keys.LeftAlt))
            {
                UpdateScreen(!_graphics.IsFullScreen);
            }

            // GAME LOGIC
            while (_tickAccumulator >= tickTargetTime)
            {
                TickUpdate();
                _tickAccumulator -= tickTargetTime;
            }
            // GAME LOGIC

            // TEXT
            fpsText.SetText(((int)fps).ToString());
            tickRateText.SetText($"TickRate: {GameSettings.Data.TickRate}");
            playerPosText.SetText($"playerPosition: {(int)level.player.position.X} , {(int)level.player.position.Y}");
            cameraPosText.SetText($"cameraPosition: {(int)level.camera.position.X} , {(int)level.camera.position.Y}");
            renderedTilesText.SetText($"renderedTiles: {level.tilemap.renderedTiles}");
            activeBlockText.SetText($"activeBlock: {level.tilemap.activeBlock}");
            placingWallsText.SetText($"placingWalls: {level.tilemap.placingWalls}");
            // TEXT

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected void TickUpdate()
        {
            level.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(viewport);
            GraphicsDevice.Clear(new Color(150,255,255));

            // VIEWPORT
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);
            level.Draw(_spriteBatch);
            _spriteBatch.End();
            // VIEWPORT

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(new Color(150, 255, 255));

            // SCREEN
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);
            _spriteBatch.Draw(viewport, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), Color.White); // Scale viewport up to window size
            _spriteBatch.End();
            // SCREEN

            // TEXT
            _spriteBatch.Begin(samplerState: SamplerState.AnisotropicClamp, blendState: BlendState.AlphaBlend);

            fpsText.Draw(_spriteBatch, screenScaleFactor);
            tickRateText.Draw(_spriteBatch, screenScaleFactor);
            playerPosText.Draw(_spriteBatch, screenScaleFactor);
            cameraPosText.Draw(_spriteBatch, screenScaleFactor);
            renderedTilesText.Draw(_spriteBatch, screenScaleFactor);
            activeBlockText.Draw(_spriteBatch, screenScaleFactor);
            placingWallsText.Draw(_spriteBatch, screenScaleFactor);

            _spriteBatch.End();
            // TEXT

            base.Draw(gameTime);
        }

        private void UpdateScreen(bool fullscreen)
        {   
            screenSize = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);

            if (fullscreen)
            {
                _graphics.PreferredBackBufferWidth = (int)screenSize.X;
                _graphics.PreferredBackBufferHeight = (int)screenSize.Y;
            }
            else
            {
                _graphics.PreferredBackBufferWidth = (int)GameSettings.Data.WindowSize.X;
                _graphics.PreferredBackBufferHeight = (int)GameSettings.Data.WindowSize.Y;
            }
            screenScaleFactor = new Vector2(_graphics.PreferredBackBufferWidth / GameSettings.Data.ViewportSize.X, _graphics.PreferredBackBufferHeight / GameSettings.Data.ViewportSize.Y);

            Console.WriteLine(screenScaleFactor);

            InputManager.screenScaleFactor = screenScaleFactor;

            _graphics.IsFullScreen = fullscreen;
            _graphics.ApplyChanges();
        }
    }
}
