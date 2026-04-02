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
    internal class Camera
    {
        private float cameraSpeed = 2f;

        public Vector2 position;

        public Camera(Vector2 position)
        {
            this.position = position;
        }

        public void Update(float deltaTime, Player player)
        {
            position.X = (player.position.X + player.Width / 2) - 640 / 2;
            position.Y = (player.position.Y + player.Height / 2) - 360 / 2;

            // Boundaries
            if (position.X < 0)
            {
                position.X = 0;
            }
            if (position.Y < 0)
            {
                position.Y = 0;
            }
        }
    }
}
