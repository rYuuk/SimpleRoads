using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    None = -1,
    North,
    East,
    South,
    West,
}


public static class Map
{
    private static Tile[,] _grid;
    private static int _width;
    private static int _length;

    private static Vector2Int _size;
    private static readonly Vector2Int[] _direction =
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0)
    };
    public static Vector2Int Size { get => _size; }
    public static void Init(int width, int length)
    {
        _width = width;
        _length = length;
        _size = new Vector2Int(_width, _length);

        _grid = new Tile[_width, _length];

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _length; j++)
                _grid[i, j] = new Tile();
        }
    }

    public static bool IsEmpty(int x, int y)
    {
        return _grid[x, y].type == Tile.Type.Empty;
    }

    public static void SetTileType(int x, int y, Tile.Type type)
    {
        _grid[x, y].type = type;

    }

    public static Tile.Type GetTileType(int x, int y)
    {
        return _grid[x, y].type;
    }

    public static Vector2Int GetDirection(int index)
    {
        return _direction[index];
    }

    public static int GetRoadConfig(int x, int y)
    {
        int config = 0;

        for (int i = 0; i < 4; i++)
        {
            int isRoad = (GetTileType(x + _direction[i].x, y + _direction[i].y) == Tile.Type.Road) ? 1 : 0;
            config += (isRoad * (int)Mathf.Pow(2, 3 - i));

        }
        return config;
    }

    //Bread First Search Algorithm
    public static List<Vector2Int> GetShortestPath(Vector2Int start, Vector2Int end)
    {
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();

        frontier.Enqueue(start);
        cameFrom[start] = start;

        Vector2Int current = Vector2Int.zero;

        while (frontier.Count > 0)
        {
            current = frontier.Dequeue();

            if (current == end)
                break;

            for (int i = 0; i < 4; i++) //Loop through all neighours North-East-West-South
            {
                Vector2Int next = new Vector2Int(current.x + _direction[i].x, current.y + _direction[i].y);

                if (next.x < 0 || next.x >= _width)
                    continue;
                if (next.y < 0 || next.y >= _length)
                    continue;

                if (GetTileType(next.x, next.y) == Tile.Type.Empty || GetTileType(next.x, next.y) == Tile.Type.Road)
                {
                    if (!cameFrom.ContainsKey(next))
                    {
                        frontier.Enqueue(next);
                        cameFrom.Add(next, current);
                    }
                }
            }
        }

        List<Vector2Int> path = new List<Vector2Int>();
        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Add(start);
        path.Reverse();
        return path;
    }
}
