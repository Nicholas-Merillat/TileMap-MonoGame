using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TileMap
{
    internal class GameSettings
    {
        public static class Data
        {
            public static Vector2 screenSize = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            public static Vector2 viewportSize = new Vector2(640, 360);
            public static Vector2 tileMapSize = new Vector2(500, 500);
            public static int tileSize = 8;
        }
    }
}
