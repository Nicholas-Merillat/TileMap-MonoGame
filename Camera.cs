using Microsoft.Xna.Framework;

namespace TileMap
{
    internal class Camera
    {
        private const bool SmoothPosition = true;

        public Vector2 position;

        public Camera(Vector2 position)
        {
            this.position = position;
        }

        public void Update(float deltaTime, Player player)
        {   
            if (SmoothPosition)
            {
                position.X = MathHelper.Lerp(position.X, (int)(player.position.X + player.Width / 2 - GameSettings.Data.viewportSize.X / 2), 0.2f * deltaTime);
                position.Y = MathHelper.Lerp(position.Y, (int)(player.position.Y + player.Height / 2 - GameSettings.Data.viewportSize.Y / 2), 0.2f * deltaTime);
            }
            else
            {
                position.X = (player.position.X + player.Width / 2) - GameSettings.Data.viewportSize.X / 2;
                position.Y = (player.position.Y + player.Height / 2) - GameSettings.Data.viewportSize.Y / 2;
            }

            // Boundaries
            if (position.X < 0)
            {
                position.X = 0;
            }
            else if (position.X > (GameSettings.Data.tileMapSize.X * GameSettings.Data.tileSize) - GameSettings.Data.viewportSize.X)
            {
                position.X = (GameSettings.Data.tileMapSize.X * GameSettings.Data.tileSize) - GameSettings.Data.viewportSize.X;
            }

            if (position.Y < 0)
            {
                position.Y = 0;
            }
            else if (position.Y > (GameSettings.Data.tileMapSize.Y * GameSettings.Data.tileSize) - GameSettings.Data.viewportSize.Y)
            {
                position.Y = (GameSettings.Data.tileMapSize.Y * GameSettings.Data.tileSize) - GameSettings.Data.viewportSize.Y;
            }
        }
    }
}
