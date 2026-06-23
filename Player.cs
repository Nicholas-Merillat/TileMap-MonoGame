using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TileMap
{
    internal class Player : Entity
    {
        private bool debug = false;

        public Player(Vector2 position, Texture2D texture, TileMap tilemap) : base(position, texture, tilemap)
        {   
            base.width = 7;
            base.height = 15;
        }

        public override void Update(float deltaTime)
        {   
            // INPUT
            base.wantToJump = InputManager.IsKeyHeld(InputManager.JumpKey);
            
            if (InputManager.IsKeyHeld(InputManager.LeftKey)) base.direction.X = -1;
            else if (InputManager.IsKeyHeld(InputManager.RightKey)) base.direction.X = 1;
            else base.direction.X = 0;

            if (InputManager.IsKeyPressed(InputManager.PlayerDebugKey))
            {
                debug = !debug;
            }
            // INPUT

            // LOOP
            if (!debug)
            {
                base.Update(deltaTime);
            }
            else
            {   
                if (InputManager.IsKeyHeld(InputManager.UpKey)) base.direction.Y = -1;
                else if (InputManager.IsKeyHeld(InputManager.DownKey)) base.direction.Y = 1;
                else base.direction.Y = 0;

                base.velocity = Vector2.Zero;

                base.position.X += (base.MaxSpeed * 3) * base.direction.X * deltaTime;
                base.position.Y += (base.MaxSpeed * 3) * base.direction.Y * deltaTime;
            }
            // LOOP
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            base.Draw(spriteBatch, cameraPosition);
        }
    }
}
