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

        private SpriteEffects spriteOrientation;

        public Player(Vector2 position, Texture2D texture, TileMap tilemap) : base(position, texture, tilemap)
        {   
            width = 7;
            height = 15;

            debug = false;
            spritesheet = new SpriteSheet(texture, new Vector2(33,32));
            currentAnim = "idle";
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
                velocity.X = maxSpeed * 3 * direction.X;
                velocity.Y = maxSpeed * 3 * direction.Y;
            }
            // LOOP

            // ANIMATION
            if (direction.X > 0) spriteOrientation = SpriteEffects.None;
            else if (direction.X < 0) spriteOrientation = SpriteEffects.FlipHorizontally;

            if (onGround)
            {
                currentAnim = "idle";
                if (velocity.X != 0)
                {
                    currentAnim = "walk";
                }
            }
            else
            {
                if (velocity.Y <= 0)
                {
                    currentAnim = "jump";
                }
                else
                {
                    currentAnim = "fall";
                }
            }
            // ANIMATION
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            // base.Draw(spriteBatch, cameraPosition);

            spriteBatch.Draw(
                texture, 
                new Vector2((int)Math.Floor(position.X - cameraPosition.X) - width * 2, (int)Math.Floor(position.Y - cameraPosition.Y) - height), 
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
