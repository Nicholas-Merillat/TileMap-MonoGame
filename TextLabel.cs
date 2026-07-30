using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TileMap
{
    internal class TextLabel
    {   
        private string text;
        private double size;
        private Vector2 position;
        private SpriteFont font;

        public TextLabel(string text, Vector2 position, double size, SpriteFont font)
        {
            this.text = text;
            this.size = size;
            this.position = position;
            this.font = font;
        }

        public void SetText(string text)
        {
            this.text = text;
        }
        public string GetText()
        {
            return text;
        }
        public void SetSize(double size)
        {
            this.size = size;
        }
        public double GetSize()
        {
            return size;
        }
        public void SetPosition(Vector2 position)
        {
            this.position = position;
        }
        public Vector2 GetPosition()
        {
            return position;
        }
        public void SetFont(SpriteFont font)
        {
            this.font = font;
        }
        public SpriteFont GetFont()
        {
            return font;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 screenScaleFactor)
        {
            spriteBatch.DrawString(
                font, // Font
                text, // Text
                position * screenScaleFactor, // Position
                Color.Black, // Color
                0, // Rotation
                Vector2.Zero, // Origin
                screenScaleFactor * (float)((size / 8) / 10), // Scale
                SpriteEffects.None, // Effects
                0 // layer Depth
            );
        }
    }
}
