using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TileMap
{
    internal class Entity
    {
        public float gravity = 0.15f;
        public float speed = 0.1f;
        public float maxSpeed = 1f;
        public float jumpForce = 3f;
        public float maxFallSpeed = 6f;

        public int width = 8;
        public int height = 8;

        public Vector2 direction;

        public bool jumping;
        public bool wantToJump;
        public bool canJump;
        public bool doGravity;
        public bool doCollision;

        public Texture2D texture;
        public TileMap tilemap;
        
        public bool onGround;
        public Vector2 position;
        public Vector2 velocity;

        public SpriteEffects spriteOrientation;
        public Vector2 spriteOffset;

        public Entity(Vector2 position, Texture2D texture, TileMap tilemap)
        {
            this.position = position;
            this.texture = texture;
            this.tilemap = tilemap;

            direction = Vector2.Zero;

            doGravity = true;
            doCollision = true;
        }

        public virtual void Input()
        {
            // Override when extending entity if input needed
        }

        public virtual void Animate()
        {
            if (direction.X > 0) spriteOrientation = SpriteEffects.None;
            else if (direction.X < 0) spriteOrientation = SpriteEffects.FlipHorizontally;
        }

        public virtual void Update()
        {
            velocity.X = MathHelper.Lerp(velocity.X, maxSpeed * direction.X, speed);
            if (Math.Abs(velocity.X) < 0.001) velocity.X = 0; // Fixes weird bug when trying to lerp to 0

            if (doGravity) velocity.Y += gravity;
            if (velocity.Y > maxFallSpeed) velocity.Y = maxFallSpeed;

            if (wantToJump && canJump)
            {
                onGround = false;
                velocity.Y = -jumpForce;
                jumping = true;
            }
            else if (!wantToJump && velocity.Y < 0 && jumping) velocity.Y *= 0.75f;

            position += velocity;

            if (doCollision)
            {
                // Floor collision
                onGround = false;
                Vector2 belowTileLeft = tilemap.WorldToTile(position.X + 1, position.Y + height);
                Vector2 belowTileRight = tilemap.WorldToTile(position.X + width - 1, position.Y + height);
                if (tilemap.GetTile(belowTileLeft.X, belowTileLeft.Y).isSolid && velocity.Y >= 0)
                {   
                    velocity.Y = 0;
                    position.Y = belowTileLeft.Y * GameSettings.Data.TileSize - height;
                    onGround = true;
                    jumping = false;
                }
                else if (tilemap.GetTile(belowTileRight.X, belowTileRight.Y).isSolid && velocity.Y >= 0)
                {   
                    velocity.Y = 0;
                    position.Y = belowTileRight.Y * GameSettings.Data.TileSize - height;
                    onGround = true;
                    jumping = false;
                }

                // Ceiling collision
                Vector2 aboveTileLeft = tilemap.WorldToTile(position.X + 1, position.Y);
                Vector2 aboveTileRight = tilemap.WorldToTile(position.X + width - 2, position.Y);
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
            }

            Animate();
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            spriteBatch.Draw(texture, new Rectangle((int)(Math.Floor(position.X - cameraPosition.X) + spriteOffset.X), (int)(Math.Floor(position.Y - cameraPosition.Y) + spriteOffset.Y), width, height), Color.Black);
        }
    }
}
