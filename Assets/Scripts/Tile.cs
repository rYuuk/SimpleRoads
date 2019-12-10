using UnityEngine;
using System.Collections;

public class Tile
{
    public enum Type
    {
        Empty,
        Road,
        Structure
    }

    public Type type;

    public Tile()
    {
        type = Type.Empty;
    }

    public Tile(Type type)
    {
        this.type = type;
    }
}
