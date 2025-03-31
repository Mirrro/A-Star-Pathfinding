using UnityEngine;

namespace Mirrro.Pathfinding
{
    public class ExampleEntry : MonoBehaviour
    {
        [SerializeField] private TileView tileViewPrefab;
        [SerializeField] private PlayerView playerView;
        [SerializeField] private int width = 10;
        [SerializeField] private int height = 8;

        private (Node node, TileView view)[,] tileMap;
        private Pathfinder pathfinder;
        private bool isMoving = false;

        private void Start()
        {
            pathfinder = new Pathfinder();
            InitializeTiles();
        }

        private void InitializeTiles()
        {
            tileMap = new (Node, TileView)[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var tileView = Instantiate(tileViewPrefab, new Vector3(x, 0, y), Quaternion.identity, transform);
                    var node = new Node(x, y, true);
                    tileView.Initialize(new Vector2Int(x, y));
                    tileView.OnLeftClick += OnLeftClick;
                    tileView.OnRightClick += OnRightClick;

                    tileMap[x, y] = (node, tileView);
                }
            }
        }

        private void OnLeftClick(TileView tileView)
        {
            var pos = tileView.GridPosition;
            var (node, view) = tileMap[pos.x, pos.y];

            node.IsWalkable = !node.IsWalkable;
            view.UpdateColor(node.IsWalkable ? Color.white : Color.black);
        }

        private void OnRightClick(TileView tileView)
        {
            if (isMoving) return;

            var pos = tileView.GridPosition;
            var (targetNode, _) = tileMap[pos.x, pos.y];

            Vector2Int playerPos = playerView.CurrentGridPosition;

            var nodeGrid = GetNodeGrid();

            if (pathfinder.TryGetPath(out var path, nodeGrid, playerPos, new Vector2Int(targetNode.X, targetNode.Y)))
            {
                isMoving = true;
                playerView.MoveAlongPath(path, OnPlayerFinishedMoving);
            }
        }

        private void OnPlayerFinishedMoving()
        {
            isMoving = false;
        }

        private Node[,] GetNodeGrid()
        {
            int width = tileMap.GetLength(0);
            int height = tileMap.GetLength(1);
            var nodeGrid = new Node[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    nodeGrid[x, y] = new Node(tileMap[x, y].node.X, tileMap[x, y].node.Y, tileMap[x, y].node.IsWalkable);
                }
            }

            return nodeGrid;
        }
    }
}