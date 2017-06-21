using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonoGear.Engine
{
    /// <summary>
    /// Pathfinding class
    /// </summary>
    public class Pathfinding : WorldEntity
    {
        /// <summary>
        /// Stores a request
        /// </summary>
        struct PathRequest
        {
            public Vector2 from;
            public Vector2 to;
            public Action<List<Vector2>> callback;
        }

        /// <summary>
        /// Instance used by static methods
        /// </summary>
        static Pathfinding instance;
        /// <summary>
        /// List of unreachable targets in case a lot of people want to go to the same unreachable place
        /// </summary>
        static HashSet<Point> unreachableTargets = new HashSet<Point>();

        /// <summary>
        /// Map data
        /// </summary>
        static Tile[] map;
        /// <summary>
        /// Nodes used in A*
        /// </summary>
        static Node[] nodes;
        /// <summary>
        /// Multithreaded queue
        /// </summary>
        static ConcurrentQueue<PathRequest> requests = new ConcurrentQueue<PathRequest>();

        /// <summary>
        /// Task that runs the actual pathfinding
        /// </summary>
        Task pathFindingTask;
        bool allowDequeue;

        public Pathfinding()
        {
            // Check for duplicates
            if(instance != null)
            {
                throw new Exception("Duplicate Pathfinding instances!");
            }
            instance = this;

            // Start request handler
            pathFindingTask = Task.Run(() =>
            {
                while(true)
                {
                    DoNextRequest();
                }
            });

            Tag = "Pathfinding";
        }

        /// <summary>
        /// Called on level load
        /// </summary>
        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();

            UpdateInternalMap();
            allowDequeue = true;
        }

        /// <summary>
        /// Called on level unload
        /// </summary>
        public override void OnLevelUnloaded()
        {
            base.OnLevelUnloaded();

            allowDequeue = false;

            PathRequest pr;
            while(requests.TryDequeue(out pr))
            {
                // empty the queue
            }

            // Reset blacklist for new level
            unreachableTargets.Clear();
        }

        /// <summary>
        /// Updates the internal tilemap and nodes based on the active level
        /// </summary>
        private void UpdateInternalMap()
        {
            var level = MonoGearGame.GetCurrentLevel();
            if(level != null)
            {
                map = level.Tiles;

                nodes = new Node[map.Length];

                // New nodes
                for(int y = 0; y < level.Height; y++)
                {
                    for(int x = 0; x < level.Width; x++)
                    {
                        nodes[x + y * level.Width] = new Node();
                        nodes[x + y * level.Width].Location.X = x;
                        nodes[x + y * level.Width].Location.Y = y;
                    }
                }
            }

        }

        /// <summary>
        /// Handles a pathfinding request
        /// </summary>
        private void DoNextRequest()
        {
            PathRequest currentRequest;
            if(allowDequeue && requests.TryDequeue(out currentRequest))
            {
                try
                {
                    // Try pathfinding and callback
                    currentRequest.callback(FindPathImpl(currentRequest.from, currentRequest.to));
                }
                catch(Exception e)
                {
                    // Log and continue
                    Debug.WriteLine(e.Message + "\r\n" + e.StackTrace);
                }
            }
        }

        /// <summary>
        /// Creates a path request with the given parameters.
        /// </summary>
        /// <param name="from">Start point</param>
        /// <param name="to">End point</param>
        /// <param name="callback">Callback when done</param>
        public static void FindPath(Vector2 from, Vector2 to, Action<List<Vector2>> callback)
        {
            var request = new PathRequest
            {
                from = from,
                to = to,
                callback = callback
            };
            requests.Enqueue(request);
        }

        /// <summary>
        /// Does actual A* pathfinding.
        /// </summary>
        /// <param name="from">Start point</param>
        /// <param name="to">End point</param>
        /// <returns>List of Vector2 or null if no path can be found</returns>
        private static List<Vector2> FindPathImpl(Vector2 from, Vector2 to)
        {
            // Set up start and end nodes
            Node current = null;
            var level = MonoGearGame.GetCurrentLevel();
            var start = nodes[(int)(from.X / level.TileWidth) + (int)(from.Y / level.TileHeight) * level.Width];
            var target = nodes[(int)(to.X / level.TileWidth) +  (int)(to.Y / level.TileHeight)   * level.Width];

            // Check for unreachable targets
            if(!unreachableTargets.Contains(target.Location))
            {
                foreach(var node in nodes)
                {
                    node.F = 0;
                    node.G = 0;
                    node.H = 0;
                    node.closed = false;
                    node.Parent = null;
                }

                var openList = new HashSet<Node>();
                int g = 0;

                // start by adding the original position to the open list
                openList.Add(start);

                bool pathFound = false;

                while(openList.Count > 0)
                {
                    // get the tile with the lowest F score
                    var lowest = openList.Min(l => l.F);
                    current = openList.First(l => l.F == lowest);

                    // add the current square to the closed list
                    current.closed = true;

                    // remove it from the open list
                    openList.Remove(current);

                    // if we added the destination to the closed list, we've found a path
                    if(target.closed)
                    {
                        pathFound = true;
                        break;
                    }

                    var adjacentTiles = GetWalkableAdjacentTiles(current.Location, map);
                    g++;

                    foreach(var adjacentTile in adjacentTiles)
                    {
                        // if this adjacent square is already in the closed list, ignore it
                        if(adjacentTile.closed)
                            continue;

                        // if it's not in the open list...
                        if(!openList.Contains(adjacentTile))
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
                            if(g + adjacentTile.H < adjacentTile.F)
                            {
                                adjacentTile.G = g;
                                adjacentTile.F = adjacentTile.G + adjacentTile.H;
                                adjacentTile.Parent = current;
                            }
                        }
                    }
                }

                if(pathFound)
                {
                    List<Vector2> path = new List<Vector2>();

                    // Build path by backtracing
                    while(current != null)
                    {
                        path.Add(new Vector2(current.Location.X, current.Location.Y) * level.TileHeight + Vector2.One * level.TileHeight / 2);
                        current = current.Parent;
                    }

                    // Reverse it for the correct order
                    path.Reverse();

                    return path;
                }
                else
                {
                    // Assume point is not reachable for anyone since the playable area should always be one zone
                    unreachableTargets.Add(target.Location);
                }
            }

            // Couldn't find anything
            return null;
        }

        /// <summary>
        /// Adds a node to the list if it's walkable.
        /// </summary>
        /// <param name="x">Tile x</param>
        /// <param name="y">Tile y</param>
        /// <param name="map">map of tiles</param>
        /// <param name="list">list to add to</param>
        static void GetNodeIfWalkable(int x, int y, Tile[] map, List<Node> list)
        {
            var level = MonoGearGame.GetCurrentLevel();
            if (x < level.Width && x >= 0 && (y) < level.Height && y >= 0 && (map[x + y * level.Width]?.Walkable) == true)
            {
                list.Add(nodes[x + y * level.Width]);
            }
        }

        /// <summary>
        /// Returns a list of walkable adjacent tiles given the location and map.
        /// </summary>
        /// <param name="location">Tile location to get neighbours from</param>
        /// <param name="map">Map to get tiles from</param>
        /// <returns>List of walkable neighbours</returns>
        static List<Node> GetWalkableAdjacentTiles(Point location, Tile[] map)
        {
            int x = location.X;
            int y = location.Y;

            var proposedLocations = new List<Node>();

            GetNodeIfWalkable(x, y - 1, map, proposedLocations);
            GetNodeIfWalkable(x, y + 1, map, proposedLocations);
            // No diagonals for now
            //GetNodeIfWalkable(x - 1, y - 1, map, proposedLocations);
            //GetNodeIfWalkable(x + 1, y + 1, map, proposedLocations);
            //GetNodeIfWalkable(x + 1, y - 1, map, proposedLocations);
            //GetNodeIfWalkable(x - 1, y + 1, map, proposedLocations);
            GetNodeIfWalkable(x - 1, y, map, proposedLocations);
            GetNodeIfWalkable(x + 1, y, map, proposedLocations);

            return proposedLocations;
        }

        /// <summary>
        /// Computes heuristic score
        /// </summary>
        /// <param name="loc">Tile location</param>
        /// <param name="targetLoc">Target tile location</param>
        /// <returns>H score</returns>
        static int ComputeHScore(Point loc, Point targetLoc)
        {
            int x = loc.X;
            int y = loc.Y;
            int targetX = targetLoc.X;
            int targetY = targetLoc.Y;
            return Math.Abs(targetX - x) + Math.Abs(targetY - y);
        }
    }

    /// <summary>
    /// Used to store pathfinding node data
    /// </summary>
    class Node
    {
        public Point Location;
        public int G;
        public int H;
        public int F;
        public Node Parent;
        public bool closed;
    }
}
