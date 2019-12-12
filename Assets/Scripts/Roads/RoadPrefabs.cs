using UnityEngine;
using System.Collections;

public class RoadPrefabs : MonoBehaviour
{
    [SerializeField] private GameObject m_StraightRoad;
    [SerializeField] private GameObject m_CornerRoad;
    [SerializeField] private GameObject m_TRoad;
    [SerializeField] private GameObject m_CrossRoad;

    [SerializeField] private Transform m_Parent;

    public GameObject StraightRoad { get => m_StraightRoad; }
    public GameObject CornerRoad { get => m_CornerRoad; }
    public GameObject TRoad { get => m_TRoad; }
    public GameObject CrossRoad { get => m_CrossRoad; }

    public Transform Parent { get => m_Parent; }
}
