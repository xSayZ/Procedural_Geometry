using Grid;
using UnityEngine;
using System;
using System.Collections.Generic;

[ExecuteAlways]
[RequireComponent(typeof(GridTile))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Piece : MonoBehaviour
{

    private void Start()
    {
    }

    private void Update()
    {
        DrawGrid();
    }

    private void DrawGrid()
    {
        var tile = GetComponent<GridTile>();
        MeshBuilder meshBuilder = new MeshBuilder();

        if (tile.GetProperty(GridTileProperty.Solid) && tile.GetProperty(GridTileProperty.Water))
        {
            meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(-0.5f, 0, -0.5f), Vector3.up, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(-0.5f, 0, 0.5f), Vector3.up, new Vector2(0, 1)),
            meshBuilder.AddVertex(new Vector3(0.5f, 0, 0.5f), Vector3.up, new Vector2(1, 1)),
            meshBuilder.AddVertex(new Vector3(0.5f, 0, -0.5f), Vector3.up, new Vector2(1, 0))
);
        }
        else if (tile.GetProperty(GridTileProperty.Solid))
        {
            meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(-0.5f, 0, -0.5f), Vector3.up, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(-0.5f, 0, 0.5f), Vector3.up, new Vector2(0, 1)),
            meshBuilder.AddVertex(new Vector3(0.5f, 0, 0.5f), Vector3.up, new Vector2(1, 1)),
            meshBuilder.AddVertex(new Vector3(0.5f, 0, -0.5f), Vector3.up, new Vector2(1, 0))
        );
        }
        else if (tile.GetProperty(GridTileProperty.Water))
        {
            meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(-0.5f, 0, -0.5f), Vector3.up, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(-0.5f, 0, 0.5f), Vector3.up, new Vector2(0, 1)),
            meshBuilder.AddVertex(new Vector3(0.5f, 0, 0.5f), Vector3.up, new Vector2(1, 1)),
            meshBuilder.AddVertex(new Vector3(0.5f, 0, -0.5f), Vector3.up, new Vector2(1, 0))
);
        }
        else
        {
        }

        Mesh mesh = new Mesh();
        meshBuilder.Build(mesh);
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    /*private void OnDrawGizmos()
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
    */
}
