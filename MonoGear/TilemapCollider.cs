using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    public class TilemapCollider : Collider
    {
        public bool[,] Tiles { get; private set; }
        public int TileSize { get; set; }

        public TilemapCollider(WorldEntity entity, bool[,] tiles, int tileSize) : base(entity)
        {
            Tiles = tiles;
            TileSize = tileSize;
            BBSize = new Microsoft.Xna.Framework.Vector2(Tiles.GetLength(0) * TileSize, Tiles.GetLength(1) * TileSize) * 2;
        }

        public override bool Collides(Collider other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Populates the collision array based on the given string.
        /// The string must be integers seperated by ,
        /// </summary>
        /// <param name="str">String to parse</param>
        /// <param name="width">Map width</param>
        /// <param name="height">Map height</param>
        public void MapFromString(string str, int width, int height)
        {
            Tiles = new bool[width, height];
            var data = str.Split(',');
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    Tiles[x, y] = int.Parse(data[x + y * width]) > 0;
                }
            }

            BBSize = new Microsoft.Xna.Framework.Vector2(Tiles.GetLength(0) * TileSize, Tiles.GetLength(1) * TileSize) * 2;
        }
    }
}
