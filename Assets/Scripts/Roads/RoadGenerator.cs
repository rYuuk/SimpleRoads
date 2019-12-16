using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RoadGenerator
{
    private enum State
    {
        None,
        Start,
        End,
        Cancelled
    }

    /**
     * List points to a binary number for whether road exist in
     * North-East-South-West (clockwise) adjacent tile 
     * Eg:           
     *          |X|
     *             
     *      |X| |R| | |
     *             
     *          | |
     *
     * R = road to be placed
     * X = road already exist
     * For this case, as only North and West have road, 
     * so combination will be : 1001 and it will be west corner road
     * 
    **/
    private readonly int[] _roadConfiguration = new int[]
    {
        //Straight
        8,  //North         1000
        4,  //East          0100
        2,  //South         0010
        1,  //West          0001
        10, //NorthSouth    1010
        5,  //EastWest      0101
        
        //Corner
        12, //North         1100
        6,  //East          0110
        3,  //South         0100
        9,  //West          1001

        //T
        13, //North         1101
        14, //East          1110
        7,  //South         0111
        11, //West          1011

        //Cross
        15 //None           1111
    };


    private State _state;

    private RoadData _roads;
    private RoadData _savedRoads;

    private RoadMeshGeneration _roadPrefabs;
    public RoadGenerator()
    {
        _roads = new RoadData();
        _savedRoads = new RoadData();
        _roadPrefabs = Object.FindObjectOfType<RoadMeshGeneration>();
    }

    public void Start()
    {
        _state = State.Start;
    }

    public void End()
    {
        if (_state != State.Start)
            return;
        _state = State.End;
        _roads.AddRange(_savedRoads.roads);
        _savedRoads.Clear();
    }

    public void Cancel()
    {
        if (_state != State.Start)
            return;
        _state = State.Cancelled;

        ClearSavedRoads();
    }

    public bool CanCreateRoad(int x, int y)
    {
        if (_state != State.Start)
            return false;

        return Map.IsEmpty(x, y);
    }

    public void ClearSavedRoads()
    {
        foreach (var road in _savedRoads.roads)
        {
            Road oldRoad = _roads.Get(road.Key.x, road.Key.y);

            Object.Destroy(road.Value.gameObject);

            if (oldRoad != null)
                oldRoad.gameObject = PlaceRoad(road.Key.x, road.Key.y, oldRoad.type, oldRoad.direction);
            else
                Map.SetTileType(road.Key.x, road.Key.y, Tile.Type.Empty);

        }
        _savedRoads.Clear();
    }

  
    public void Generate(int x, int y)
    {
        if (Map.GetTileType(x,y) == Tile.Type.Road)
            return;

        CreateNewRoad(x, y);
        for (int i = 0; i < 4; i++)
            ModifyRoad(x + Map.GetDirection(i).x, y + Map.GetDirection(i).y);
    }

    private Road CreateNewRoad(int x, int y)
    {
        Road road = CreateRoad(x, y);
        road.gameObject = PlaceRoad(x, y, road.type, road.direction);
        _savedRoads.Add(x, y, road);
        return road;
    }

    private void ModifyRoad(int x, int y)
    {
        if (Map.GetTileType(x, y) != Tile.Type.Road)
            return;

        Road newRoad = CreateRoad(x, y);
        Road oldRoad = _savedRoads.Get(x, y);

        if (oldRoad == null)
            oldRoad = _roads.Get(x, y);

        if (!oldRoad.Equals(newRoad))
        {
            Object.Destroy(oldRoad.gameObject);
            newRoad.gameObject = PlaceRoad(x, y, newRoad.type, newRoad.direction);
            _savedRoads.Add(x, y, newRoad);
        }
    }

    private Road CreateRoad(int x, int y)
    {
        int configuration = Map.GetRoadConfig(x, y);

        Road.Type roadType = Road.Type.None;

        int direction = -1;
        int index = System.Array.IndexOf(_roadConfiguration, configuration);

        switch (index)
        {
            case var _ when index > 0 && index < 6:
                roadType = Road.Type.Straight;
                direction = index;
                break;
            case var _ when index >= 6 && index < 10:
                roadType = Road.Type.Corner;
                direction = index - 6;
                break;
            case var _ when index >= 10 && index < 14:
                roadType = Road.Type.T;
                direction = index - 10;
                break;
            case 14:
                roadType = Road.Type.Cross;
                break;
        }

        return new Road((Direction)direction, roadType);
    }
    private GameObject PlaceRoad(int x, int y, Road.Type roadType, Direction direction)
    {
        return PlaceRoad(x, y, roadType, (int)direction);
    }
    private GameObject PlaceRoad(int x, int y, Road.Type roadType, int direction)
    {
        GameObject prefab = _roadPrefabs.StraightRoad;
        Quaternion rotation = Quaternion.identity;

        switch (roadType)
        {
            case Road.Type.Straight:
                prefab = _roadPrefabs.StraightRoad;
                rotation = Quaternion.Euler(0, 90 * direction, 0);
                break;

            case Road.Type.Corner:
                prefab = _roadPrefabs.CornerRoad;
                rotation = Quaternion.Euler(0, 90 * (direction - 4), 0);
                break;
            case Road.Type.T:
                prefab = _roadPrefabs.TRoad;
                rotation = Quaternion.Euler(0, 90 * direction, 0);
                break;
            case Road.Type.Cross:
                prefab = _roadPrefabs.CrossRoad;
                break;
        }

        GameObject obj = Object.Instantiate(prefab, new Vector3(x - Map.Size.x/2, 0f, y - Map.Size.y/2), rotation, _roadPrefabs.Parent);
        obj.name = x + "," + y + ", " + roadType;
        obj.SetActive(true);
        return obj;
    }
}
