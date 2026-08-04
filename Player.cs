using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TileMap
{
    internal class Player : Entity
    {
        private bool debug;

        public Player(Vector2 position, Texture2D texture, TileMap tilemap) : base(position, texture, tilemap)
        {   
            width = 7;
            height = 15;

            debug = false;
        }

        public override void Input()
        {   
            // INPUT
            if (InputManager.IsKeyPressed(InputManager.JumpKey)) wantToJump = true;
            else if (InputManager.IsKeyReleased(InputManager.JumpKey)) wantToJump = false;

            direction = new Vector2(
                InputManager.GetInputAxis(InputManager.LeftKey, InputManager.RightKey), 
                InputManager.GetInputAxis(InputManager.UpKey, InputManager.DownKey)
            );

            if (InputManager.IsKeyPressed(InputManager.PlayerDebugKey)) debug = !debug;
            // INPUT
        }

        public override void Update()
        {   
            // LOOP
            doCollision = !debug;
            doGravity = !debug;

            base.Update();

            canJump = onGround && !wantToJump;

            if (debug)
            {   
                velocity.X = (maxSpeed * 3) * direction.X;
                velocity.Y = (maxSpeed * 3) * direction.Y;
            }
            // LOOP
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            base.Draw(spriteBatch, cameraPosition);
        }
    }
}
