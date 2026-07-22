using Microsoft.Xna.Framework;

namespace TileMap
{
    internal class GameSettings
    {
        public static class Data
        {   
            public static bool VSync = false;
            public static int MaxFps = 0; // 0 is uncapped
            
            // Don't edit below here unless you're not stupid

            public static int TickRate = 60;

            public static Vector2 WindowSize = new Vector2(1280, 720);
            public static Vector2 ViewportSize = new Vector2(640, 360);

            public static int TileSize = 8;
            public static int TileTextureSize = 8;
            public static Vector2 TileMapSize = new Vector2(500, 500);
        }
    }
}
