using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TileMap
{
    public class Main : Game
    {   
        private Vector2 screenSize;
        private float screenScaleFactor;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SpriteFont _fontSmall;
        private SpriteFont _fontMedium;

        private RenderTarget2D viewport;
        private float deltaTime;
        private float fps;

        private Texture2D[] tileTextures;
        private Texture2D white;

        private Level level;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // FPS cap
            _graphics.SynchronizeWithVerticalRetrace = false;

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
            _fontSmall = Content.Load<SpriteFont>("Fonts/FontSmall");
            _fontMedium = Content.Load<SpriteFont>("Fonts/FontMedium");

            white = new Texture2D(GraphicsDevice, 1, 1);
            white.SetData(new Color[] {Color.White});

            viewport = new RenderTarget2D(GraphicsDevice, (int)GameSettings.Data.ViewportSize.X, (int)GameSettings.Data.ViewportSize.Y);

            tileTextures = new Texture2D[Enum.GetNames(typeof(Tile.Block)).Length - 1];
            for (int i=0; i < tileTextures.Length; i++)
            {
                tileTextures[i] = Content.Load<Texture2D>($"Images/{(Tile.Block)i+1}");
            }

            level = new Level("level", tileTextures, white, white);
        }

        protected override void Update(GameTime gameTime)
        {
            deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds * GameSettings.Data.PhysicsFps;
            fps = 1 / (deltaTime / GameSettings.Data.PhysicsFps);

            InputManager.Update();

            if (InputManager.IsKeyPressed(Keys.LeftAlt))
            {
                UpdateScreen(!_graphics.IsFullScreen);
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            level.Update(deltaTime, screenScaleFactor);

            base.Update(gameTime);
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
            _spriteBatch.Begin(samplerState: SamplerState.LinearClamp, blendState: BlendState.AlphaBlend);

            _spriteBatch.DrawString(_fontMedium, ((int)fps).ToString(), new Vector2(5, 0), Color.Black, 0, Vector2.Zero, screenScaleFactor / 2, SpriteEffects.None, 0);
            _spriteBatch.DrawString(_fontSmall, $"PhysicsFps: {GameSettings.Data.PhysicsFps}", new Vector2(5, 40 * (screenScaleFactor / 2)), Color.Black, 0, Vector2.Zero, screenScaleFactor / 2, SpriteEffects.None, 0);
            _spriteBatch.DrawString(_fontSmall, $"playerPosition: {(int)level.player.position.X} , {(int)level.player.position.Y}", new Vector2(5, 65 * (screenScaleFactor / 2)), Color.Black, 0, Vector2.Zero, screenScaleFactor / 2, SpriteEffects.None, 0);
            _spriteBatch.DrawString(_fontSmall, $"cameraPosition: {(int)level.camera.position.X} , {(int)level.camera.position.Y}", new Vector2(5, 90 * (screenScaleFactor / 2)), Color.Black, 0, Vector2.Zero, screenScaleFactor / 2, SpriteEffects.None, 0);
            _spriteBatch.DrawString(_fontSmall, $"renderedTiles: {level.tilemap.renderedTiles}", new Vector2(5, 115 * (screenScaleFactor / 2)), Color.Black, 0, Vector2.Zero, screenScaleFactor / 2, SpriteEffects.None, 0);
            _spriteBatch.DrawString(_fontSmall, $"activeBlock: {level.tilemap.activeBlock}", new Vector2(5, 140 * (screenScaleFactor / 2)), Color.Black, 0, Vector2.Zero, screenScaleFactor / 2, SpriteEffects.None, 0);
            _spriteBatch.DrawString(_fontSmall, $"placingWalls: {level.tilemap.placingWalls}", new Vector2(5, 165 * (screenScaleFactor / 2)), Color.Black, 0, Vector2.Zero, screenScaleFactor / 2, SpriteEffects.None, 0);

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
            screenScaleFactor = _graphics.PreferredBackBufferWidth / GameSettings.Data.ViewportSize.X;

            InputManager.screenScaleFactor = screenScaleFactor;

            _graphics.IsFullScreen = fullscreen;
            _graphics.ApplyChanges();
        }
    }
}
