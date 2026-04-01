using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public Vector2 visibleRange;

        public Vector2 size;
        public Texture2D texture;
        public int[,] grid;

        public TileMap(Vector2 screenSize, Vector2 size, Texture2D texture)
        {
            this.screenSize = screenSize;
            this.size = size;
            this.texture = texture;

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
        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            visibleRange.X = cameraPosition.X / 64 + (screenSize.X / 64);
            visibleRange.Y = cameraPosition.Y / 64 + (screenSize.Y / 64);

            for (int x = (int)cameraPosition.X / 64; x < visibleRange.X; x++)
            {
                for (int y = (int)cameraPosition.Y / 64; y < visibleRange.Y; y++)
                {   
                    if (x >= size.X || y >= size.Y)
                    {
                        break;
                    }
                    else if (grid[x, y] == 1)
                    {
                        spriteBatch.Draw(texture, new Vector2((x * 64) - cameraPosition.X, (y * 64) - cameraPosition.Y), Color.White);
                    }
                }
            }
        }
    }
}
