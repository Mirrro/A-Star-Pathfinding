using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mirrro.Pathfinding
{
    public class Pathfinder
    {
        public bool TryGetPath(out List<Node> path, Node[,] nodes, Vector2Int start, Vector2Int end)
        {
            if (nodes[start.x, start.y].IsWalkable)
            {
                var openList = new List<Node>();
                var closedList = new HashSet<Node>();
    
                openList.Add(nodes[start.x, start.y]);

                while (openList.Count > 0)
                {
                    var currentNode = GetLowestFCostNode(openList);

                    if (currentNode.X == end.x && currentNode.Y == end.y)
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
            
                        float tentativeGCost = currentNode.GCost + CalculateCost(currentNode, neighbourNode);

                        if (tentativeGCost < neighbourNode.GCost || !openList.Contains(neighbourNode))
                        {
                            neighbourNode.Parent = currentNode;
                            neighbourNode.GCost = tentativeGCost;
                            neighbourNode.HCost = CalculateHCost(neighbourNode, nodes[end.x, end.y]);

                            if (!openList.Contains(neighbourNode))
                            {
                                openList.Add(neighbourNode);
                            }
                        }
                    }
                }
            }
        
            // No path found
            path = new List<Node>();
            return false;
        }

        private Node GetLowestFCostNode(List<Node> nodes)
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

        private float CalculateCost(Node currentNode, Node neighbor) 
            => 1.0f;

        private float CalculateHCost(Node node, Node goalNode) 
            => Math.Abs(node.X - goalNode.X) + Math.Abs(node.Y - goalNode.Y);


        private List<Node> GetNeighbourNodes(Node[,] nodes, Node node)
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

        private bool IsInBounds(Node[,] nodes, Vector2Int position)
        {
            return position.x >= 0 && position.y >= 0 && position.x < nodes.GetLength(0) && position.y < nodes.GetLength(1);
        }

        private List<Node> ConstructPath(Node node)
        {
            var path = new List<Node>();
            while (node != null)
            {
                path.Add(node);
                node = node.Parent;
            }
            path.Reverse();
            return path;
        }
    }
}