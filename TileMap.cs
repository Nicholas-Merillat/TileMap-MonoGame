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
        private Vector2 screenSize;
        private Camera camera;

        public Vector2 visibleRange;

        public Vector2 size;
        public int tileSize;
        public TileSet tileset;
        public int[,] grid;

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
                    if (y >= 8)
                    {
                        grid[x, y] = 1;
                    }
                    else
                    {
                        grid[x, y] = 0;
                    }
                }
            }
        }

        public void SetTile(int x, int y, int id)
        {
            grid[x, y] = id;
        }
        public int GetTile(int x, int y)
        {
            return grid[x, y];
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
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    grid[(int)mousePosition.X, (int)mousePosition.Y] = 1;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {   
            Vector2 cameraTilePosition = camera.position / new Vector2(tileSize);
            visibleRange.X = cameraTilePosition.X + (screenSize.X / tileSize);
            visibleRange.Y = cameraTilePosition.Y + (screenSize.Y / tileSize);

            // Only render visible tiles
            for (int x = (int)cameraTilePosition.X; x < visibleRange.X; x++)
            {
                for (int y = (int)cameraTilePosition.Y; y < visibleRange.Y; y++)
                {   
                    if (x >= size.X || y >= size.Y) // Edge detection
                    {
                        break;
                    }
                    else if (grid[x, y] == 1)
                    {
                        Vector2 tilePos = new Vector2((x * tileSize) - camera.X, (y * tileSize) - camera.Y);
                        spriteBatch.Draw(tileset.texture, tilePos, tileset.GetTileRect(0), Color.White);
                    }
                }
            }
        }
    }
}
