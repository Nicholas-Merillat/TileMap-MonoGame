using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileMap
{
    internal class Player
    {
        private const int Width = 16;
        private const int Height = 16;
        private const float Gravity = 0.1f;
        private const float Speed = 0.2f;
        private const float MaxSpeed = 5f;

        private Texture2D texture;
        private TileMap tilemap;

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
            velocity.Y += Gravity * deltaTime;

            if (tilemap.GetTile(position.X / 8, (position.Y + Height) / 8) != 0)
            {
                velocity.Y = 0;
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
