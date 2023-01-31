using Grid;
using UnityEngine;
using System;
using System.Collections.Generic;

[ExecuteAlways]
[RequireComponent(typeof(GridTile))]
public class Piece : MonoBehaviour
{

    private readonly List<Vector3> vertices = new List<Vector3>();
    private readonly List<Vector3> normals = new List<Vector3>();
    private readonly List<Vector2> uv = new List<Vector2>();
    private readonly List<int> triangles = new List<int>();

    public int AddVertex(Vector3 position, Vector3 normal, Vector2 uv)
    {
        int index = vertices.Count;
        vertices.Add(position);
        normals.Add(normal);
        this.uv.Add(uv);
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

    private void Update()
    {

    }

    private void DrawGrid()
    {
        var tile = GetComponent<GridTile>();
        if (tile.GetProperty(GridTileProperty.Solid))
        {
            // Draw sand

        }
        else
        {
            // Draw nothing
        }
    }

    private void OnDrawGizmos()
    {
        var tile = GetComponent<GridTile>();
        if (tile.GetProperty(GridTileProperty.Solid))
        {
            Gizmos.color = Color.red;
        }
        else if (tile.GetProperty(GridTileProperty.Water))
        {
            Gizmos.color = Color.blue;
        }
        else
        {
            Gizmos.color = Color.green;
        }
        if (tile.GetNeighbourProperty(0, GridTileProperty.Solid))
        {
            Gizmos.color = Color.yellow;
        }
        Gizmos.DrawCube(transform.position, new Vector3(1, 0.1f, 1));
    }
}
