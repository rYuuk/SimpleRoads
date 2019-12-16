using UnityEngine;

public class Triangulator
{
    private float _leftGap;
    private float _rightGap;

    private int _leftIndex = 0;
    private int _rightIndex = 0;

    private Vector2[] _leftVertices;
    private Vector2[] _rightVertices;

    private bool _isClockwise;
    private Vector3 _vertexOffset;
    private MeshData _meshData;

    public MeshData Triangulate(Vector2[] leftVertices, Vector2[] rightVertices, float height, bool isClockwise, Vector3 vertexOffset, int triangleOffest = 0)
    {
        _leftVertices = leftVertices;
        _rightVertices = rightVertices;
        _leftGap = 1f / (leftVertices.Length - 1);
        _rightGap = 1f / (rightVertices.Length - 1);
        _leftIndex = 0;
        _rightIndex = 0;
        _currentHeight = height;
        _isClockwise = isClockwise;
        _vertexOffset = vertexOffset;
        _meshData = new MeshData(triangleOffest);

        while (!LeftSideComplete() || !RightSideComplete())
            GenerateNextTriangle();

        Extend(leftVertices, height, isClockwise);
        Extend(rightVertices, height, !isClockwise);
        Extend(new Vector2[] { leftVertices[0], rightVertices[0] }, height, !isClockwise);
        Extend(new Vector2[] { leftVertices[leftVertices.Length - 1], rightVertices[rightVertices.Length - 1] }, height, isClockwise);

        return _meshData;
    }

    public MeshData Extend(Vector2[] leftVertices, Vector2[] rightVertices, float height, bool isClockwise, Vector3 vertexOffset, int triangleOffest = 0)
    {
        _vertexOffset = vertexOffset;
        _meshData = new MeshData(triangleOffest);
        Extend(leftVertices, height, isClockwise);
        Extend(rightVertices, height, !isClockwise);
        Extend(new Vector2[] { leftVertices[0], rightVertices[0] }, height, !isClockwise);
        Extend(new Vector2[] { leftVertices[leftVertices.Length - 1], rightVertices[rightVertices.Length - 1] }, height, isClockwise);
        return _meshData;
    }

    public MeshData Fill(Vector2[] leftVertices, Vector2[] rightVertices, float height, bool isClockwise, Vector3 vertexOffset, int triangleOffest = 0)
    {
        _leftVertices = leftVertices;
        _rightVertices = rightVertices;
        _leftGap = 1f / (leftVertices.Length - 1);
        _rightGap = 1f / (rightVertices.Length - 1);
        _leftIndex = 0;
        _rightIndex = 0;
        _currentHeight = height;
        _isClockwise = isClockwise;
        _vertexOffset = vertexOffset;
        _meshData = new MeshData(triangleOffest);
        while (!LeftSideComplete() || !RightSideComplete())
            GenerateNextTriangle();

        return _meshData;
    }

    private void GenerateNextTriangle()
    {
        if (!LeftSideComplete() && GetLeftTriangleCost() < GetRightTriangleCost())
            MakeLeftTriangle();
        else
            MakeRightTriangle();
    }

    float _currentHeight;

    private void MakeLeftTriangle()
    {
        Vector2 v0 = _leftVertices[_leftIndex];
        Vector2 v1 = _leftVertices[_leftIndex + 1];
        Vector2 v2 = _rightVertices[_rightIndex];
        ProcessTriangle(v0, v1, v2);
        _leftIndex++;
    }

    private void MakeRightTriangle()
    {
        Vector2 v0 = _rightVertices[_rightIndex];
        Vector2 v1 = _leftVertices[_leftIndex];
        Vector2 v2 = _rightVertices[_rightIndex + 1];
        ProcessTriangle(v0, v1, v2);
        _rightIndex++;
    }


    private float GetLeftTriangleCost()
    {
        return (_leftIndex + 1) * _leftGap - _rightIndex * _rightGap;
    }

    private float GetRightTriangleCost()
    {
        return (_rightIndex + 1) * _rightGap - _leftIndex * _leftGap;
    }

    private bool LeftSideComplete()
    {
        return _leftIndex == _leftVertices.Length - 1;
    }

    private bool RightSideComplete()
    {
        return _rightIndex == _rightVertices.Length - 1;
    }

    private void ProcessTriangle(Vector2 v0, Vector2 v1, Vector2 v2)
    {
        Vector3 a = new Vector3(v0.x, _currentHeight, v0.y) + _vertexOffset;
        Vector3 b = new Vector3(v1.x, _currentHeight, v1.y) + _vertexOffset;
        Vector3 c = new Vector3(v2.x, _currentHeight, v2.y) + _vertexOffset;

        Vector3[] verts = new Vector3[] { a, b, c };
        _meshData.GenerateTriangles(verts, _isClockwise ? new int[] { 0, 1, 2 } : new int[] { 0, 2, 1 });
    }

    private void Extend(Vector2[] curveVertices, float height, bool isClockwise)
    {
        for (int i = 0; i < curveVertices.Length - 1; i++)
        {
            Vector3[] verts = GenerateQuadVertices(curveVertices[i], curveVertices[i + 1], height);

            if (isClockwise)
                _meshData.GenerateTriangles(verts, new int[] { 0, 1, 3, 1, 2, 3 });
            else
                _meshData.GenerateTriangles(verts, new int[] { 1, 0, 3, 1, 3, 2 });
        }
    }


    private Vector3[] GenerateQuadVertices(Vector2 vertex0, Vector2 vertex1, float topHeight)
    {
        Vector3[] positions = new Vector3[4];
        positions[0] = new Vector3(vertex0.x, topHeight, vertex0.y) + _vertexOffset;
        positions[1] = new Vector3(vertex0.x, 0, vertex0.y) + _vertexOffset;
        positions[2] = new Vector3(vertex1.x, 0, vertex1.y) + _vertexOffset;
        positions[3] = new Vector3(vertex1.x, topHeight, vertex1.y) + _vertexOffset;
        return positions;
    }

}
