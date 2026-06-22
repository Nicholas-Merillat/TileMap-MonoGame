using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TileMap
{
    internal class Entity
    {
        public float Gravity = 0.15f;
        public float Speed = 0.1f;
        public float MaxSpeed = 1f;
        public float JumpForce = 3f;
        public float MaxFallSpeed = 6f;

        public int width = 8;
        public int height = 8;

        public int direction;
        public bool wantToJump;

        public Texture2D texture;
        public TileMap tilemap;
        
        public bool onGround;
        public Vector2 position;
        // private Vector2 previousPosition;
        public Vector2 velocity;

        public Entity(Vector2 position, Texture2D texture, TileMap tilemap)
        {
            this.position = position;
            this.texture = texture;
            this.tilemap = tilemap;
        }

        public virtual void Update(float deltaTime)
        {
            // previousPosition = position;

            velocity.X = MathHelper.Lerp(velocity.X, MaxSpeed * direction, Speed * deltaTime);
            velocity.Y += Gravity * deltaTime;
            if (velocity.Y > MaxFallSpeed) velocity.Y = MaxFallSpeed;

            // Prepare for the horde of if statements

            // Floor collision
            onGround = false;
            Vector2 belowTileLeft = tilemap.WorldToTile(position.X + 1, position.Y + height);
            Vector2 belowTileRight = tilemap.WorldToTile(position.X + width - 1, position.Y + height);
            if (tilemap.GetTile(belowTileLeft.X, belowTileLeft.Y).isSolid && velocity.Y >= 0)
            {
                position.Y = belowTileLeft.Y * GameSettings.Data.TileSize - height;
                velocity.Y = 0;
                onGround = true;
            }
            else if (tilemap.GetTile(belowTileRight.X, belowTileRight.Y).isSolid && velocity.Y >= 0)
            {
                position.Y = belowTileRight.Y * GameSettings.Data.TileSize - height;
                velocity.Y = 0;
                onGround = true;
            }

            // Ceiling collision
            Vector2 aboveTileLeft = tilemap.WorldToTile(position.X + 1, position.Y);
            Vector2 aboveTileRight = tilemap.WorldToTile(position.X + width - 1, position.Y);
            if (tilemap.GetTile(aboveTileLeft.X, aboveTileLeft.Y).isSolid && velocity.Y <= 0)
            {
                position.Y = (aboveTileLeft.Y + 1) * GameSettings.Data.TileSize;
                velocity.Y = 0;
            }
            else if (tilemap.GetTile(aboveTileRight.X, aboveTileRight.Y).isSolid && velocity.Y <= 0)
            {
                position.Y = (aboveTileRight.Y + 1) * GameSettings.Data.TileSize;
                velocity.Y = 0;
            }

            // Right collision
            Vector2 rightTileTop = tilemap.WorldToTile(position.X + width, position.Y);
            Vector2 rightTileBottom = tilemap.WorldToTile(position.X + width, position.Y + height - 1);
            if (tilemap.GetTile(rightTileTop.X, rightTileTop.Y).isSolid && velocity.X >= 0)
            {
                position.X = rightTileTop.X * GameSettings.Data.TileSize - width;
                velocity.X = 0;
            }
            else if (tilemap.GetTile(rightTileBottom.X, rightTileBottom.Y).isSolid && velocity.X >= 0)
            {
                position.X = rightTileBottom.X * GameSettings.Data.TileSize - width;
                velocity.X = 0;
            }

            // Left collision
            Vector2 leftTileTop = tilemap.WorldToTile(position.X - 1, position.Y);
            Vector2 leftTileBottom = tilemap.WorldToTile(position.X - 1, position.Y + height - 1);
            if (tilemap.GetTile(leftTileTop.X, leftTileTop.Y).isSolid && velocity.X <= 0)
            {
                position.X = (leftTileTop.X + 1) * GameSettings.Data.TileSize;
                velocity.X = 0;
            }
            else if (tilemap.GetTile(leftTileBottom.X, leftTileBottom.Y).isSolid && velocity.X <= 0)
            {
                position.X = (leftTileBottom.X + 1) * GameSettings.Data.TileSize;
                velocity.X = 0;
            }

            // World border
            if (position.X - 1 < 0 && velocity.X <= 0)
            {
                position.X = 0;
                velocity.X = 0;
            }
            if (position.Y < 0 && velocity.Y <= 0)
            {
                position.Y = 0;
                velocity.Y = 0;
            }
            if (position.X + width > tilemap.size.X * GameSettings.Data.TileSize && velocity.X >= 0)
            {
                position.X = tilemap.size.X * GameSettings.Data.TileSize - width;
                velocity.X = 0;
            }
            if (position.Y + height + 1 > tilemap.size.Y * GameSettings.Data.TileSize && velocity.Y >= 0)
            {
                position.Y = tilemap.size.Y * GameSettings.Data.TileSize - height;
                velocity.Y = 0;
                onGround = true;
            }

            if (wantToJump && onGround)
            {
                onGround = false;
                velocity.Y = -JumpForce;
            }
            else if (!wantToJump && velocity.Y < 0) velocity.Y *= 0.75f;

            position.X += velocity.X * deltaTime;
            position.Y += velocity.Y * deltaTime;

            // if (position.Y - previousPosition.Y > GameSettings.Data.TileSize)
            // {
            //     position.Y = previousPosition.Y;
            // }
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            spriteBatch.Draw(texture, new Rectangle((int)(position.X - cameraPosition.X), (int)(position.Y - cameraPosition.Y), width, height), Color.Black);
        }
    }
}
