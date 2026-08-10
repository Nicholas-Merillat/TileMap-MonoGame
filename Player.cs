using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TileMap
{
    internal class Player : Entity
    {
        private bool debug;

        private SpriteSheet spritesheet;
        private double currentAnimFrame;
        private string currentAnim;

        public Player(Vector2 position, Texture2D texture, TileMap tilemap) : base(position, texture, tilemap)
        {   
            width = 7;
            height = 15;

            debug = false;
            spritesheet = new SpriteSheet(texture, new Vector2(33,32));
            currentAnim = "idle";
            spriteOffset = new Vector2(-width * 2 + 1, -height);
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

        public override void Animate()
        {   
            // LOGIC
            if (onGround)
            {
                currentAnim = "idle";
                if (velocity.X > 0.05 || velocity.X < -0.05)
                {
                    currentAnim = "walk";
                }
            }
            else
            {   
                currentAnim = "fall";
                if (velocity.Y <= 0)
                {
                    currentAnim = "jump";
                }
            }
            // LOGIC

            // ANIMATION
            if (currentAnim == "idle")
            {
                currentAnimFrame += 0.065;
                if (currentAnimFrame < 15 || currentAnimFrame > 19) currentAnimFrame = 15;
            }
            else if (currentAnim == "walk")
            {
                if (velocity.X > 0)
                {
                    currentAnimFrame += MathExtras.Remap(velocity.X, 0, maxSpeed, 0.075, 0.2);
                }
                else if (velocity.X < 0)
                {
                    currentAnimFrame += MathExtras.Remap(velocity.X, 0, -maxSpeed, 0.075, 0.2);
                }
                if (currentAnimFrame < 21 || currentAnimFrame > 27) currentAnimFrame = 21;
            }
            else if (currentAnim == "jump")
            {
                currentAnimFrame = 19;
            }
            else if (currentAnim == "fall")
            {
                currentAnimFrame = 20;
            }
            // ANIMATION

            base.Animate();
        }

        public override void Update()
        {   
            // LOOP
            base.Update();

            canJump = onGround && !wantToJump;
            doCollision = !debug;
            doGravity = !debug;

            if (debug)
            {   
                velocity.X = maxSpeed * 3 * direction.X;
                velocity.Y = maxSpeed * 3 * direction.Y;
            }
            // LOOP
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            // base.Draw(spriteBatch, cameraPosition);

            spriteBatch.Draw(
                texture, 
                new Vector2((int)Math.Floor(position.X - cameraPosition.X) + spriteOffset.X, (int)Math.Floor(position.Y - cameraPosition.Y) + spriteOffset.Y), 
                spritesheet.GetSpriteRect((int)currentAnimFrame), 
                Color.White,
                0,
                Vector2.Zero,
                1,
                spriteOrientation,
                0
            );
        }
    }
}
