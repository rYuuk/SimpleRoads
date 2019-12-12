using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridInteraction : MonoBehaviour
{
    [SerializeField] private Camera m_Camera;
    [SerializeField] private GameObject m_Selection;
    [SerializeField] private float m_SelectionMoveSpeed = 10f;

    RoadGenerator roadGenerator;

    private void Start()
    {
        roadGenerator = new RoadGenerator();
        Tiles.Init(1000, 1000);
    }

    private void LateUpdate()
    {
        Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
            OnRayCastHit(hit);
    }

    int x = 0;
    int y = 0;

    private void OnRayCastHit(RaycastHit hit)
    {
        int posX = Mathf.RoundToInt(hit.point.x);
        int posZ = Mathf.RoundToInt(hit.point.z);

        Vector3 oldPos = m_Selection.transform.position;
        Vector3 newPos = new Vector3(posX, 0f, posZ);
        m_Selection.transform.position = Vector3.Lerp(oldPos, newPos, m_SelectionMoveSpeed * Time.deltaTime);

        int gridPosX = posX + 256;
        int gridPosY = posZ + 256;

        if (Input.GetMouseButtonDown(0))
        {
            //Start generation
            roadGenerator.Start();
            x = gridPosX;
            y = gridPosY;

        }

        if (Input.GetMouseButton(0) && roadGenerator.CanCreateRoad(gridPosX, gridPosY))
        {
            roadGenerator.ClearSavedRoads();
            Build(gridPosX, gridPosY, x, y);
        }

        if (Input.GetMouseButtonUp(0))
        {
            //End generation
            roadGenerator.End();
        }

        if (Input.GetMouseButtonDown(1))
        {
            //Cancel generation
            roadGenerator.Cancel();
        }

    }

    public void Build(int destX, int destY, int x, int y)
    {
        roadGenerator.Generate(x, y);
        if (x == destX && y == destY)
            return;
        else
        {
            if (destX > destY)
            {
                if (x != destX)
                {
                    if (x < destX)
                        x += 1;
                    else
                        x -= 1;
                }
                else
                {
                    if (y != destY)
                    {
                        if (y < destY)
                            y += 1;
                        else
                            y -= 1;
                    }
                }

            }
            else
            {
                if (y != destY)
                {
                    if (y < destY)
                        y += 1;
                    else
                        y -= 1;
                }
                else
                {
                    if (x != destX)
                    {
                        if (x < destX)
                            x += 1;
                        else
                            x -= 1;
                    }
                }

            }

            Build(destX, destY, x, y);
        }
    }
}
