using UnityEngine;
using System.Collections.Generic;

public class MeshData
{
    public List<Vector3> vertices;
    public List<int> triangles;
    public int vertexCount;

    public MeshData(int index = 0)
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        vertexCount = index;
    }

    public void GenerateTriangles(Vector3[] verts, int[] tri)
    {
        vertices.AddRange(verts);
        for (int i = 0; i < tri.Length; i++)
            triangles.Add(tri[i] + vertexCount);
        vertexCount += verts.Length;
    }

    public void Add(MeshData meshData)
    {
        vertices.AddRange(meshData.vertices);
        foreach (var index in meshData.triangles)
            triangles.Add(index + vertexCount);
        vertexCount += meshData.vertexCount;
    }
}
