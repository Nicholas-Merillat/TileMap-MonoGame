using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TileMap
{
    internal class GameSettings
    {
        public static class Data
        {   
            public static int MaxFps = 30; // 0 is uncapped
            
            // Don't edit below here unless your not stupid

            public static int PhysicsFps = 60;

            public static Vector2 WindowSize = new Vector2(1280, 720);
            public static Vector2 ViewportSize = new Vector2(640, 360);

            public static int TileSize = 8;
            public static Vector2 TileMapSize = new Vector2(500, 500);
        }
    }
}
