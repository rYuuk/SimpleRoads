using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridInteraction : MonoBehaviour
{
    [SerializeField] private Camera m_Camera;
    [SerializeField] private GameObject m_Selection;
    [SerializeField] private float m_SelectionMoveSpeed = 10f;
    [SerializeField] private GameObject m_MapGameobject;
    [SerializeField] private Vector2Int m_Size = new Vector2Int(1000, 1000);
    [SerializeField] private int m_ObstacleCount = 50;

    private RoadGenerator _roadGenerator;
    private int _mouseDownPosX = 0;
    private int _mouseDownPosY = 0;

    private void Start()
    {
        _roadGenerator = new RoadGenerator();
        Map.Init(m_Size.x, m_Size.y);
        m_MapGameobject.transform.localScale = new Vector3(m_Size.x / 10, 1, m_Size.y / 10);

        for (int i = 0; i < m_ObstacleCount; i++)
        {
            int x = Random.Range(0, m_Size.x);
            int y = Random.Range(0, m_Size.y);

            Map.SetTileType(x, y, Tile.Type.Structure);
            GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obstacle.transform.position = new Vector3(x - m_Size.x / 2, 0, y - m_Size.y / 2);
            obstacle.GetComponent<Collider>().enabled = false;
        }
    }

    private void LateUpdate()
    {
        Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
            OnRayCastHit(hit);
    }

    private void OnRayCastHit(RaycastHit hit)
    {
        int posX = (int)(hit.point.x);
        int posZ = (int)(hit.point.z);

        Vector3 oldPos = m_Selection.transform.position;
        Vector3 newPos = new Vector3(posX, 0f, posZ);

        m_Selection.transform.position = Vector3.Lerp(oldPos, newPos, m_SelectionMoveSpeed * Time.deltaTime);

        BuildRoads(posX + m_Size.x / 2, posZ + m_Size.y / 2);
    }

    public void BuildRoads(int gridPosX, int gridPosY)
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Start generation
            _roadGenerator.Start();
            _mouseDownPosX = gridPosX;
            _mouseDownPosY = gridPosY;

        }

        if (Input.GetMouseButton(0))
        {
            //Generating
            _roadGenerator.ClearSavedRoads();
            if (_roadGenerator.CanCreateRoad(gridPosX, gridPosY))
            {
                Vector2Int startPos = new Vector2Int(_mouseDownPosX, _mouseDownPosY);
                Vector2Int endPos = new Vector2Int(gridPosX, gridPosY);
                
                List<Vector2Int> path = Map.GetShortestPath(startPos, endPos);
                foreach (Vector2Int road in path)
                    _roadGenerator.Generate(road.x, road.y);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            //End generation
            _roadGenerator.End();
        }

        if (Input.GetMouseButtonDown(1))
        {
            //Cancel generation
            _roadGenerator.Cancel();
        }
    }
}
