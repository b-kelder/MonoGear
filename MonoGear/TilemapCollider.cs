using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    public class TilemapCollider : Collider
    {
        public UInt16[,] Tiles { get; private set; }
        public int TileSize { get; set; }

        public TilemapCollider(WorldEntity entity, UInt16[,] tiles, int tileSize) : base(entity)
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
            Tiles = new UInt16[width, height];
            var data = str.Split(',');
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    Tiles[x, y] = UInt16.Parse(data[x + y * width]);
                }
            }

            BBSize = new Microsoft.Xna.Framework.Vector2(Tiles.GetLength(0) * TileSize, Tiles.GetLength(1) * TileSize) * 2;
        }

        public int GetTileValue(Vector2 position)
        {
            var xy = (position - Entity.Position) / TileSize;
            if(xy.X <0 || xy.Y <0 || xy.X > Tiles.GetLength(0) || xy.Y > Tiles.GetLength(1))
            {
                return -1;
            }
            return Tiles[(int)xy.X, (int)xy.Y];

        }
    }
}
