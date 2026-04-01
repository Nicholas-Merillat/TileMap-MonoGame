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
    internal class TileSet
    {
        private Vector2 size;
        private int tileSize;
        private Dictionary<int, Vector2> positions = new Dictionary<int, Vector2>();

        public Texture2D texture;

        public TileSet(Texture2D texture, int tileSize)
        {   
            this.texture = texture;
            this.tileSize = tileSize;
            size.X = texture.Width / tileSize;
            size.Y = texture.Height / tileSize;

            int x = 0;
            int y = 0;
            for (int i = 0; i < size.X * size.Y; i++)
            {
                x = i % (int)size.X;

                positions.Add(i, new Vector2(x, y));

                if ((x + 1) / size.X >= 1)
                {
                    y++;
                }
            }
        }

        public Rectangle GetTileRect(int tileIndex)
        {
            return new Rectangle((int)positions[tileIndex].X * tileSize, (int)positions[tileIndex].Y * tileSize, tileSize, tileSize);
        }
    }
}
