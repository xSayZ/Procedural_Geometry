using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuilder
{
    private readonly List<Vector3> vertices = new List<Vector3>();
    private readonly List<Vector3> normals = new List<Vector3>();
    private readonly List<Vector2> uv = new List<Vector2>();
    private readonly List<int> triangles = new List<int>();

    public Matrix4x4 VertexMatrix = Matrix4x4.identity;
    public Matrix4x4 TextureMatrix = Matrix4x4.identity;

    public int AddVertex(Vector3 position, Vector3 normal, Vector2 uv)
    {
        var index = vertices.Count;
        vertices.Add(VertexMatrix.MultiplyPoint(position));
        normals.Add(VertexMatrix.MultiplyVector(normal));
        this.uv.Add(TextureMatrix.MultiplyPoint(uv));
        return index;
    }

    public void AddQuad(int bottomLeft, int topLeft, int topRight, int bottomRight)
    {
        // First triangle
        triangles.Add(bottomLeft);
        triangles.Add(topLeft);
        triangles.Add(topRight);

        // Second triangle
        triangles.Add(bottomLeft);
        triangles.Add(topRight);
        triangles.Add(bottomRight);
    }

    public void Build(Mesh mesh)
    {
        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uv);
        mesh.SetIndices(triangles, MeshTopology.Triangles, 0);
        mesh.MarkModified();
    }
}