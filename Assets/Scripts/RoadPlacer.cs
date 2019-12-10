using UnityEngine;
using System.Collections;

public class RoadPlacer : MonoBehaviour
{
    public RoadPrefabs m_RoadGenerator;

    private void OnEnable()
    {
        Tiles.OnTileChanged += OnTileChanged;
    }

    private void OnDisable()
    {
        Tiles.OnTileChanged -= OnTileChanged;

    }

    private void OnTileChanged(int x, int y, RoadTile road)
    {
        GameObject oldTile = GameObject.Find(x + "," + y);
        if (oldTile != null)
            Destroy(oldTile);

        PlaceRoad(x, y, road);
    }

    public void PlaceRoad(int x, int y, RoadTile road)
    {
        GameObject prefab = m_RoadGenerator.StraightRoad;
        Quaternion rotation = Quaternion.identity;

        switch (road.roadType)
        {
            case RoadTile.RoadType.Straight:
                prefab = m_RoadGenerator.StraightRoad;
                rotation = Quaternion.Euler(0, 90 * (int)road.direction, 0);
                break;

            case RoadTile.RoadType.Corner:
                prefab = m_RoadGenerator.CornerRoad;
                rotation = Quaternion.Euler(0, 90 * ((int)road.direction - 4), 0);
                break;
            case RoadTile.RoadType.T:
                prefab = m_RoadGenerator.TRoad;
                rotation = Quaternion.Euler(0, 90 * (int)road.direction, 0);
                break;
            case RoadTile.RoadType.Cross:
                prefab = m_RoadGenerator.CrossRoad;
                break;
        }

        GameObject tile = Instantiate(prefab, new Vector3(x - 256, 0.01f, y - 256), rotation, transform);
        tile.name = x + "," + y;
        tile.SetActive(true);
    }
}
