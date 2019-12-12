using UnityEngine;
using System.Collections;

public class Road
{
    public enum Type
    {
        None,
        Straight,
        Corner,
        T,
        Cross
    }

    public Type type;
    public Direction direction;
    public GameObject gameObject;

    public Road(Direction direction, Type roadType = Type.Straight)
    {
        this.direction = direction;
        this.type = roadType;
    }

    public override bool Equals(object obj)
    {
        Road roadTile = (Road)obj;

        if (type == roadTile.type && direction == roadTile.direction)
            return true;
        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }


    public override string ToString()
    {
        return "\tRoadType: " + type.ToString() + ", direction: " + direction.ToString();
    }
}