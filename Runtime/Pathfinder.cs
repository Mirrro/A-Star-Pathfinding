using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mirrro.Pathfinding
{
    public static class Pathfinder
    {
        /// <summary>
        /// Attempts to find a valid path using the A* algorithm.
        /// Returns <c>false</c> if no path is found (e.g., blocked or unreachable).
        /// </summary>
        /// <param name="path">The resulting path as a list of grid positions (from start to destination).</param>
        /// <param name="pathFinderNodes">2D grid representing walkable and non-walkable tiles.</param>
        /// <param name="start">Start position in the grid.</param>
        /// <param name="destination">Target position in the grid.</param>
        /// <returns><c>true</c> if a path was found; otherwise, <c>false</c>.</returns>
        public static bool TryGetPath(out List<Vector2Int> path, PathFinderNode[,] pathFinderNodes, Vector2Int start, Vector2Int destination)
        {
            Node[,] nodes = ConvertToNodes(pathFinderNodes);
            
            if (nodes[start.x, start.y].IsWalkable)
            {
                var openList = new List<Node>();
                var closedList = new HashSet<Node>();
    
                openList.Add(nodes[start.x, start.y]);

                while (openList.Count > 0)
                {
                    var currentNode = GetLowestFCostNode(openList);

                    if (currentNode.X == destination.x && currentNode.Y == destination.y)
                    {
                        path = ConstructPath(currentNode);
                        return true;
                    }
        
                    openList.Remove(currentNode);
                    closedList.Add(currentNode);

                    foreach (var neighbourNode in GetNeighbourNodes(nodes, currentNode))
                    {
                        if (!neighbourNode.IsWalkable || closedList.Contains(neighbourNode))
                        {
                            continue;
                        }
            
                        float tentativeGCost = currentNode.GCost + CalculateMovementCost(currentNode, neighbourNode);

                        if (tentativeGCost < neighbourNode.GCost || !openList.Contains(neighbourNode))
                        {
                            neighbourNode.Parent = currentNode;
                            neighbourNode.GCost = tentativeGCost;
                            neighbourNode.HCost = CalculateHCost(neighbourNode, nodes[destination.x, destination.y]);

                            if (!openList.Contains(neighbourNode))
                            {
                                openList.Add(neighbourNode);
                            }
                        }
                    }
                }
            }
        
            path = new List<Vector2Int>();
            return false;
        }

        private static Node[,] ConvertToNodes(PathFinderNode[,] pathFinderNodes)
        {
            int width = pathFinderNodes.GetLength(0);
            int height = pathFinderNodes.GetLength(1);
            
            Node[,] nodes = new Node[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    nodes[x, y] = new Node(x, y, pathFinderNodes[x, y].IsWalkable);
                }
            }

            return nodes;
        }

        private static Node GetLowestFCostNode(List<Node> nodes)
        {
            Node lowestCostNode = nodes[0];
            for (int i = 1; i < nodes.Count; i++)
            {
                if (nodes[i].FCost < lowestCostNode.FCost)
                {
                    lowestCostNode = nodes[i];
                }
            }
            return lowestCostNode;
        }

        private static float CalculateMovementCost(Node currentNode, Node neighbor) 
            => 1.0f;

        private static float CalculateHCost(Node node, Node goalNode) 
            => Math.Abs(node.X - goalNode.X) + Math.Abs(node.Y - goalNode.Y);
        
        private static List<Node> GetNeighbourNodes(Node[,] nodes, Node node)
        {
            var neighbours = new List<Node>();

            var right = new Vector2Int(node.X + 1, node.Y);
            if (IsInBounds(nodes, right))
            {
                neighbours.Add(nodes[right.x, right.y]);
            }
    
            var left = new Vector2Int(node.X - 1, node.Y);
            if (IsInBounds(nodes, left))
            {
                neighbours.Add(nodes[left.x, left.y]);
            }
    
            var up = new Vector2Int(node.X, node.Y + 1);
            if (IsInBounds(nodes, up))
            {
                neighbours.Add(nodes[up.x, up.y]);
            }
    
            var down = new Vector2Int(node.X, node.Y - 1);
            if (IsInBounds(nodes, down))
            {
                neighbours.Add(nodes[down.x, down.y]);
            }

            return neighbours;
        }

        private static bool IsInBounds(Node[,] nodes, Vector2Int position)
        {
            return position.x >= 0 && position.y >= 0 && position.x < nodes.GetLength(0) && position.y < nodes.GetLength(1);
        }

        private static List<Vector2Int> ConstructPath(Node node)
        {
            var path = new List<Vector2Int>();
            while (node != null)
            {
                path.Add(new Vector2Int(node.X, node.Y));
                node = node.Parent;
            }
            path.Reverse();
            return path;
        }
        
        private class Node
        {
            public int X { get; } 
            public int Y { get; }
            public bool IsWalkable { get; set; }
    
            public Node Parent { get; set; }
            public float GCost { get; set; }
            public float HCost { get; set; }

            public float FCost => GCost + HCost;

            public Node(int x, int y, bool isWalkable)
            {
                X = x;
                Y = y;
                IsWalkable = isWalkable;
            }
        }
    }
}