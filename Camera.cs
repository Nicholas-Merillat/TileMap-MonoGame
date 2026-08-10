using Microsoft.Xna.Framework;

namespace TileMap
{
    internal class Camera
    {
        private Vector2 targetPos;

        public bool smoothPosition = true;
        public float smoothingSpeed = 0.1f;

        public Vector2 position;

        public Camera(Vector2 position)
        {
            this.position = position;
        }

        public void Update(Entity entity)
        {   
            targetPos.X = (int)(entity.position.X + entity.width / 2 - GameSettings.Data.ViewportSize.X / 2);
            targetPos.Y = (int)(entity.position.Y + entity.height / 2 - GameSettings.Data.ViewportSize.Y / 2);

            if (smoothPosition)
            {
                position.X = MathHelper.Lerp(position.X, targetPos.X, smoothingSpeed);
                position.Y = MathHelper.Lerp(position.Y, targetPos.Y, smoothingSpeed);
            }
            else
            {
                position.X = targetPos.X;
                position.Y = targetPos.Y;
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
