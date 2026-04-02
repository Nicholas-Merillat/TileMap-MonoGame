using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileMap
{
    internal class TileMap
    {
        const int NW = 1;
        const int N = 2;
        const int NE = 4;
        const int W = 8;
        const int E = 16;
        const int SW = 32;
        const int S = 64;
        const int SE = 128;

        Dictionary<int, int> maskToTile = new Dictionary<int, int>()
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

        private int[,] grid;
        private Vector2 screenSize;
        private Camera camera;

        public int tileSize;
        public Vector2 size;
        public Vector2 visibleRange;
        public TileSet tileset;

        public TileMap(Vector2 screenSize, Vector2 size, int tileSize, Texture2D texture, Camera camera)
        {
            this.screenSize = screenSize;
            this.size = size;
            this.tileSize = tileSize;
            this.camera = camera;

            tileset = new TileSet(texture, tileSize);

            // Create grid and generate
            grid = new int[(int)size.X, (int)size.Y];
            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {   
                    if (y >= 15)
                    {
                        SetTile(x, y, 1);
                    }
                    else
                    {
                        SetTile(x, y, 0);
                    }
                }
            }
        }

        // Gemini was PARTIALLY used to help get autotiling working (although its dumb as fuck and I had to mess with the diagonal tiles to get it working)
        // It was really nice for the dictionary tho lowk
        public int GetMask(int x, int y, int id)
        {   
            // Edge check
            bool Check(int dx, int dy)
            {
                int nx = x + dx;
                int ny = y + dy;

                if (nx < 0 || ny < 0 || nx >= size.X || ny >= size.Y)
                    return false;

                return grid[nx, ny] == id;
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

        public void SetTile(float x, float y, int id)
        {
            grid[(int)x, (int)y] = id;
        }
        public int GetTile(float x, float y)
        {
            return grid[(int)Math.Clamp(x, 0, size.X-1), (int)Math.Clamp(y, 0, size.Y-1)];
        }
        public Vector2 ScreenToTile(float x, float y)
        {
            return new Vector2((int)Math.Floor((x / tileSize) + camera.X / tileSize), (int)Math.Floor((y / tileSize) + camera.Y / tileSize));
        }
        public Vector2 WorldToTile(float x, float y)
        {
            return new Vector2((int)Math.Floor(x / tileSize), (int)Math.Floor(y / tileSize));
        }

        public void Update(MouseState mouseState, int screenScaleFactor)
        {
            Vector2 mousePosition = ScreenToTile(mouseState.X / screenScaleFactor, mouseState.Y / screenScaleFactor);
            if (mousePosition.X >= 0 && mousePosition.Y >= 0 && mousePosition.X < size.X && mousePosition.Y < size.Y)
            {
                if (mouseState.RightButton == ButtonState.Pressed)
                {
                    SetTile(mousePosition.X, mousePosition.Y, 1);
                }
                else if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    SetTile(mousePosition.X, mousePosition.Y, 0);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {   
            // TODO: Move this camera stuff out of draw
            Vector2 cameraTilePosition = (camera.position / new Vector2(tileSize));

            visibleRange.X = cameraTilePosition.X + (screenSize.X / tileSize);
            visibleRange.Y = cameraTilePosition.Y + (screenSize.Y / tileSize);

            // Only render visible tiles (Column major because apparently it's more memory efficient or something)
            for (int y = (int)cameraTilePosition.Y; y < visibleRange.Y; y++)
            {
                for (int x = (int)cameraTilePosition.X; x < visibleRange.X; x++)
                {
                    if (x >= size.X || y >= size.Y || GetTile(x,y) == 0) // Edge detection and skip air
                    {
                        continue;
                    }
                    else if (GetTile(x, y) == 1)
                    {
                        int mask = GetMask(x, y, 1);
                        int bitmask = 0;

                        if (maskToTile.TryGetValue(mask, out int tileIndex))
                            bitmask = tileIndex;
                        else
                            bitmask = 47; // Fallback to last tile in tileset

                        Vector2 tilePos = new Vector2((x * tileSize) - camera.X, (y * tileSize) - camera.Y);
                        spriteBatch.Draw(tileset.texture, new Vector2((int)tilePos.X, (int)tilePos.Y), tileset.GetTileRect(bitmask), Color.White);
                    }
                }
            }
        }
    }
}
