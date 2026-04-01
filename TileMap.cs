using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileMap
{
    internal class TileMap
    {
        public Vector2 size;
        public int[,] grid;

        public TileMap(Vector2 size)
        {
            this.size = size;
            grid = new int[(int)size.X, (int)size.Y];
        }
    }
}
