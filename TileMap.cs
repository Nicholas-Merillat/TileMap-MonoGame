using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TileMap
{
    internal class TileMap
    {
        public bool lightingEnabled = true;
        public bool placingWalls = false;

        public Tile.Block activeBlock = Tile.Block.Grass;
        private readonly int blocksLength = Enum.GetNames(typeof(Tile.Block)).Length;

        private const int NW = 1;
        private const int N = 2;
        private const int NE = 4;
        private const int W = 8;
        private const int E = 16;
        private const int SW = 32;
        private const int S = 64;
        private const int SE = 128;

        private readonly Dictionary<int, int> maskToTile = new Dictionary<int, int>()
        {
            { 2, 1 }, { 8, 2 }, { 10, 3 }, { 11, 4 },
            { 16, 5 }, { 18, 6 }, { 22, 7 }, { 24, 8 },
            { 26, 9 }, { 27, 10 }, { 30, 11 }, { 31, 12 },
            { 64, 13 }, { 66, 14 }, { 72, 15 }, { 74, 16 },
            { 75, 17 }, { 80, 18 }, { 82, 19 }, { 86, 20 },
            { 88, 21 }, { 90, 22 }, { 91, 23 }, { 94, 24 },
            { 95, 25 }, { 104, 26 }, { 106, 27 }, { 107, 28 },
            { 120, 29 }, { 122, 30 }, { 123, 31 }, { 126, 32 },
            { 127, 33 }, { 208, 34 }, { 210, 35 }, { 214, 36 },
            { 216, 37 }, { 218, 38 }, { 219, 39 }, { 222, 40 },
            { 223, 41 }, { 248, 42 }, { 250, 43 }, { 251, 44 },
            { 254, 45 }, { 255, 46 }, { 0, 47 }
        };

        private readonly Tile[,] grid;
        private readonly Tile[,] wallGrid;
        private readonly int[,] lightGrid;

        private readonly int tileSize = GameSettings.Data.TileSize;
        private readonly Camera camera;
        private readonly Texture2D cursorTexture;

        private Vector2 mousePosition;

        public TileSet[] tilesets;
        public Vector2 size;
        public Vector2 visibleRange;
        public int renderedTiles;
        public int safeAreaExtension = 12;

        public TileMap(Vector2 size, Texture2D[] tileTextures, Camera camera, Texture2D cursorTexture)
        {
            this.size = size;
            this.camera = camera;
            this.cursorTexture = cursorTexture;

            tilesets = new TileSet[tileTextures.Length];
            for (int i = 0; i < tileTextures.Length; i++)
            {
                tilesets[i] = new TileSet(tileTextures[i], 8);
            }

            // Create grid and lightGrid, then generate
            grid = new Tile[(int)size.X, (int)size.Y];
            wallGrid = new Tile[(int)size.X, (int)size.Y];
            lightGrid = new int[(int)size.X, (int)size.Y];
            GenerateWorld();
        }

        public void SetTile(float x, float y, Tile.Block ID)
        {
            grid[(int)x, (int)y] = new Tile(ID);
        }
        public void SetWallTile(float x, float y, Tile.Block ID)
        {
            wallGrid[(int)x, (int)y] = new Tile(ID);
        }
        public void SetLightTile(float x, float y, int brightness)
        {
            lightGrid[(int)Math.Clamp(x, 0, size.X - 1), (int)Math.Clamp(y, 0, size.Y - 1)] = brightness;
        }
        public Tile GetTile(float x, float y)
        {
            return grid[(int)Math.Clamp(x, 0, size.X - 1), (int)Math.Clamp(y, 0, size.Y - 1)];
        }
        public Tile GetWallTile(float x, float y)
        {
            return wallGrid[(int)Math.Clamp(x, 0, size.X - 1), (int)Math.Clamp(y, 0, size.Y - 1)];
        }
        public int GetLightTile(float x, float y)
        {
            return lightGrid[(int)Math.Clamp(x, 0, size.X - 1), (int)Math.Clamp(y, 0, size.Y - 1)];
        }
        public Vector2 ScreenToTile(float x, float y)
        {
            return new Vector2((int)Math.Floor((x / tileSize) + camera.position.X / tileSize), (int)Math.Floor((y / tileSize) + camera.position.Y / tileSize));
        }
        public Vector2 WorldToTile(float x, float y)
        {
            return new Vector2((int)Math.Floor(x / tileSize), (int)Math.Floor(y / tileSize));
        }

        public void GenerateWorld()
        {
            float frequency = 0.5f;

            FastNoiseLite noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noise.SetSeed(578434);

            for (int y = 0; y < size.Y; y++)
            {
                for (int x = 0; x < size.X; x++)
                {   
                    if (y >= 50)
                    {
                        if (noise.GetNoise(x * frequency, y * frequency) < 0.2)
                        {
                            SetTile(x, y, Tile.Block.Grass);
                            SetWallTile(x, y, Tile.Block.Grass);
                        }
                        else
                        {
                            SetTile(x, y, Tile.Block.Air);
                            SetWallTile(x, y, Tile.Block.Air);
                        }
                    }
                    else
                    {
                        SetTile(x, y, Tile.Block.Air);
                        SetWallTile(x, y, Tile.Block.Air);
                    }
                }
            }
        }

        public int GetMask(int x, int y, Tile.Block ID, bool isWall)
        {
            // Edge check
            bool Check(int dx, int dy)
            {
                int nx = x + dx;
                int ny = y + dy;

                if (nx < 0 || ny < 0 || nx >= size.X || ny >= size.Y) // Edge detection
                    return true;

                if (isWall) return GetWallTile(nx, ny).ID != Tile.Block.Air;
                return GetTile(nx, ny).isSolid;
            }

            int mask = 0;

            bool n = Check(0, -1);
            bool e = Check(1, 0);
            bool s = Check(0, 1);
            bool w = Check(-1, 0);

            // Cardinals
            if (n) mask |= N;
            if (e) mask |= E;
            if (s) mask |= S;
            if (w) mask |= W;

            // Diagonals
            if (n && w && Check(-1, -1)) mask |= NW;
            if (n && e && Check(1, -1)) mask |= NE;
            if (s && w && Check(-1, 1)) mask |= SW;
            if (s && e && Check(1, 1)) mask |= SE;

            return mask;
        }

        public void CalculateLighting(int x, int y)
        {   
            // Cancer
            bool Check()
            {   
                bool leftCheck = GetTile(x - 1, y).ID == Tile.Block.Air && GetWallTile(x - 1, y).ID == Tile.Block.Air;
                bool topCheck = GetTile(x, y - 1).ID == Tile.Block.Air && GetWallTile(x, y - 1).ID == Tile.Block.Air;
                bool rightCheck = GetTile(x + 1, y).ID == Tile.Block.Air && GetWallTile(x + 1, y).ID == Tile.Block.Air;
                bool bottomCheck = GetTile(x, y + 1).ID == Tile.Block.Air && GetWallTile(x, y + 1).ID == Tile.Block.Air;

                return leftCheck || topCheck || rightCheck || bottomCheck;
            }

            double decayFactor = 1.5;

            if (GetTile(x, y).ID == Tile.Block.Air && GetWallTile(x, y).ID == Tile.Block.Air)
            {
                SetLightTile(x, y, 255);
                return;
            }

            if (!Check())
            {   
                int topLightTile = GetLightTile(x, y - 1);
                int bottomLightTile = GetLightTile(x, y + 1);
                int leftLightTile = GetLightTile(x - 1, y);
                int rightLightTile = GetLightTile(x + 1, y);

                int maxLight = topLightTile;
                if (bottomLightTile > maxLight) maxLight = bottomLightTile;
                if (leftLightTile > maxLight) maxLight = leftLightTile;
                if (rightLightTile > maxLight) maxLight = rightLightTile;

                SetLightTile(x, y, (int)(maxLight / decayFactor));
            }
            else
            {
                SetLightTile(x, y, 255);
            }
        }

        public void Update(float screenScaleFactor)
        {
            mousePosition = ScreenToTile(InputManager.GetMousePosition().X / screenScaleFactor, InputManager.GetMousePosition().Y / screenScaleFactor);
            if (mousePosition.X >= 0 && mousePosition.Y >= 0 && mousePosition.X < size.X && mousePosition.Y < size.Y)
            {
                if (InputManager.IsMouseButtonHeld(InputManager.MouseButton.Right))
                {   
                    if (placingWalls) SetWallTile(mousePosition.X, mousePosition.Y, activeBlock);
                    else SetTile(mousePosition.X, mousePosition.Y, activeBlock);
                }
                else if (InputManager.IsMouseButtonHeld(InputManager.MouseButton.Left))
                {
                    if (placingWalls) SetWallTile(mousePosition.X, mousePosition.Y, Tile.Block.Air);
                    else SetTile(mousePosition.X, mousePosition.Y, Tile.Block.Air);
                }
            }

            if (InputManager.IsKeyPressed(Keys.Tab))
            {
                activeBlock++;
                if ((int)activeBlock > blocksLength - 1)
                {
                    activeBlock = (Tile.Block)1;
                }
            }

            if (InputManager.IsMouseButtonPressed(InputManager.MouseButton.Middle))
            {
                placingWalls = !placingWalls;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {   
            // TODO: Move this camera stuff out of draw
            Vector2 cameraTilePosition = new Vector2((camera.position.X / tileSize) - safeAreaExtension, (camera.position.Y / tileSize) - safeAreaExtension);

            visibleRange.X = cameraTilePosition.X + (GameSettings.Data.ViewportSize.X / tileSize) + (safeAreaExtension * 2);
            visibleRange.Y = cameraTilePosition.Y + (GameSettings.Data.ViewportSize.Y / tileSize) + (safeAreaExtension * 2);

            // Only render visible tiles (Column major because apparently it's more memory efficient or something)
            renderedTiles = 0;
            for (int y = (int)cameraTilePosition.Y; y < visibleRange.Y; y++)
            {
                for (int x = (int)cameraTilePosition.X; x < visibleRange.X; x++)
                {
                    Tile tile = GetTile(x, y);
                    Tile wallTile = GetWallTile(x, y);

                    if (x < size.X || y < size.Y)
                    {
                        // BITMASKING
                        int mask = GetMask(x, y, tile.ID, false);
                        int bitmask = 0;

                        if (maskToTile.TryGetValue(mask, out int tileIndex))
                            bitmask = tileIndex;
                        else
                            bitmask = 47;

                        int wallMask = GetMask(x, y, wallTile.ID, true);
                        int wallBitmask = 0;

                        if (maskToTile.TryGetValue(wallMask, out int wallTileIndex))
                            wallBitmask = wallTileIndex;
                        else
                            wallBitmask = 47;
                        // BITMASKING

                        // LIGHTING
                        if (lightingEnabled)
                        {
                            CalculateLighting(x, y);
                        }
                        else
                        {
                            SetLightTile(x, y, 255);
                        }
                        Color tileColor = new Color(GetLightTile(x, y), GetLightTile(x, y), GetLightTile(x, y), 255);
                        Color wallTileColor = new Color(GetLightTile(x, y) / 2, GetLightTile(x, y) / 2, GetLightTile(x, y) / 2, 255);
                        // LIGHTING

                        Vector2 tilePos = new Vector2((x * tileSize) - camera.position.X, (y * tileSize) - camera.position.Y);
                        if (wallTile.ID != Tile.Block.Air)
                        {
                            renderedTiles++;
                            spriteBatch.Draw(tilesets[(int)wallTile.ID - 1].texture, new Vector2((float)Math.Floor(tilePos.X), (float)Math.Floor(tilePos.Y)), tilesets[(int)wallTile.ID - 1].GetTileRect(wallBitmask), wallTileColor);
                        }
                        if (tile.ID != Tile.Block.Air)
                        {
                            renderedTiles++;
                            spriteBatch.Draw(tilesets[(int)tile.ID - 1].texture, new Vector2((float)Math.Floor(tilePos.X), (float)Math.Floor(tilePos.Y)), tilesets[(int)tile.ID - 1].GetTileRect(bitmask), tileColor);
                        }
                    }
                }
            }
            spriteBatch.Draw(cursorTexture, new Vector2((float)Math.Floor((mousePosition.X * GameSettings.Data.TileSize) - camera.position.X), (float)Math.Floor((mousePosition.Y * GameSettings.Data.TileSize) - camera.position.Y)), Color.White * 0.5f);
        }
    }
}
