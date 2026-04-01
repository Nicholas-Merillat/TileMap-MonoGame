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
        public float X;
        public float Y;

        private float cameraSpeed = 2.5f;

        public Camera(Vector2 position)
        {
            this.position = position;
        }

        public void Update(float deltaTime)
        {   
            X = position.X;
            Y = position.Y;

            // Input
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                position.X -= cameraSpeed * deltaTime;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                position.X += cameraSpeed * deltaTime;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                position.Y -= cameraSpeed * deltaTime;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                position.Y += cameraSpeed * deltaTime;
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
