using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGear
{
    class Node
    {
        private Node parentNode;
        public Vector2 location { get; set; }
        public bool isWalkable { get; set; }
        public float g { get; set; }
        public float h { get; set; }
        public NodeState state { get; set; }
        public float f
        {
            get { return g + h; }
        }

        public Node ParentNode
        {
            get { return parentNode; }
            set
            {
                this.parentNode = value;
                this.g = parentNode.g + GetTraversalCost(location, parentNode.location);
            }
        }

        public Node(Vector2 location, int isWalkable, Vector2 end)
        {
            this.location = location;
            this.state = NodeState.untested;
            this.h = GetTraversalCost(this.location, end);
            this.g = 0;

            if (isWalkable == 1)
                this.isWalkable = false;
            else
                 this.isWalkable = true;
        }

        internal static float GetTraversalCost(Vector2 location1, Vector2 location2)
        {
            float deltaX = location2.X - location1.X;
            float deltaY = location2.Y - location1.Y;
            return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }
    }
}
