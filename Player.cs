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
        private const int Width = 8;
        private const int Height = 16;
        private const float Gravity = 0.1f;
        private const float Speed = 0.1f;
        private const float MaxSpeed = 2.5f;

        private Texture2D texture;
        private TileMap tilemap;

        private int direction;

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
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                direction = -1;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                direction = 1;
            }
            else
            {
                direction = 0;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                velocity.Y = -1;
            }

            velocity.X = MathHelper.Lerp(velocity.X, MaxSpeed * direction, Speed * deltaTime);
            velocity.Y += Gravity * deltaTime;
            
            // Collision Sensors
            Vector2 belowTileLeft = tilemap.WorldToTile(position.X + 1, position.Y + Height);
            Vector2 belowTileRight = tilemap.WorldToTile(position.X + Width - 1, position.Y + Height);
            Vector2 aboveTileLeft = tilemap.WorldToTile(position.X + 1, position.Y - 1);
            Vector2 aboveTileRight = tilemap.WorldToTile(position.X + Width - 1, position.Y - 1);
            Vector2 rightTileTop = tilemap.WorldToTile(position.X + Width, position.Y + 1);
            Vector2 rightTileBottom = tilemap.WorldToTile(position.X + Width, position.Y + Height - 1);
            Vector2 leftTileTop = tilemap.WorldToTile(position.X - 1, position.Y + 1);
            Vector2 leftTileBottom = tilemap.WorldToTile(position.X - 1, position.Y + Height - 1);

            // Floor collision
            if (tilemap.GetTile(belowTileLeft.X, belowTileLeft.Y) != 0 && velocity.Y >= 0)
            {
                position.Y = belowTileLeft.Y * 8 - Height;
                velocity.Y = 0;
            }
            else if (tilemap.GetTile(belowTileRight.X, belowTileRight.Y) != 0 && velocity.Y >= 0)
            {
                position.Y = belowTileRight.Y * 8 - Height;
                velocity.Y = 0;
            }

            // Ceiling collision
            if (tilemap.GetTile(aboveTileLeft.X, aboveTileLeft.Y) != 0 && velocity.Y <= 0)
            {
                position.Y = (aboveTileLeft.Y + 1) * 8;
                velocity.Y = 0;
            }
            else if (tilemap.GetTile(aboveTileRight.X, aboveTileRight.Y) != 0 && velocity.Y <= 0)
            {
                position.Y = (aboveTileRight.Y + 1) * 8;
                velocity.Y = 0;
            }

            // Right collision
            if (tilemap.GetTile(rightTileTop.X, rightTileTop.Y) != 0 && velocity.X >= 0)
            {
                position.X = rightTileTop.X * 8 - Width;
                velocity.X = 0;
            }
            else if (tilemap.GetTile(rightTileBottom.X, rightTileBottom.Y) != 0 && velocity.X >= 0)
            {
                position.X = rightTileBottom.X * 8 - Width;
                velocity.X = 0;
            }

            // Left collision
            if (tilemap.GetTile(leftTileTop.X, leftTileTop.Y) != 0 && velocity.X <= 0)
            {
                position.X = (leftTileTop.X + 1) * 8;
                velocity.X = 0;
            }
            else if (tilemap.GetTile(leftTileBottom.X, leftTileBottom.Y) != 0 && velocity.X <= 0)
            {
                position.X = (leftTileBottom.X + 1) * 8;
                velocity.X = 0;
            }

            // World border
            if (position.X < 0 && velocity.X <= 0)
            {
                position.X = 0;
            }
            if (position.Y < 0)
            {
                position.Y = 0;
            }

            position.X += velocity.X * deltaTime;
            position.Y += velocity.Y * deltaTime;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, Width, Height), Color.White);
        }
    }
}
