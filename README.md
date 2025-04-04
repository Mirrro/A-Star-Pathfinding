# Unity Package for A* Pathfinding

## Description

This package provides a `Pathfinder` based on the A* pathfinding algorithm. 
It supports cardinal movement on 2D grids with walkable and non-walkable tiles.

https://github.com/user-attachments/assets/8f3d9064-679c-459b-a24b-83103cc4d920

## Installation
To import the package you can use the Unity Package Manager via git URL:

`https://github.com/Mirrro/a-star-pathfinding`

Or by manually editing your manifest.json file.
```
"com.mirrro.pathfinding": "https://github.com/Mirrro/A-Star-Pathfinding.git"
```

## Code Structure

### `Pathfinder.cs`
This is the main utility class.

Call:
```csharp
bool TryGetPath(out List<Vector2Int> path, PathFinderNode[,] grid, Vector2Int start, Vector2Int end)
```
Returns `true` if a valid path exists, and outputs the result as a list of `Vector2Int` positions representing a grid index.

---

### `PathFinderNode.cs`
Represents a single cell of your grid and can define whether it `IsWalkable` or not

---

## Example Usage

Here’s a snippet to give you the general idea:

```csharp
Vector2Int targetPos = tileView.GridPosition;
Vector2Int playerPos = playerView.CurrentGridPosition;

if (Pathfinder.TryGetPath(out var path, myGrid.ToNodeGrid(), playerPos, targetPos))
{
    playerView.MoveAlongPath(path);
}
```

---

## Grid Conversion via Extension

You’ll have to convert your own grid data into a grid of PathFinderNode[,] that `Pathfinder` can use.  
I recommend to setup some extension or service for that:

```csharp
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
```
---
This could also be cool, because you won't have to store a IsWalkable flag inside your tiles. Maybe each tile knows what units are on it, terrain type, etc.
The convertion can take care of interpreting your tiles and create different PathFinderNodes for different types of movement.

For example, a ground unit could be blocked by other enemies standing on a tile.

```csharp
public static PathFinderNode[,] ToNodeGridForGroundUnits(this MyTileData[,] grid)
{
    int width = grid.GetLength(0);
    int height = grid.GetLength(1);
    var nodeGrid = new PathFinderNode[width, height];

    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            // Occupied tiles are non-walkable
            bool isOccupied = grid[x, y].EnemyCount > 0;
            nodeGrid[x, y] = new PathFinderNode(!isOccupied);
        }
    }

    return nodeGrid;
}
```

While an air unit could ignore that entirely and just fly over:

```csharp
public static PathFinderNode[,] ToNodeGridForAirborneUnits(this MyTileData[,] grid)
{
    int width = grid.GetLength(0);
    int height = grid.GetLength(1);
    var nodeGrid = new PathFinderNode[width, height];

    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            // Any tile is walkable
            nodeGrid[x, y] = new PathFinderNode(true);
        }
    }

    return nodeGrid;
}
```
 
