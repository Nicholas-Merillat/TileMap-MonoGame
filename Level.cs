using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TileMap
{
    internal class Level
    {   
        public string name;
        
        public Camera camera;
        public TileMap tilemap;
        public Player player;

        private List<Entity> entities;

        public Level(string name, Texture2D[] tileTextures, Texture2D playerTexture, Texture2D cursorTexture)
        {
            this.name = name;

            camera = new Camera(Vector2.Zero);
            tilemap = new TileMap(GameSettings.Data.TileMapSize, tileTextures, camera, cursorTexture);
            player = new Player(Vector2.Zero, playerTexture, tilemap);
            
            entities = new List<Entity>() 
            { 
                player,
                new Entity(new Vector2(100, 100), cursorTexture, tilemap),
            };
        }

        public void Update(float deltaTime, float screenScaleFactor)
        {   
            tilemap.Update(screenScaleFactor);

            foreach (var entity in entities)
            {
                entity.Update(deltaTime);
            }

            camera.Update(deltaTime, player);
        }

        public void Draw(SpriteBatch spriteBatch)
        {   
            tilemap.Draw(spriteBatch);

            foreach (var entity in entities)
            {
                entity.Draw(spriteBatch, camera.position);
            }
        }
    }
}
