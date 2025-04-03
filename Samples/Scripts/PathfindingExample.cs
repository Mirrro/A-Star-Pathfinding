using UnityEngine;

namespace Mirrro.Pathfinding
{
    /// <summary>
    /// Example setup for using the <see cref="Pathfinder"/>.
    /// Shows how to convert a custom grid into a grid of <see cref="PathFinderNode"/>
    /// and move a player character along a calculated path.
    /// </summary>
    public class PathfindingExample : MonoBehaviour
    {
        [SerializeField] private TileView tileViewPrefab;
        [SerializeField] private PlayerView playerView;
        [SerializeField] private int width = 10;
        [SerializeField] private int height = 8;

        private MyTileData[,] myGrid;
        private bool isMoving;

        private void Start()
        {
            InitializeGrid();
        }

        // Setup of the grid and hook up click events.
        private void InitializeGrid()
        {
            myGrid = new MyTileData[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var tileView = Instantiate(tileViewPrefab, new Vector3(x, 0, y), Quaternion.identity, transform);
                    tileView.Initialize(new Vector2Int(x, y));
                    tileView.OnLeftClick += OnLeftClick;
                    tileView.OnRightClick += OnRightClick;

                    myGrid[x, y] = new MyTileData(isWalkable: true, tileView);
                }
            }
        }

        // Toggle walkability when a tile is left-clicked.
        private void OnLeftClick(TileView tileView)
        {
            var pos = tileView.GridPosition;
            var gridTile = myGrid[pos.x, pos.y];

            gridTile.IsWalkable = !gridTile.IsWalkable;
            gridTile.View.UpdateColor(gridTile.IsWalkable ? Color.white : Color.black);
        }

        // Find a path from the player to the clicked tile and move the player along it.
        private void OnRightClick(TileView tileView)
        {
            if (isMoving) return;

            Vector2Int targetPos = tileView.GridPosition;
            Vector2Int playerPos = playerView.CurrentGridPosition;

            if (Pathfinder.TryGetPath(out var path, myGrid.ToNodeGrid(), playerPos, targetPos))
            {
                isMoving = true;
                playerView.MoveAlongPath(path, OnPlayerFinishedMoving);
            }
        }

        private void OnPlayerFinishedMoving()
        {
            isMoving = false;
        }
    }
    
    // Handy extension method to convert your tile data into pathfinder nodes.
    public static class MyGridConversionExtensions
    {
        public static PathFinderNode[,] ToNodeGrid(this MyTileData[,] grid)
        {
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            var nodeGrid = new PathFinderNode[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    nodeGrid[x, y] = new PathFinderNode(grid[x, y].IsWalkable);
                }
            }

            return nodeGrid;
        }
    }
    
    // Your custom data container for each tile on the grid.
    public class MyTileData
    {
        public bool IsWalkable;
        public TileView View;

        public MyTileData(bool isWalkable, TileView view)
        {
            IsWalkable = isWalkable;
            View = view;
        }
    }
}