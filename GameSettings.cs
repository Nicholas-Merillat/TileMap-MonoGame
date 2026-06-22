using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TileMap
{
    internal class GameSettings
    {
        public static class Data
        {   
            public static Vector2 ScreenSize = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            public static Vector2 WindowSize = new Vector2(1280, 720);
            public static Vector2 ViewportSize = new Vector2(640, 360);

            public static int TileSize = 8;
            public static Vector2 TileMapSize = new Vector2(500, 500);
        }
    }
}
