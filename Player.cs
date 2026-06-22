using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TileMap
{
    internal class Player : Entity
    {
        private const Keys LeftKey = Keys.A;
        private const Keys RightKey = Keys.D;
        private const Keys JumpKey = Keys.Space;

        public Player(Vector2 position, Texture2D texture, TileMap tilemap) : base(position, texture, tilemap)
        {   
            base.width = 7;
            base.height = 15;
        }

        public override void Update(float deltaTime)
        {   
            base.wantToJump = Keyboard.GetState().IsKeyDown(JumpKey);
            
            if (Keyboard.GetState().IsKeyDown(LeftKey)) base.direction = -1;
            else if (Keyboard.GetState().IsKeyDown(RightKey)) base.direction = 1;
            else base.direction = 0;

            base.Update(deltaTime);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            base.Draw(spriteBatch, cameraPosition);
        }
    }
}
