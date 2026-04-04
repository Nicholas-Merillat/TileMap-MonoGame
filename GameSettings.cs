using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileMap
{
    internal class GameSettings
    {
        public static class Data
        {
            public static Vector2 screenSize = new Vector2(1280, 720);
            public static Vector2 viewportSize = new Vector2(640, 360);
            public static Vector2 tileMapSize = new Vector2(8400, 4200);
            public static int tileSize = 8;
        }
    }
}
