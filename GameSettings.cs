using Microsoft.Xna.Framework;

namespace TileMap
{
    internal class GameSettings
    {
        public static class Data
        {
            public static Vector2 screenSize = new Vector2(1280, 720);
            public static Vector2 viewportSize = new Vector2(640, 360);
            public static Vector2 tileMapSize = new Vector2(100, 100);
            public static int tileSize = 8;
        }
    }
}
