using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    enum NodeState
    {
        Closed,
        Open,
        untested
    }
    public class PathFinding : WorldEntity
    {
        private int width;
        private int height;
        private TilemapCollider map;
        private Node[,] nodes;
        private Node startNode;
        private Node endNode;

        public PathFinding()
        {
            Tag = "PathFinder";
        }

        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();

            var obj = MonoGearGame.FindEntitiesWithTag("Tilemap");
            if (obj.Count > 0)
            {
                var tilemap = obj[0];
                var collider = tilemap.Collider as TilemapCollider;
                map = collider;
                width = map.Tiles.GetLength(0);
                height = map.Tiles.GetLength(1);
                nodes = new Node[width, height];


                for (int y = 0; y < this.height; y++)
                {
                    for (int x = 0; x < this.width; x++)
                    {
                        nodes[x, y] = new Node(new Vector2(x * collider.TileSize, y * collider.TileSize) + tilemap.Position, map.Tiles[x, y], Vector2.Zero);
                    }
                }
            }
        }

        public List<Vector2> FindPath(Vector2 start, Vector2 destination)
        {
            startNode = new Node(start, 0, destination);
            endNode = nodes[(int)(destination.X / map.TileSize - map.Entity.Position.X), (int)(destination.Y / map.TileSize - map.Entity.Position.Y)];

            InitializeNodes();

            List<Vector2> path = new List<Vector2>();
            bool succes = Search(startNode);

            if (succes)
            {
                Node node = endNode;

                path.Add(destination);

                path.Reverse();
            }

            return path;
        }

        private void InitializeNodes()
        {
            for (int y = 0; y < this.height; y++)
            {
                for (int x = 0; x < this.width; x++)
                {
                    nodes[x, y].isWalkable = map.Tiles[x, y] != 1;
                    nodes[x, y].g = 0;
                    nodes[x, y].h = Node.GetTraversalCost(nodes[x, y].location, endNode.location);
                }
            }
        }

        private bool Search(Node node)
        {
            node.state = NodeState.Closed;
            List<Node> nextNodes = GetAdjacentWalkableNodes(node);

            nextNodes.Sort((node1, node2) => node1.f.CompareTo(node2.f));

            foreach (var nextNode in nextNodes)
            {
                if (Vector2.DistanceSquared(nextNode.location, endNode.location) < 30)
                {
                    endNode = nextNode;
                    return true;
                }
                else
                {
                    if (Search(nextNode))
                        return true;
                }
            }

            return false;
        }

        private List<Node> GetAdjacentWalkableNodes(Node node)
        {
            List<Node> walkableNodes = new List<Node>();
            IEnumerable<Vector2> nextLocations = GetAdjacentLocations(node.location);

            foreach (var location in nextLocations)
            {
                int x = (int)location.X;
                int y = (int)location.Y;

                if (x < 0 || x >= width || y < 0 || y >= height)
                    continue;

                Node currentNode = nodes[x, y];
                if (!currentNode.isWalkable)
                    continue;

                if (currentNode.state == NodeState.Closed)
                    continue;

                if (currentNode.state == NodeState.Closed)
                {
                    float traversalCost = Node.GetTraversalCost(node.location, node.ParentNode.location);
                    float gTemp = node.g + traversalCost;
                    if (gTemp < currentNode.g)
                    {
                        node.ParentNode = node;
                        walkableNodes.Add(currentNode);
                    }
                }
                else
                {
                    node.ParentNode = node;
                    currentNode.state = NodeState.Open;
                    walkableNodes.Add(currentNode);
                }
            }

            return walkableNodes;
        }

        private static IEnumerable<Vector2> GetAdjacentLocations(Vector2 fromLocation)
        {
            return new Vector2[]
            {
                new Vector2(fromLocation.X-1, fromLocation.Y-1),
                new Vector2(fromLocation.X-1, fromLocation.Y  ),
                new Vector2(fromLocation.X-1, fromLocation.Y+1),
                new Vector2(fromLocation.X+1, fromLocation.Y+1),
                new Vector2(fromLocation.X+1, fromLocation.Y  ),
                new Vector2(fromLocation.X+1, fromLocation.Y-1),
                new Vector2(fromLocation.X,   fromLocation.Y-1),
                new Vector2(fromLocation.X,   fromLocation.Y+1),
            };
        }
    }
}
