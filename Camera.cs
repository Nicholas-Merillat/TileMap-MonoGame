using Microsoft.Xna.Framework;

namespace TileMap
{
    internal class Camera
    {
        private const bool SmoothPosition = true;
        private const float Speed = 0.1f;

        public Vector2 position;

        public Camera(Vector2 position)
        {
            this.position = position;
        }

        public void Update(float deltaTime, Entity entity)
        {   
            if (SmoothPosition)
            {
                position.X = MathHelper.Lerp(position.X, (int)(entity.position.X + entity.width / 2 - GameSettings.Data.ViewportSize.X / 2), Speed * deltaTime);
                position.Y = MathHelper.Lerp(position.Y, (int)(entity.position.Y + entity.height / 2 - GameSettings.Data.ViewportSize.Y / 2), Speed * deltaTime);
            }
            else
            {
                position.X = (entity.position.X + entity.width / 2) - GameSettings.Data.ViewportSize.X / 2;
                position.Y = (entity.position.Y + entity.height / 2) - GameSettings.Data.ViewportSize.Y / 2;
            }

            // Boundaries
            if (position.X < 0)
            {
                position.X = 0;
            }
            else if (position.X > (GameSettings.Data.TileMapSize.X * GameSettings.Data.TileSize) - GameSettings.Data.ViewportSize.X)
            {
                position.X = (GameSettings.Data.TileMapSize.X * GameSettings.Data.TileSize) - GameSettings.Data.ViewportSize.X;
            }

            if (position.Y < 0)
            {
                position.Y = 0;
            }
            else if (position.Y > (GameSettings.Data.TileMapSize.Y * GameSettings.Data.TileSize) - GameSettings.Data.ViewportSize.Y)
            {
                position.Y = (GameSettings.Data.TileMapSize.Y * GameSettings.Data.TileSize) - GameSettings.Data.ViewportSize.Y;
            }
        }
    }
}
