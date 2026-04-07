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
        public bool placingWalls = false;
        public int activeBlock = 1;
        public enum Blocks {
            Air,
            Grass,
            Dirt,
            Stone,
            CobbleStone,
            Clay,
            Wood,
        }
        private bool layerKeyPressed = false;
        private bool blockKeyPressed = false;
        private readonly int blocksLength = Enum.GetNames(typeof(Blocks)).Length;

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

        private readonly int[,] grid;
        private readonly int[,] wallGrid;
        private readonly int[,] lightGrid;

        private readonly int tileSize = GameSettings.Data.tileSize;
        private readonly Camera camera;

        private Vector2 mousePosition;

        public TileSet[] tilesets;
        public Vector2 size;
        public Vector2 visibleRange;
        public int visibleTiles;

        public TileMap(Vector2 size, Texture2D[] tileTextures, Camera camera)
        {
            this.size = size;
            this.camera = camera;

            tilesets = new TileSet[tileTextures.Length];
            for (int i = 0; i < tileTextures.Length; i++)
            {
                tilesets[i] = new TileSet(tileTextures[i], 8);
            }

            // Create grid and lightGrid, then generate
            grid = new int[(int)size.X, (int)size.Y];
            wallGrid = new int[(int)size.X, (int)size.Y];
            lightGrid = new int[(int)size.X, (int)size.Y];
            GenerateWorld();
        }

        public void SetTile(float x, float y, int id)
        {
            grid[(int)x, (int)y] = id;
        }
        public void SetWallTile(float x, float y, int id)
        {
            wallGrid[(int)x, (int)y] = id;
        }
        public void SetLightTile(float x, float y, int brightness)
        {
            lightGrid[(int)x, (int)y] = brightness;
        }
        public int GetTile(float x, float y)
        {
            return grid[(int)Math.Clamp(x, 0, size.X - 1), (int)Math.Clamp(y, 0, size.Y - 1)];
        }
        public int GetWallTile(float x, float y)
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
            for (int y = 0; y < size.Y; y++)
            {
                for (int x = 0; x < size.X; x++)
                {
                    if (y == 50)
                    {
                        SetTile(x, y, (int)Blocks.Grass);
                        SetWallTile(x, y, (int)Blocks.Grass);
                    }
                    else if (y > 50 && y < 55) {
                        SetTile(x, y, (int)Blocks.Dirt);
                        SetWallTile(x, y, (int)Blocks.Dirt);
                    }
                    else if (y >= 55)
                    {
                        SetTile(x, y, (int)Blocks.Stone);
                        SetWallTile(x, y, (int)Blocks.Stone);
                    }
                    else
                    {
                        SetTile(x, y, (int)Blocks.Air);
                    }
                }
            }
        }

        public int GetMask(int x, int y, int id, bool isWall)
        {
            // Edge check
            bool Check(int dx, int dy)
            {
                int nx = x + dx;
                int ny = y + dy;

                if (nx < 0 || ny < 0 || nx >= size.X || ny >= size.Y) // Edge detection
                    return true;

                if (isWall) return GetWallTile(nx, ny) != (int)Blocks.Air;
                return GetTile(nx, ny) == id;
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

        public void calculateLighting(int x, int y)
        {
            int decayFactor = 24;

            int topLightTile = GetLightTile(x, y - 1);
            int bottomLightTile = GetLightTile(x, y + 1);
            int leftLightTile = GetLightTile(x - 1, y);
            int rightLightTile = GetLightTile(x + 1, y);

            int maxLight = topLightTile;
            if (bottomLightTile > maxLight) maxLight = bottomLightTile;
            if (leftLightTile > maxLight) maxLight = leftLightTile;
            if (rightLightTile > maxLight) maxLight = rightLightTile;

            SetLightTile(x, y, (maxLight - decayFactor));
        }

        public void Update(MouseState mouseState, float screenScaleFactor)
        {
            mousePosition = ScreenToTile(mouseState.X / screenScaleFactor, mouseState.Y / screenScaleFactor);
            if (mousePosition.X >= 0 && mousePosition.Y >= 0 && mousePosition.X < size.X && mousePosition.Y < size.Y)
            {
                if (mouseState.RightButton == ButtonState.Pressed)
                {   
                    if (placingWalls) SetWallTile(mousePosition.X, mousePosition.Y, activeBlock);
                    else SetTile(mousePosition.X, mousePosition.Y, activeBlock);
                }
                else if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    if (placingWalls) SetWallTile(mousePosition.X, mousePosition.Y, (int)Blocks.Air);
                    else SetTile(mousePosition.X, mousePosition.Y, (int)Blocks.Air);
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Tab))
            {
                if (!blockKeyPressed)
                {
                    blockKeyPressed = true;
                    activeBlock++;
                    if (activeBlock > blocksLength - 1)
                    {
                        activeBlock = 1;
                    }
                }
            }
            else
            {
                blockKeyPressed = false;
            }

            if (mouseState.MiddleButton == ButtonState.Pressed)
            {
                if (!layerKeyPressed)
                {
                    layerKeyPressed = true;
                    if (placingWalls) placingWalls = false;
                    else placingWalls = true;
                }
            }
            else
            {
                layerKeyPressed = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {   
            // TODO: Move this camera stuff out of draw
            Vector2 cameraTilePosition = (camera.position / new Vector2(tileSize));

            visibleRange.X = cameraTilePosition.X + (GameSettings.Data.viewportSize.X / tileSize);
            visibleRange.Y = cameraTilePosition.Y + (GameSettings.Data.viewportSize.Y / tileSize);

            // Only render visible tiles (Column major because apparently it's more memory efficient or something)
            visibleTiles = 0;
            for (int y = (int)cameraTilePosition.Y; y < visibleRange.Y; y++)
            {
                for (int x = (int)cameraTilePosition.X; x < visibleRange.X; x++)
                {
                    int tileID = GetTile(x, y);
                    int wallTileID = GetWallTile(x, y);

                    if (x >= size.X || y >= size.Y) // Edge detection
                    {
                        continue;
                    }
                    if (tileID == (int)Blocks.Air && wallTileID == (int)Blocks.Air) {
                        SetLightTile(x, y, 255);
                    }
                    else
                    {
                        // BITMASKING
                        int mask = GetMask(x, y, tileID, false);
                        int bitmask = 0;

                        if (maskToTile.TryGetValue(mask, out int tileIndex))
                            bitmask = tileIndex;
                        else
                            bitmask = 47;

                        int wallMask = GetMask(x, y, wallTileID, true);
                        int wallBitmask = 0;

                        if (maskToTile.TryGetValue(wallMask, out int wallTileIndex))
                            wallBitmask = wallTileIndex;
                        else
                            wallBitmask = 47;
                        // BITMASKING

                        // LIGHTING
                        calculateLighting(x, y);
                        Color tileColor = new Color(GetLightTile(x, y), GetLightTile(x, y), GetLightTile(x, y), 255);
                        Color wallTileColor = new Color(GetLightTile(x, y) / 2, GetLightTile(x, y) / 2, GetLightTile(x, y) / 2, 255);
                        // LIGHTING

                        Vector2 tilePos = new Vector2((x * tileSize) - camera.position.X, (y * tileSize) - camera.position.Y);
                        if (wallTileID != (int)Blocks.Air)
                        {
                            visibleTiles++;
                            spriteBatch.Draw(tilesets[wallTileID - 1].texture, new Vector2((int)Math.Floor(tilePos.X), (int)Math.Floor(tilePos.Y)), tilesets[wallTileID - 1].GetTileRect(wallBitmask), wallTileColor);
                        }
                        if (tileID != (int)Blocks.Air)
                        {
                            visibleTiles++;
                            spriteBatch.Draw(tilesets[tileID - 1].texture, new Vector2((int)Math.Floor(tilePos.X), (int)Math.Floor(tilePos.Y)), tilesets[tileID - 1].GetTileRect(bitmask), tileColor);
                        }
                    }
                }
            }
        }
    }
}
