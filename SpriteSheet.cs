using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

// Almost identical to TileSet but with a 2 dimensional size parameter

namespace TileMap
{
    internal class SpriteSheet
    {
        private Vector2 size;
        private Vector2 spriteSize;
        private Dictionary<int, Vector2> positions = new Dictionary<int, Vector2>();

        public Texture2D texture;

        public SpriteSheet(Texture2D texture, Vector2 spriteSize)
        {   
            this.texture = texture;
            this.spriteSize = spriteSize;
            size.X = texture.Width / spriteSize.X;
            size.Y = texture.Height / spriteSize.Y;

            int x = 0;
            int y = 0;
            for (int i = 0; i < size.X * size.Y; i++)
            {
                x = i % (int)size.X;

                positions.Add(i, new Vector2(x, y));

                if ((x + 1) / size.X >= 1)
                {
                    y++;
                }
            }
        }

        public Rectangle GetSpriteRect(int spriteIndex)
        {
            return new Rectangle((int)(positions[spriteIndex].X * spriteSize.X), (int)(positions[spriteIndex].Y * spriteSize.Y), (int)spriteSize.X, (int)spriteSize.Y);
        }
    }
}
