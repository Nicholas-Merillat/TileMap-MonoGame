using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileMap
{
    internal class Tile
    {
        public enum Block
        {
            Air,
            Grass,
            Dirt,
            Stone,
            CobbleStone,
            Clay,
            Wood,
        }

        private static readonly Dictionary<Block, string> tileNames = new Dictionary<Block, string>
        {
            {Block.Air, "Air"},
            {Block.Grass, "Grass"},
            {Block.Dirt, "Dirt"},
            {Block.Stone, "Stone"},
            {Block.CobbleStone, "Cobble Stone"},
            {Block.Clay, "Clay"},
            {Block.Wood, "Wood"},
        };

        private static readonly Dictionary<Block, bool> solidTiles = new Dictionary<Block, bool>
        {
            {Block.Air, false},
            {Block.Grass, true},
            {Block.Dirt, true},
            {Block.Stone, true},
            {Block.CobbleStone, true},
            {Block.Clay, true},
            {Block.Wood, true},
        };

        public Block ID;
        public string name;
        public bool isSolid;
        public bool isLight;

        public Tile(Block ID)
        {
            this.ID = ID;

            name = tileNames[this.ID];
            isSolid = solidTiles[this.ID];
        }
    }
}
