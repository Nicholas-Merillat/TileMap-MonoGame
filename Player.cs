using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileMap
{
    internal class Player
    {
        private const float Gravity = 0.15f;
        private const float Speed = 0.05f;
        private const float MaxSpeed = 1.25f;
        private const float JumpForce = 3f;
        private const float MaxFallSpeed = 6f;

        public readonly int Width = 7;
        public readonly int Height = 15;

        private int direction;
        private bool wantToJump;
        private Texture2D texture;
        private TileMap tilemap;
        
        public bool onGround;
        public Vector2 position;
        public Vector2 velocity;

        public Player(Vector2 position, Texture2D texture, TileMap tilemap)
        {
            this.position = position;
            this.texture = texture;
            this.tilemap = tilemap;
        }

        public void Update(float deltaTime)
        {
            wantToJump = Keyboard.GetState().IsKeyDown(Keys.W);
            
            if (Keyboard.GetState().IsKeyDown(Keys.A)) direction = -1;
            else if (Keyboard.GetState().IsKeyDown(Keys.D)) direction = 1;
            else direction = 0;

            velocity.X = MathHelper.Lerp(velocity.X, MaxSpeed * direction, Speed * deltaTime);
            velocity.Y += Gravity * deltaTime;
            if (velocity.Y > MaxFallSpeed) velocity.Y = MaxFallSpeed;

            // Prepare for the horde of if statements

            // Floor collision
            onGround = false;
            Vector2 belowTileLeft = tilemap.WorldToTile(position.X + 1, position.Y + Height);
            Vector2 belowTileRight = tilemap.WorldToTile(position.X + Width - 1, position.Y + Height);
            if (tilemap.GetTile(belowTileLeft.X, belowTileLeft.Y) != 0 && velocity.Y >= 0)
            {
                position.Y = belowTileLeft.Y * GameSettings.Data.tileSize - Height;
                velocity.Y = 0;
                onGround = true;
            }
            else if (tilemap.GetTile(belowTileRight.X, belowTileRight.Y) != 0 && velocity.Y >= 0)
            {
                position.Y = belowTileRight.Y * GameSettings.Data.tileSize - Height;
                velocity.Y = 0;
                onGround = true;
            }

            // Ceiling collision
            Vector2 aboveTileLeft = tilemap.WorldToTile(position.X + 1, position.Y);
            Vector2 aboveTileRight = tilemap.WorldToTile(position.X + Width - 1, position.Y);
            if (tilemap.GetTile(aboveTileLeft.X, aboveTileLeft.Y) != 0 && velocity.Y <= 0)
            {
                position.Y = (aboveTileLeft.Y + 1) * GameSettings.Data.tileSize;
                velocity.Y = 0;
            }
            else if (tilemap.GetTile(aboveTileRight.X, aboveTileRight.Y) != 0 && velocity.Y <= 0)
            {
                position.Y = (aboveTileRight.Y + 1) * GameSettings.Data.tileSize;
                velocity.Y = 0;
            }

            // Right collision
            Vector2 rightTileTop = tilemap.WorldToTile(position.X + Width, position.Y);
            Vector2 rightTileBottom = tilemap.WorldToTile(position.X + Width, position.Y + Height - 1);
            if (tilemap.GetTile(rightTileTop.X, rightTileTop.Y) != 0 && velocity.X >= 0)
            {
                position.X = rightTileTop.X * GameSettings.Data.tileSize - Width;
                velocity.X = 0;
            }
            else if (tilemap.GetTile(rightTileBottom.X, rightTileBottom.Y) != 0 && velocity.X >= 0)
            {
                position.X = rightTileBottom.X * GameSettings.Data.tileSize - Width;
                velocity.X = 0;
            }

            // Left collision
            Vector2 leftTileTop = tilemap.WorldToTile(position.X - 1, position.Y);
            Vector2 leftTileBottom = tilemap.WorldToTile(position.X - 1, position.Y + Height - 1);
            if (tilemap.GetTile(leftTileTop.X, leftTileTop.Y) != 0 && velocity.X <= 0)
            {
                position.X = (leftTileTop.X + 1) * GameSettings.Data.tileSize;
                velocity.X = 0;
            }
            else if (tilemap.GetTile(leftTileBottom.X, leftTileBottom.Y) != 0 && velocity.X <= 0)
            {
                position.X = (leftTileBottom.X + 1) * GameSettings.Data.tileSize;
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
            if (position.X + 1 > (tilemap.size.X - 1) * 8 && velocity.X >= 0)
            {
                position.X = (tilemap.size.X - 1) * GameSettings.Data.tileSize;
                velocity.X = 0;
            }
            if (position.Y + Height + 1 > tilemap.size.Y * 8 && velocity.Y >= 0)
            {
                position.Y = tilemap.size.Y * GameSettings.Data.tileSize - Height;
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
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            spriteBatch.Draw(texture, new Rectangle((int)(position.X - cameraPosition.X), (int)(position.Y - cameraPosition.Y), Width, Height), Color.Black);
        }
    }
}
