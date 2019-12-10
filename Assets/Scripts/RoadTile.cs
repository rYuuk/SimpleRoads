using UnityEngine;
using System.Collections;

public class RoadTile : Tile
{
    public enum RoadType
    {
        None,
        Straight,
        Corner,
        T,
        Cross
    }

    public RoadType roadType;
    public Direction direction;

    public RoadTile(Direction direction, RoadType roadType = RoadType.Straight, Type type = Type.Road)
    {
        this.type = type;
        this.direction = direction;
        this.roadType = roadType;
    }

    public override string ToString()
    {
        return "\tRoadType: " + roadType.ToString() + ", direction: " + direction.ToString();
    }
}