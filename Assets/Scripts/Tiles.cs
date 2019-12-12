using UnityEngine;

public enum Direction
{
    None = -1,
    North,
    East,
    South,
    West,
}

public static class Tiles
{
    public static Tile[,] Grid { get; private set; }

    public static void Init(int width, int length)
    {
        Grid = new Tile[width, length];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
                Grid[i, j] = new Tile();
        }

    }

    public static bool IsEmpty(int x, int y)
    {
        return Grid[x, y].type == Tile.Type.Empty;
    }

    public static void SetTileType(int x, int y, Tile.Type type)
    {
        Grid[x, y].type = type;

    }

    public static Tile.Type GetTileType(int x, int y)
    {
        return Grid[x, y].type;
    }

    public static int GetRoadConfig(int x, int y)
    {
        int north = Grid[x, y + 1].type == Tile.Type.Road ? 1 : 0;
        int east = Grid[x + 1, y].type == Tile.Type.Road ? 1 : 0;
        int south = Grid[x, y - 1].type == Tile.Type.Road ? 1 : 0;
        int west = Grid[x - 1, y].type == Tile.Type.Road ? 1 : 0;

        return (north * (int)Mathf.Pow(2, 3))
                + (east * ((int)Mathf.Pow(2, 2)))
                + (south * ((int)Mathf.Pow(2, 1)))
                + (west * ((int)Mathf.Pow(2, 0)));
    }
}
