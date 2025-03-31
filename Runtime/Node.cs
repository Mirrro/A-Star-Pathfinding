namespace Mirrro.Pathfinding
{
    public class Node
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