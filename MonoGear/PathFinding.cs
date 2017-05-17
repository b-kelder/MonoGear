using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    class PathFinding
    {
        private int width;
        private int height;
        private Node[,] nodes;
        private Node startNode;
        private Node endNode;

        public PathFinding()
        {
            var tilemap = MonoGearGame.FindEntitiesWithTag("Tilemap")[0];
            var collider = tilemap.Collider as TilemapCollider;
            var map = collider.Tiles;
            InitializeNodes(map);
        }

        public List<Vector2> FindPath(Vector2 start, Vector2 destination)
        {
            startNode = new Node(start, 0, destination);
            endNode = new Node(destination, 0, destination);

            List<Vector2> path = new List<Vector2>();
            bool succes = Search(startNode);

            if (succes)
            {
                Node node = endNode;

                while (node.ParentNode != null)
                {
                    path.Add(node.location);
                    node = node.ParentNode;
                }

                path.Reverse();
            }

            return path;
        }

        private void InitializeNodes(UInt16[,] map)
        {
            width = map.GetLength(0);
            height = map.GetLength(1);
            nodes = new Node[this.width, this.height];
            for (int y = 0; y < this.height; y++)
            {
                for (int x = 0; x < this.width; x++)
                {
                    nodes[x, y] = new Node(new Vector2(x,y), map[x, y], endNode.location);
                }
            }
        }

        private bool Search(Node node)
        {
            node.state = "closed";
            List<Node> nextNodes = GetAdjacentWalkableNodes(node);

            nextNodes.Sort((node1, node2) => node1.f.CompareTo(node2.f));

            foreach (var nextNode in nextNodes)
            {
                if (nextNode.location == endNode.location)
                {
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

                if (currentNode.state.Equals("closed"))
                    continue;

                if (currentNode.state.Equals("open"))
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
                    currentNode.state = "open";
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
                new Vector2(fromLocation.X,   fromLocation.Y-1)
            };
        }
    }
}
