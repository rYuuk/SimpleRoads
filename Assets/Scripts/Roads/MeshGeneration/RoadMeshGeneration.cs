using UnityEngine;
using System.Collections.Generic;

public class RoadMeshGeneration : MonoBehaviour
{
    [SerializeField] private Transform m_Parent;

    public GameObject StraightRoad { get => m_StraightRoad.gameObject; }
    public GameObject CornerRoad { get => m_CornerRoad.gameObject; }
    public GameObject TRoad { get => m_TRoad.gameObject; }
    public GameObject CrossRoad { get => m_CrossRoad.gameObject; }

    public Transform Parent { get => m_Parent; }

    [SerializeField] public MeshFilter m_StraightRoad;
    [SerializeField] public MeshFilter m_CornerRoad;
    [SerializeField] public MeshFilter m_TRoad;
    [SerializeField] public MeshFilter m_CrossRoad;
    [SerializeField] public float m_RoadWidth = 0.5f;
    [SerializeField] public float m_PavementWidth = 0.25f;
    [SerializeField] public float m_PavementHeight = 0.25f;
    [SerializeField] public float m_SegmentLength = 0.25f;
    [SerializeField] public float m_RadiusOffset = 0;

    private const float ROAD_SCALE = 1;
    private readonly Vector3 _vertexOffset = new Vector3(-ROAD_SCALE / 2, 0, -ROAD_SCALE / 2);

    [ContextMenu("Striaght Road")]
    public void GenerateStraightRoad()
    {
        Triangulator triangulator = new Triangulator();
        Mesh mesh = new Mesh();
        mesh.subMeshCount = 2;
        bool clockwise = true;

        float roadLeft = ROAD_SCALE / 2 * (1 - m_RoadWidth);
        float roadRight = ROAD_SCALE - roadLeft;

        //Pavement
        Vector2[] leftVertices = new Vector2[]
        {
            new Vector2(roadLeft - m_PavementWidth,0),
            new Vector2(roadLeft - m_PavementWidth,ROAD_SCALE),
        };

        Vector2[] leftRoadVertices = new Vector2[]
        {
            new Vector2(roadLeft, 0),
            new Vector2(roadLeft, ROAD_SCALE),
        };

        Vector2[] rightRoadVertices = new Vector2[]
        {
            new Vector2(roadRight, 0),
            new Vector2(roadRight, ROAD_SCALE),
        };

        Vector2[] rightVertices = new Vector2[]
        {
            new Vector2(roadRight + m_PavementWidth,0),
            new Vector2(roadRight + m_PavementWidth,ROAD_SCALE),
        };


        MeshData pavementMeshData = triangulator.Triangulate(leftVertices, leftRoadVertices, m_PavementHeight, clockwise, _vertexOffset);
        pavementMeshData.Add(triangulator.Triangulate(rightRoadVertices, rightVertices, m_PavementHeight, clockwise, _vertexOffset));

        //Road
        Vector3[] roadVertices = new Vector3[]
        {
            new Vector3(leftRoadVertices[0].x, 0, leftRoadVertices[0].y) + _vertexOffset,
            new Vector3(leftRoadVertices[leftRoadVertices.Length-1].x, 0, leftRoadVertices[leftRoadVertices.Length-1].y) + _vertexOffset,
            new Vector3(rightRoadVertices[rightRoadVertices.Length-1].x, 0, rightRoadVertices[rightRoadVertices.Length-1].y)+ _vertexOffset,
            new Vector3(rightRoadVertices[0].x, 0, rightRoadVertices[0].y) + _vertexOffset,
        };

        MeshData roadMeshData = new MeshData();
        roadMeshData.vertexCount = pavementMeshData.vertexCount;
        roadMeshData.GenerateTriangles(roadVertices, new int[] { 0, 1, 2, 0, 2, 3 });


        List<Vector3> vertices = new List<Vector3>();
        vertices.AddRange(pavementMeshData.vertices);
        vertices.AddRange(roadMeshData.vertices);

        mesh.vertices = vertices.ToArray();
        mesh.SetTriangles(pavementMeshData.triangles.ToArray(), 0);
        mesh.SetTriangles(roadMeshData.triangles.ToArray(), 1);
        mesh.RecalculateNormals();

        m_StraightRoad.mesh = mesh;
    }

    [ContextMenu("Generate corner")]
    public void GenerateCornerRoad()
    {
        float roadLeft = ROAD_SCALE / 2 * (1 - m_RoadWidth);
        float roadRight = ROAD_SCALE - roadLeft;

        var direction = new Vector2(-1, -1);
        var offset = new Vector2(direction.x < 0 ? ROAD_SCALE : 0, direction.y < 0 ? ROAD_SCALE : 0);

        Vector2[] leftVertices = CurveGenerator.GenerateCurved(roadLeft - m_PavementWidth, m_SegmentLength, direction, offset);
        Vector2[] leftRoadVertices = CurveGenerator.GenerateCurved(roadLeft, m_SegmentLength, direction, offset);
        Vector2[] rightRoadVertices = CurveGenerator.GenerateCurved(roadRight, m_SegmentLength, direction, offset);
        Vector2[] rightVertices = CurveGenerator.GenerateCurved(roadRight + m_PavementWidth, m_SegmentLength, direction, offset);

        Mesh mesh = new Mesh();
        mesh.subMeshCount = 2;

        bool clockwise = true;

        if (direction.x != direction.y)
            clockwise = false;

        Triangulator triangulator = new Triangulator();

        MeshData pavementMeshData = triangulator.Triangulate(leftVertices, leftRoadVertices, m_PavementHeight, clockwise, _vertexOffset);
        pavementMeshData.Add(triangulator.Triangulate(rightRoadVertices, rightVertices, m_PavementHeight, clockwise, _vertexOffset));
        MeshData roadMeshData = triangulator.Triangulate(leftRoadVertices, rightRoadVertices, 0, clockwise, _vertexOffset, pavementMeshData.vertexCount);

        List<Vector3> vertices = new List<Vector3>();
        vertices.AddRange(pavementMeshData.vertices);
        vertices.AddRange(roadMeshData.vertices);

        mesh.vertices = vertices.ToArray();
        mesh.SetTriangles(pavementMeshData.triangles.ToArray(), 0);
        mesh.SetTriangles(roadMeshData.triangles.ToArray(), 1);
        mesh.RecalculateNormals();

        m_CornerRoad.mesh = mesh;
    }

    [ContextMenu("Generate T")]
    public void GenerateTRoad()
    {
        Triangulator triangulator = new Triangulator();
        Mesh mesh = new Mesh();
        mesh.subMeshCount = 2;

        float roadLeft = ROAD_SCALE / 2 * (1 - m_RoadWidth);

        //Left curve pavement
        var direction = new Vector2(1, -1);
        var offset = new Vector2(direction.x < 0 ? ROAD_SCALE : 0, direction.y < 0 ? ROAD_SCALE : 0);
        bool clockwise = direction.x == direction.y;

        Vector2[] leftVertices = CurveGenerator.GenerateCurved(roadLeft - m_PavementWidth, m_SegmentLength, direction, offset);
        Vector2[] leftRoadVertices = CurveGenerator.GenerateCurved(roadLeft, m_SegmentLength, direction, offset);
        MeshData pavementMeshData = triangulator.Triangulate(leftVertices, leftRoadVertices, m_PavementHeight, clockwise, _vertexOffset);

        //Right curve pavement
        direction = new Vector2(-1, -1);
        offset = new Vector2(direction.x < 0 ? ROAD_SCALE : 0, direction.y < 0 ? ROAD_SCALE : 0);
        clockwise = direction.x == direction.y;

        Vector2[] rightRoadVertices = CurveGenerator.GenerateCurved(roadLeft - m_PavementWidth, m_SegmentLength, direction, offset);
        Vector2[] rightVertices = CurveGenerator.GenerateCurved(roadLeft, m_SegmentLength, direction, offset);
        pavementMeshData.Add(triangulator.Triangulate(rightRoadVertices, rightVertices, m_PavementHeight, clockwise, _vertexOffset));

        //Bottom pavement
        Vector2[] bottomRoadVertices = new Vector2[]
        {
            new Vector2(0,roadLeft),
            new Vector2(ROAD_SCALE,roadLeft),
        };

        Vector2[] bottomVertices = new Vector2[]
        {
            new Vector2(0,roadLeft- m_PavementWidth),
            new Vector2(ROAD_SCALE,roadLeft - m_PavementWidth),
        };
        pavementMeshData.Add(triangulator.Triangulate(bottomRoadVertices, bottomVertices, m_PavementHeight, clockwise, _vertexOffset));
        pavementMeshData.Add(triangulator.Extend(bottomRoadVertices, bottomVertices, m_PavementHeight, clockwise, _vertexOffset));

        //Road
        Vector3[] roadVertices = new Vector3[]
        {
            new Vector3(bottomRoadVertices[0].x, 0, bottomRoadVertices[0].y) + _vertexOffset,
            new Vector3(leftRoadVertices[leftRoadVertices.Length-1].x, 0, leftRoadVertices[leftRoadVertices.Length-1].y) + _vertexOffset,
            new Vector3(rightVertices[rightVertices.Length-1].x, 0, rightVertices[rightVertices.Length-1].y) + _vertexOffset,
            new Vector3(bottomRoadVertices[1].x, 0, bottomRoadVertices[0].y) + _vertexOffset,
        };

        MeshData roadMeshData = new MeshData();
        roadMeshData.vertexCount = pavementMeshData.vertexCount;
        roadMeshData.GenerateTriangles(roadVertices, new int[] { 0, 1, 2, 0, 2, 3 });
        roadMeshData.Add(triangulator.Fill(leftRoadVertices, rightVertices, 0, !clockwise, _vertexOffset));

        List<Vector3> vertices = new List<Vector3>();
        vertices.AddRange(pavementMeshData.vertices);
        vertices.AddRange(roadMeshData.vertices);

        mesh.vertices = vertices.ToArray();
        mesh.SetTriangles(pavementMeshData.triangles.ToArray(), 0);
        mesh.SetTriangles(roadMeshData.triangles.ToArray(), 1);
        mesh.RecalculateNormals();

        m_TRoad.mesh = mesh;
    }

    [ContextMenu("Generate Cross")]
    public void GenerateCrossRoad()
    {
        Triangulator triangulator = new Triangulator();
        Mesh mesh = new Mesh();
        mesh.subMeshCount = 2;

        float roadLeft = ROAD_SCALE / 2 * (1 - m_RoadWidth);

        //Top left curve
        var direction = new Vector2(1, -1);
        var offset = new Vector2(direction.x < 0 ? ROAD_SCALE : 0, direction.y < 0 ? ROAD_SCALE : 0);
        bool clockwise = direction.x == direction.y;

        Vector2[] topLeftVertices = CurveGenerator.GenerateCurved(roadLeft - m_PavementWidth, m_SegmentLength, direction, offset);
        Vector2[] topLeftRoadVertices = CurveGenerator.GenerateCurved(roadLeft, m_SegmentLength, direction, offset);
        MeshData pavementMeshData = triangulator.Triangulate(topLeftVertices, topLeftRoadVertices, m_PavementHeight, clockwise, _vertexOffset);

        //Top right Curve
        direction = new Vector2(-1, -1);
        offset = new Vector2(direction.x < 0 ? ROAD_SCALE : 0, direction.y < 0 ? ROAD_SCALE : 0);
        clockwise = direction.x == direction.y;

        Vector2[] topRightRoadVertices = CurveGenerator.GenerateCurved(roadLeft - m_PavementWidth, m_SegmentLength, direction, offset);
        Vector2[] topRightVertices = CurveGenerator.GenerateCurved(roadLeft, m_SegmentLength, direction, offset);
        pavementMeshData.Add(triangulator.Triangulate(topRightRoadVertices, topRightVertices, m_PavementHeight, clockwise, _vertexOffset));

        //Bottom left curve
        direction = new Vector2(1, 1);
        offset = new Vector2(direction.x < 0 ? ROAD_SCALE : 0, direction.y < 0 ? ROAD_SCALE : 0);
        clockwise = direction.x == direction.y;

        Vector2[] bottomLeftVertices = CurveGenerator.GenerateCurved(roadLeft - m_PavementWidth, m_SegmentLength, direction, offset);
        Vector2[] bottomLeftRoadVertices = CurveGenerator.GenerateCurved(roadLeft, m_SegmentLength, direction, offset);
        pavementMeshData.Add(triangulator.Triangulate(bottomLeftVertices, bottomLeftRoadVertices, m_PavementHeight, clockwise, _vertexOffset));

        //Bottom right curve
        direction = new Vector2(-1, 1);
        offset = new Vector2(direction.x < 0 ? ROAD_SCALE : 0, direction.y < 0 ? ROAD_SCALE : 0);
        clockwise = direction.x == direction.y;

        Vector2[] bottomRightRoadVertices = CurveGenerator.GenerateCurved(roadLeft - m_PavementWidth, m_SegmentLength, direction, offset);
        Vector2[] bottomRightVertices = CurveGenerator.GenerateCurved(roadLeft, m_SegmentLength, direction, offset);
        pavementMeshData.Add(triangulator.Triangulate(bottomRightRoadVertices, bottomRightVertices, m_PavementHeight, clockwise, _vertexOffset));


        //Road
        Vector3[] road1Vertices = new Vector3[]
        {
            new Vector3(topLeftRoadVertices[topLeftRoadVertices.Length-1].x, 0, topLeftRoadVertices[topLeftRoadVertices.Length-1].y) + _vertexOffset,
            new Vector3(topRightVertices[topRightVertices.Length-1].x, 0, topRightVertices[topRightVertices.Length-1].y) + _vertexOffset,
            new Vector3(bottomRightVertices[bottomRightVertices.Length - 1].x, 0, bottomRightVertices[bottomRightVertices.Length -1].y) + _vertexOffset,
            new Vector3(bottomLeftRoadVertices[bottomLeftRoadVertices.Length - 1].x, 0, bottomLeftRoadVertices[bottomLeftRoadVertices.Length - 1].y) + _vertexOffset,
        };

        MeshData roadMeshData = new MeshData();
        roadMeshData.vertexCount = pavementMeshData.vertexCount;
        roadMeshData.GenerateTriangles(road1Vertices, new int[] { 0, 1, 2, 0, 2, 3 });
        roadMeshData.Add(triangulator.Fill(topLeftRoadVertices, topRightVertices, 0, clockwise, _vertexOffset));
        roadMeshData.Add(triangulator.Fill(bottomLeftRoadVertices, bottomRightVertices, 0, !clockwise, _vertexOffset));

        List<Vector3> vertices = new List<Vector3>();
        vertices.AddRange(pavementMeshData.vertices);
        vertices.AddRange(roadMeshData.vertices);

        mesh.vertices = vertices.ToArray();
        mesh.SetTriangles(pavementMeshData.triangles.ToArray(), 0);
        mesh.SetTriangles(roadMeshData.triangles.ToArray(), 1);
        mesh.RecalculateNormals();

        m_CrossRoad.mesh = mesh;
    }
}
