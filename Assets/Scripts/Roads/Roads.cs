using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Roads
{
    public Dictionary<Vector2Int, Road> roads { get; }

    public Roads()
    {
        roads = new Dictionary<Vector2Int, Road>();
    }

    public bool Contains(int x, int y)
    {
        if (roads.ContainsKey(new Vector2Int(x, y)))
            return true;
        return false;
    }

    public void Add(int x, int y, Road road)
    {
        Tiles.Grid[x, y].type = Tile.Type.Road;

        Vector2Int key = new Vector2Int(x, y);
        if (!roads.ContainsKey(key))
            roads.Add(key, road);
        else
            roads[key] = road;
    }

    public void AddRange(Dictionary<Vector2Int, Road> roads)
    {
        foreach (var road in roads)
            Add(road.Key.x, road.Key.y, road.Value);
    }

    public Road Get(int x, int y)
    {
        Vector2Int key = new Vector2Int(x, y);
        if (roads.ContainsKey(key))
            return roads[key];

        return null;
    }

    public void Remove(int x, int y)
    {
        Vector2Int key = new Vector2Int(x, y);
        if (roads.ContainsKey(key))
        {
            roads.Remove(key);
            Tiles.Grid[x, y].type = Tile.Type.Empty;
        }
    }

    public void Clear(bool clearAllTiles = false)
    {
        if (clearAllTiles)
        {
            foreach (var road in roads)
            {
                Tiles.Grid[road.Key.x, road.Key.y].type = Tile.Type.Empty;
                Object.Destroy(road.Value.gameObject);
            }
        }

        roads.Clear();
    }

    public override string ToString()
    {
        string log = "Roads: ";
        foreach (var road in roads)
            log += "\n" + road.Key.x + " " + road.Key.y + ", type: " + road.Value.type + ", gameobejct:" + road.Value.gameObject.name;

        return log;
    }
}