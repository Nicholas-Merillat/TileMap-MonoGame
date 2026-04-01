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
                    if (y >= 8)
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

        public void SetTile(float x, float y, int id)
        {
            grid[(int)x, (int)y] = id;
        }
        public int GetTile(float x, float y)
        {
            return grid[(int)x, (int)y];
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
            Vector2 cameraTilePosition = (camera.position / new Vector2(tileSize));

            visibleRange.X = cameraTilePosition.X + (screenSize.X / tileSize);
            visibleRange.Y = cameraTilePosition.Y + (screenSize.Y / tileSize);

            // Only render visible tiles (Column major because apparently it's more memory efficient or something)
            for (int y = (int)cameraTilePosition.Y; y < visibleRange.Y; y++)
            {
                for (int x = (int)cameraTilePosition.X; x < visibleRange.X; x++)
                {   
                    if (x >= size.X || y >= size.Y) // Edge detection
                    {
                        break;
                    }
                    else if (GetTile(x,y) == 1)
                    {
                        Vector2 tilePos = new Vector2((x * tileSize) - camera.X, (y * tileSize) - camera.Y);
                        spriteBatch.Draw(tileset.texture, tilePos, tileset.GetTileRect(0), Color.White);
                    }
                }
            }
        }
    }
}
