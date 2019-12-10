using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridInteraction : MonoBehaviour
{
    [SerializeField] private Camera m_Camera;
    [SerializeField] private GameObject m_Selection;
    [SerializeField] private RoadPlacer m_RoadGenerator;


    private void LateUpdate()
    {
        Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
            OnRayCastHit(hit);
    }

    private void OnRayCastHit(RaycastHit hit)
    {
        int posX = Mathf.RoundToInt(hit.point.x);
        int posZ = Mathf.RoundToInt(hit.point.z);

        m_Selection.transform.position = new Vector3(posX, 0.01f, posZ);

        int gridPosX = posX + 256;
        int gridPosY = posZ + 256;

        if (Input.GetMouseButton(0) && Tiles.IsEmpty(gridPosX, gridPosY))
        {
            RoadTile road = Tiles.CreateRoad(gridPosX, gridPosY);
            m_RoadGenerator.PlaceRoad(gridPosX, gridPosY, road);
        }
    }
}
