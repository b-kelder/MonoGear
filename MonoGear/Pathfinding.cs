using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    class Pathfinding
    {
        UInt16[,] map;

        public Pathfinding()
        {
            var tilemap = MonoGearGame.FindEntitiesWithTag("Tilemap");
            var col = tilemap[0].Collider as TilemapCollider;
            map = col.Tiles;
        }

        public List<Vector2> FindPath(Vector2 from, Vector2 to)
        {
            Point pointFrom = new Point((int)(from.X / 16), (int)(from.Y / 16));
            Point pointTo = new Point((int)(to.X / 16), (int)(to.Y/ 16));

            Node current = null;
            var start = new Node { Location = pointFrom };
            var target = new Node { Location = pointTo };
            var openList = new List<Node>();
            var closedList = new List<Node>();
            int g = 0;

            // start by adding the original position to the open list
            openList.Add(start);

            while (openList.Count > 0)
            {
                // get the tile with the lowest F score
                var lowest = openList.Min(l => l.F);
                current = openList.First(l => l.F == lowest);

                // add the current square to the closed list
                closedList.Add(current);

                // remove it from the open list
                openList.Remove(current);

                // if we added the destination to the closed list, we've found a path
                if (closedList.FirstOrDefault(l => l.Location == target.Location) != null)
                    break;

                var adjacentTiles = GetWalkableAdjacentTiles(current.Location, map);
                g++;

                foreach (var adjacentTile in adjacentTiles)
                {
                    // if this adjacent square is already in the closed list, ignore it
                    if (closedList.FirstOrDefault(l => l.Location == adjacentTile.Location) != null)
                        continue;

                    // if it's not in the open list...
                    if (openList.FirstOrDefault(l => l.Location == adjacentTile.Location) == null)
                    {
                        // compute its score, set the parent
                        adjacentTile.G = g;
                        adjacentTile.H = ComputeHScore(adjacentTile.Location, target.Location);
                        adjacentTile.F = adjacentTile.G + adjacentTile.H;
                        adjacentTile.Parent = current;

                        // and add it to the open list
                        openList.Add(adjacentTile);
                    }
                    else
                    {
                        // test if using the current G score makes the adjacent square's F score
                        // lower, if yes update the parent because it means it's a better path
                        if (g + adjacentTile.H < adjacentTile.F)
                        {
                            adjacentTile.G = g;
                            adjacentTile.F = adjacentTile.G + adjacentTile.H;
                            adjacentTile.Parent = current;
                        }
                    }
                }
            }

            List<Vector2> path = new List<Vector2>();

            while (current!= null)
            {
                path.Add(new Vector2(current.Location.X, current.Location.Y) * 16 + Vector2.One * 8);
                current = current.Parent;
            }

            path.Reverse();

            return path;
        }

        static Node GetNodeIfWalkable(int x, int y, UInt16[,] map, List<Node> list)
        {
            if (x < map.GetLength(0) && x >= 0 && (y) < map.GetLength(1) && y >= 0 && map[x, y] != 1)
            {
                var node = new Node();
                node.Location.X = x;
                node.Location.Y = y;
                list.Add(node);
                return node;
            }
            return null;
        }

        static List<Node> GetWalkableAdjacentTiles(Point location, UInt16[,] map)
        {
            int x = location.X;
            int y = location.Y;

            var proposedLocations = new List<Node>();

            GetNodeIfWalkable(x, y - 1, map, proposedLocations);
            GetNodeIfWalkable(x, y + 1, map, proposedLocations);
            GetNodeIfWalkable(x - 1, y - 1, map, proposedLocations);
            GetNodeIfWalkable(x + 1, y + 1, map, proposedLocations);
            GetNodeIfWalkable(x + 1, y - 1, map, proposedLocations);
            GetNodeIfWalkable(x - 1, y + 1, map, proposedLocations);
            GetNodeIfWalkable(x - 1, y, map, proposedLocations);
            GetNodeIfWalkable(x + 1, y, map, proposedLocations);

            return proposedLocations;
        }

        static int ComputeHScore(Point loc, Point targetLoc)
        {
            int x = loc.X;
            int y = loc.Y;
            int targetX = targetLoc.X;
            int targetY = targetLoc.Y;
            return Math.Abs(targetX - x) + Math.Abs(targetY - y);
        }
    }

    class Node
    {
        public Point Location;
        public int G;
    public int H;
public int F;
        public Node Parent;
    }
}
