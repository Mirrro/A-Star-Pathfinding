namespace Mirrro.Pathfinding
{
    /// <summary>
    /// Represents a single cell in the grid for the <see cref="Pathfinder"/>.
    /// Used to define whether a tile is walkable or blocked.
    /// </summary>
    public struct PathFinderNode
    {
        public bool IsWalkable;

        public PathFinderNode(bool isWalkable)
        {
            IsWalkable = isWalkable;
        }
    }
}