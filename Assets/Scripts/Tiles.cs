using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum Direction
{
    None = -1,
    North,
    East,
    South,
    West,
    NorthEast,
    SouthEast,
    SouthWest,
    NorthWest

}

public static class Tiles
{
    private static Tile[,] _grid;

    public static System.Action<int, int, RoadTile> OnTileChanged;

    /**
     * Both list points to a binary number for whether road exist in
     * North-East-South-West (clockwise) adjacent tile 
    **/
    private static readonly List<int> _tRoad = new List<int>
    {
        13, //North 1101
        14, //East  1110
        7,  //South 0111
        11  //West  1011
    };

    private static readonly List<int> _cornerRoad = new List<int>
    {
        12, //NorthEast 1100
        6,  //SouthEast 0110
        3,  //SouthWest 0100
        9   //NorthWest 1001
    };

    private static readonly List<int> _straightRoad = new List<int>
    {
        8,  //North 1000
        4,  //East  0100
        2,  //South 0010
        1,  //West  0001
        10, //NorthSouth 1010
        5,  //EastWest   0101
    };



    public static int _crossRoad = 15; //NorthEastSouthWest 1111

    public static void Init(int width, int length)
    {
        _grid = new Tile[width, length];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
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

    public static RoadTile CreateRoad(int x, int y)
    {
        CreateNewRoad(x, y);
        CreateNewRoad(x + 1, y, true);
        CreateNewRoad(x, y + 1, true);
        CreateNewRoad(x - 1, y, true);
        CreateNewRoad(x, y - 1, true);
        return (RoadTile)_grid[x, y];
    }

    private static void CreateNewRoad(int x, int y, bool isCheck = false)
    {
        if (isCheck && _grid[x, y].type != Tile.Type.Road)
            return;

        int north = _grid[x, y + 1].type == Tile.Type.Road ? 1 : 0;
        int east = _grid[x + 1, y].type == Tile.Type.Road ? 1 : 0;
        int south = _grid[x, y - 1].type == Tile.Type.Road ? 1 : 0;
        int west = _grid[x - 1, y].type == Tile.Type.Road ? 1 : 0;

        int combination = (north * (int)Mathf.Pow(2, 3))
                        + (east * ((int)Mathf.Pow(2, 2)))
                        + (south * ((int)Mathf.Pow(2, 1)))
                        + (west * ((int)Mathf.Pow(2, 0)));


        int index = _cornerRoad.IndexOf(combination);
        RoadTile.RoadType roadType;

        if (index != -1)
        {
            roadType = RoadTile.RoadType.Corner;
            index += 4;
        }
        else
        {
            index = _tRoad.IndexOf(combination);

            if (index != -1)
            {
                roadType = RoadTile.RoadType.T;
            }
            else if (combination == _crossRoad)
                roadType = RoadTile.RoadType.Cross;
            else
            {
                index = _straightRoad.IndexOf(combination);
                if (index == -1)
                    index = 0;
                roadType = RoadTile.RoadType.Straight;
            }
        }

        //TODO Rotate according to direction

        if (isCheck)
        {
            RoadTile road = (RoadTile)(_grid[x, y]);

            if ((road.roadType != roadType || road.roadType == RoadTile.RoadType.Straight && road.direction != (Direction)index))
            {
                road.roadType = roadType;
                road.direction = (Direction)index;
                OnTileChanged?.Invoke(x, y, (RoadTile)_grid[x, y]);
            }
        }
        else
            _grid[x, y] = new RoadTile((Direction)index, roadType);
    }

}
