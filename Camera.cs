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
        public Vector2 position;

        private int cameraSpeed = 5;

        public Camera(Vector2 position)
        {
            this.position = position;
        }

        public void Update()
        {   

            // Input
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                position.X -= cameraSpeed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                position.X += cameraSpeed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                position.Y -= cameraSpeed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                position.Y += cameraSpeed;
            }

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
