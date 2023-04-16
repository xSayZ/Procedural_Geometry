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
    protected bool[] type = new bool[2]; // Array to store the type of the tile (solid/water)
    bool isBridge;

    GridTile tile;

    [SerializeField] Material material;
    private Mesh mesh;

    [SerializeField] private bool showGizmos;

    private void Start()
    {
        mesh = new Mesh();
        mesh.name = "Tile";
        tile = GetComponent<GridTile>();
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material = material;

        type[0] = tile.GetProperty(GridTileProperty.Solid);
        type[1] = tile.GetProperty(GridTileProperty.Water);

    }

    protected bool CheckSameType(bool[,] neighbours, int selectedNeighbour, bool typeValue)
    {
        return neighbours[selectedNeighbour, 0] == typeValue;
    }
    protected bool CompareNeighbourType(int selectedNeighbour, GridTileProperty property)
    {
        return tile.GetNeighbourProperty<GridTileProperty>(selectedNeighbour, property);
    }

    private void Update()
    {
        DrawGrid();
    }

    private void DrawGrid()
    {
       
        var tile = GetComponent<GridTile>();
        MeshBuilder meshBuilder = new MeshBuilder();
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        // Check if the tile is solid and/or water
        bool isSolid = tile.GetProperty(GridTileProperty.Solid);
        bool isWater = tile.GetProperty(GridTileProperty.Water);
        bool hasSolidNeighbor = false;

        // Check if any of the neighboring tiles are solid and/or water
        bool[,] neighbors = new bool[8, 2];
        for (int i = 0; i < 8; i++)
        {
            neighbors[i, 0] = tile.GetNeighbourProperty(i, GridTileProperty.Solid);
            neighbors[i, 1] = tile.GetNeighbourProperty(i, GridTileProperty.Water);
        }

        for (int i = 0; i < 8; i++)
        {
            if (CheckSameType(neighbors, i, true)) hasSolidNeighbor = true; // Check for a specific solid tile type
        }

        bool isParallelSolid =
               CompareNeighbourType(0, GridTileProperty.Solid)
            && CompareNeighbourType(4, GridTileProperty.Solid)
            || CompareNeighbourType(2, GridTileProperty.Solid)
            && CompareNeighbourType(6, GridTileProperty.Solid);

        if (isWater)
        {
            // Define rotation angle in degrees
            float angle = 0f;

            // Define rotation axis (in this case, y-axis)
            Vector3 axis = Vector3.up;

            // Define rotation matrix
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.AngleAxis(angle, axis));
            WaterTile(meshBuilder, rotationMatrix);

            // Bridge
            if (isParallelSolid && isSolid)
            {
                if (CompareNeighbourType(0, GridTileProperty.Solid) && CompareNeighbourType(4, GridTileProperty.Solid))
                {
                    angle = 90;
                    rotationMatrix = Matrix4x4.Rotate(Quaternion.AngleAxis(angle, axis));
                    BridgeTile(meshBuilder, rotationMatrix);
                }
                else
                {
                    BridgeTile(meshBuilder, Matrix4x4.identity);
                }
            }

            if (hasSolidNeighbor)
            {

                // Slopes
                if (CompareNeighbourType(0, GridTileProperty.Solid) && !CompareNeighbourType(0, GridTileProperty.Water))
                {
                    angle = 180f;

                    // Define rotation matrix
                    rotationMatrix = Matrix4x4.Rotate(Quaternion.AngleAxis(angle, axis));

                    SlopeTile(meshBuilder, rotationMatrix);
                }
                if (CompareNeighbourType(2, GridTileProperty.Solid) && !CompareNeighbourType(2, GridTileProperty.Water))
                {
                    angle = 90;

                    // Define rotation matrix
                    rotationMatrix = Matrix4x4.Rotate(Quaternion.AngleAxis(angle, axis));

                    SlopeTile(meshBuilder, rotationMatrix);
                }
                if (CompareNeighbourType(4, GridTileProperty.Solid) && !CompareNeighbourType(4, GridTileProperty.Water))
                {
                    SlopeTile(meshBuilder, Matrix4x4.identity);
                }
                if (CompareNeighbourType(6, GridTileProperty.Solid) && !CompareNeighbourType(6, GridTileProperty.Water))
                {
                    angle = 270;

                    // Define rotation matrix
                    rotationMatrix = Matrix4x4.Rotate(Quaternion.AngleAxis(angle, axis));

                    SlopeTile(meshBuilder, rotationMatrix);
                }

                // Corners
                if (CompareNeighbourType(1, GridTileProperty.Solid)
                    && !CompareNeighbourType(0, GridTileProperty.Solid)
                    && !CompareNeighbourType(2, GridTileProperty.Solid)
                    || CompareNeighbourType(1, GridTileProperty.Solid)
                    && CompareNeighbourType(0, GridTileProperty.Water)
                    && CompareNeighbourType(2, GridTileProperty.Water))
                {
                    if (!CompareNeighbourType(1, GridTileProperty.Water))
                    {
                        angle = 180;

                        // Define rotation matrix
                        rotationMatrix = Matrix4x4.Rotate(Quaternion.AngleAxis(angle, axis));
                        CornerTile(meshBuilder, rotationMatrix);
                    }
                }
                if (CompareNeighbourType(3, GridTileProperty.Solid)
                    && !CompareNeighbourType(2, GridTileProperty.Solid)
                    && !CompareNeighbourType(4, GridTileProperty.Solid)
                    || CompareNeighbourType(3, GridTileProperty.Solid)
                    && CompareNeighbourType(2, GridTileProperty.Water)
                    && CompareNeighbourType(4, GridTileProperty.Water))
                {
                    if (!CompareNeighbourType(3, GridTileProperty.Water))
                    {
                        angle = 90;

                        // Define rotation matrix
                        rotationMatrix = Matrix4x4.Rotate(Quaternion.AngleAxis(angle, axis));
                        CornerTile(meshBuilder, rotationMatrix);
                    }
                }
                if (CompareNeighbourType(5, GridTileProperty.Solid)
                    && !CompareNeighbourType(4, GridTileProperty.Solid)
                    && !CompareNeighbourType(6, GridTileProperty.Solid)
                    || CompareNeighbourType(5, GridTileProperty.Solid)
                    && CompareNeighbourType(4, GridTileProperty.Water)
                    && CompareNeighbourType(6, GridTileProperty.Water))
                {
                    if (!CompareNeighbourType(5, GridTileProperty.Water))
                    {
                        CornerTile(meshBuilder, Matrix4x4.identity);
                    }
                }
                if (CompareNeighbourType(7, GridTileProperty.Solid)
                    && !CompareNeighbourType(6, GridTileProperty.Solid)
                    && !CompareNeighbourType(0, GridTileProperty.Solid)
                    || CompareNeighbourType(7, GridTileProperty.Solid)
                    && CompareNeighbourType(6, GridTileProperty.Water)
                    && CompareNeighbourType(0, GridTileProperty.Water))
                {
                    if (!CompareNeighbourType(7, GridTileProperty.Water))
                    {
                        angle = 270;

                        // Define rotation matrix
                        rotationMatrix = Matrix4x4.Rotate(Quaternion.AngleAxis(angle, axis));
                        CornerTile(meshBuilder, rotationMatrix);
                    }
                }
            }

        }

        else if (isSolid)
        {
            // Define rotation angle in degrees
            float angle = 0f;

            // Define rotation axis (in this case, y-axis)
            Vector3 axis = Vector3.up;

            // Define rotation matrix
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.AngleAxis(angle, axis));
            SolidTile(meshBuilder, rotationMatrix);
        }

        meshBuilder.Build(mesh);
        mesh.RecalculateTangents();
        meshFilter.sharedMesh = mesh;
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        var tile = GetComponent<GridTile>();
        if (tile.GetProperty(GridTileProperty.Water) && tile.GetProperty(GridTileProperty.Solid))
        {
            Gizmos.color = Color.black;
        }
        else if (tile.GetProperty(GridTileProperty.Solid))
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
        Gizmos.DrawCube(transform.position, new Vector3(1, 0.1f, 1));
    }

    private Vector3 CalculateNormal(Vector3 v0, Vector3 v1, Vector3 v3)
    {

        return Vector3.Cross(v1 - v0, v3 - v0);

    }
    private void WaterTile(MeshBuilder meshBuilder, Matrix4x4 rotationMatrix)
    {
        meshBuilder.TextureMatrix =
            Matrix4x4.Translate(new Vector3(0.5f, 0f, 0.0f)) *
            Matrix4x4.Scale(new Vector3(0.5f, 0.5f, 1.0f));


        meshBuilder.VertexMatrix = rotationMatrix;

        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(-0.5f, 0f, -0.5f), Vector3.up, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(-0.5f, 0f, 0.5f), Vector3.up, new Vector2(0, 1)),
            meshBuilder.AddVertex(new Vector3(0.5f, 0f, 0.5f), Vector3.up, new Vector2(1, 1)),
            meshBuilder.AddVertex(new Vector3(0.5f, 0f, -0.5f), Vector3.up, new Vector2(1, 0)));
    }
    private void SolidTile(MeshBuilder meshBuilder, Matrix4x4 rotationMatrix)
    {
        meshBuilder.TextureMatrix =
            Matrix4x4.Translate(new Vector3(0f, 0.5f, 0.0f)) *
            Matrix4x4.Scale(new Vector3(0.5f, 0.5f, 1.0f));
        meshBuilder.VertexMatrix = rotationMatrix;

        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, -0.5f), Vector3.up, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, 0.5f), Vector3.up, new Vector2(0, 1)),
            meshBuilder.AddVertex(new Vector3(0.5f, 0.1f, 0.5f), Vector3.up, new Vector2(1, 1)),
            meshBuilder.AddVertex(new Vector3(0.5f, 0.1f, -0.5f), Vector3.up, new Vector2(1, 0)));
    }
    private void CornerTile(MeshBuilder meshBuilder, Matrix4x4 rotationMatrix)
    {
         meshBuilder.TextureMatrix =
            Matrix4x4.Translate(new Vector3(0.5f, 0.5f, 0.0f)) *
            Matrix4x4.Scale(new Vector3(0.5f, 0.5f, 1.0f));

        meshBuilder.VertexMatrix = rotationMatrix;

        Vector3 normal;
        normal = CalculateNormal(new Vector3(-0.35f, 0.08f, -0.5f), new Vector3(-0.40f, 0.08f, -0.40f), new Vector3(-0.25f, 0f, -0.5f));
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(-0.35f, 0.08f, -0.5f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(-0.40f, 0.08f, -0.40f), normal, new Vector2(0, 1)),
            meshBuilder.AddVertex(new Vector3(-0.3f, 0f, -0.35f), normal, new Vector2(1, 1)),
            meshBuilder.AddVertex(new Vector3(-0.25f, 0f, -0.5f), normal, new Vector2(1, 0)));

        normal = CalculateNormal(new Vector3(-0.5f, 0.08f, -0.35f), new Vector3(-0.5f, 0f, -0.25f), new Vector3(-0.4f, 0.08f, -0.40f));
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(-0.5f, 0.08f, -0.35f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(-0.5f, 0f, -0.25f), normal, new Vector2(0, 1)),
            meshBuilder.AddVertex(new Vector3(-0.30f, 0f, -0.35f), normal, new Vector2(1, 1)),
            meshBuilder.AddVertex(new Vector3(-0.4f, 0.08f, -0.40f), normal, new Vector2(1, 0)));

        normal = CalculateNormal(new Vector3(-0.5f, 0.1f, -0.5f), new Vector3(-0.5f, 0.08f, -0.35f), new Vector3(-0.35f, 0.08f, -0.5f));
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, -0.5f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(-0.5f, 0.08f, -0.35f), normal, new Vector2(0, 1)),
            meshBuilder.AddVertex(new Vector3(-0.4f, 0.08f, -0.40f), normal, new Vector2(1, 1)),
            meshBuilder.AddVertex(new Vector3(-0.35f, 0.08f, -0.5f), normal, new Vector2(1, 0)));
    }
    private void SlopeTile(MeshBuilder meshBuilder, Matrix4x4 rotationMatrix)
    {
        meshBuilder.TextureMatrix =
            Matrix4x4.Translate(new Vector3(0.5f, 0.5f, 0.0f)) *
            Matrix4x4.Scale(new Vector3(0.5f, 0.5f, 1.0f));
        meshBuilder.VertexMatrix = rotationMatrix;

        Vector3 normal;
        normal = CalculateNormal(new Vector3(-0.35f, 0.08f, -0.5f), new Vector3(-0.35f, 0.08f, -0.3f), new Vector3(-0.25f, 0, -0.5f));

        // First Embankment
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(-0.35f, 0.08f, -0.5f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(-0.35f, 0.08f, -0.3f), normal, new Vector2(0, 1)),
            meshBuilder.AddVertex(new Vector3(-0.25f, 0, -0.3f), normal, new Vector2(1, 1)),
            meshBuilder.AddVertex(new Vector3(-0.25f, 0, -0.5f), normal, new Vector2(1, 0)));
        normal = CalculateNormal(new Vector3(-0.5f, 0.1f, -0.5f), new Vector3(-0.5f, 0.1f, -0.3f), new Vector3(-0.35f, 0.08f, -0.5f));
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, -0.5f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, -0.3f), normal, new Vector2(0, 1)),
            meshBuilder.AddVertex(new Vector3(-0.35f, 0.08f, -0.3f), normal, new Vector2(1, 1)),
            meshBuilder.AddVertex(new Vector3(-0.35f, 0.08f, -0.5f), normal, new Vector2(1, 0)));

        // Second Embankment
        normal = CalculateNormal(new Vector3(-0.35f, 0.08f, -0.3f), new Vector3(-0.3f, 0.08f, -0.025f), new Vector3(-0.25f, 0, -0.3f));
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(-0.35f, 0.08f, -0.3f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(-0.3f, 0.08f, -0.025f), normal, new Vector2(0, 1)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0, -0.025f), normal, new Vector2(1, 1)),
            meshBuilder.AddVertex(new Vector3(-0.25f, 0, -0.3f), normal, new Vector2(1, 0)));
        normal = CalculateNormal(new Vector3(-0.5f, 0.1f, -0.3f), new Vector3(-0.5f, 0.1f, -0.025f), new Vector3(-0.35f, 0.08f, -0.3f));
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, -0.3f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, -0.025f), normal, new Vector2(0, 1)),
            meshBuilder.AddVertex(new Vector3(-0.3f, 0.08f, -0.025f), normal, new Vector2(1, 1)),
            meshBuilder.AddVertex(new Vector3(-0.35f, 0.08f, -0.3f), normal, new Vector2(1, 0)));

        // Third Embankment
        normal = CalculateNormal(new Vector3(-0.3f, 0.08f, -0.025f), new Vector3(-0.3f, 0.08f, 0.25f), new Vector3(-0.2f, 0, -0.025f));
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(-0.3f, 0.08f, -0.025f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(-0.3f, 0.08f, 0.25f), normal, new Vector2(0, 1)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0, 0.25f), normal, new Vector2(1, 1)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0, -0.025f), normal, new Vector2(1, 0)));
        normal = CalculateNormal(new Vector3(-0.5f, 0.1f, -0.025f), new Vector3(-0.5f, 0.1f, 0.25f), new Vector3(-0.3f, 0.08f, -0.025f));
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, -0.025f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, 0.25f), normal, new Vector2(0, 1)),
            meshBuilder.AddVertex(new Vector3(-0.3f, 0.08f, 0.25f), normal, new Vector2(1, 1)),
            meshBuilder.AddVertex(new Vector3(-0.3f, 0.08f, -0.025f), normal, new Vector2(1, 0)));

        // Fourth Embankment
        normal = CalculateNormal(new Vector3(-0.3f, 0.08f, 0.25f), new Vector3(-0.35f, 0.08f, 0.5f), new Vector3(-0.2f, 0, 0.25f));
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(-0.3f, 0.08f, 0.25f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(-0.35f, 0.08f, 0.5f), normal, new Vector2(0, 1)),
            meshBuilder.AddVertex(new Vector3(-0.25f, 0, 0.5f), normal, new Vector2(1, 1)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0, 0.25f), normal, new Vector2(1, 0)));
        normal = CalculateNormal(new Vector3(-0.5f, 0.1f, 0.25f), new Vector3(-0.5f, 0.1f, 0.5f), new Vector3(-0.3f, 0.08f, 0.25f));
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, 0.25f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, 0.5f), normal, new Vector2(0, 1)),
            meshBuilder.AddVertex(new Vector3(-0.35f, 0.08f, 0.5f), normal, new Vector2(1, 1)),
            meshBuilder.AddVertex(new Vector3(-0.3f, 0.08f, 0.25f), normal, new Vector2(1, 0)));
    }
    private void BridgeTile(MeshBuilder meshBuilder, Matrix4x4 rotationMatrix)
    {
        meshBuilder.TextureMatrix =
            Matrix4x4.Translate(new Vector3(0f, 0f, 0.0f)) *
            Matrix4x4.Scale(new Vector3(0.5f, 0.5f, 1.0f));
        meshBuilder.VertexMatrix = rotationMatrix;

        Vector3 normal;
        normal = CalculateNormal(new Vector3(-0.2f, 0.175f, -0.4f), new Vector3(-0.2f, 0.175f, 0.4f), new Vector3(0.2f, 0.175f, -0.4f));

        // Floor
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(-0.2f, 0.175f, -0.4f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0.175f, 0.4f), normal, new Vector2(0, 1)),
            meshBuilder.AddVertex(new Vector3(0.2f, 0.175f, 0.4f), normal, new Vector2(0.5f, 1)),
            meshBuilder.AddVertex(new Vector3(0.2f, 0.175f, -0.4f), normal, new Vector2(0.5f, 0)));

        normal = CalculateNormal(new Vector3(0.2f, 0.125f, -0.4f), new Vector3(0.2f, 0.125f, 0.4f), new Vector3(-0.2f, 0.125f, -0.4f));
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(0.2f, 0.125f, -0.4f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(0.2f, 0.125f, 0.4f), normal, new Vector2(0, 1)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0.125f, 0.4f), normal, new Vector2(0.5f, 1)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0.125f, -0.4f), normal, new Vector2(0.5f, 0)));

        // Walter Walls
        normal = CalculateNormal(new Vector3(-0.2f, 0.125f, -0.4f), new Vector3(-0.2f, 0.175f, -0.4f), new Vector3(0.2f, 0.125f, -0.4f));
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(-0.2f, 0.125f, -0.4f), normal, new Vector2(0,0)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0.175f, -0.4f), normal, new Vector2(0, 0.1785f)),
            meshBuilder.AddVertex(new Vector3(0.2f, 0.175f, -0.4f), normal, new Vector2(1, 0.1785f)),
            meshBuilder.AddVertex(new Vector3(0.2f, 0.125f, -0.4f), normal, new Vector2(1, 0)));

        normal = CalculateNormal(new Vector3(-0.2f, 0.125f, -0.4f), new Vector3(-0.2f, 0.125f, 0.4f), new Vector3(-0.2f, 0.175f, -0.4f));
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(-0.2f, 0.125f, -0.4f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0.125f, 0.4f), normal, new Vector2(0, 1)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0.175f, 0.4f), normal, new Vector2(0.0625f, 1)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0.175f, -0.4f), normal, new Vector2(0.0625f, 0)));

        normal = CalculateNormal(new Vector3(0.2f, 0.125f, 0.4f), new Vector3(0.2f, 0.175f, 0.4f), new Vector3(-0.2f, 0.125f, 0.4f));
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(0.2f, 0.125f, 0.4f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(0.2f, 0.175f, 0.4f), normal, new Vector2(0, 0.1785f)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0.175f, 0.4f), normal, new Vector2(1, 0.1785f)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0.125f, 0.4f), normal, new Vector2(1, 0)));


        meshBuilder.TextureMatrix =
            Matrix4x4.Translate(new Vector3(0f, 0.125f, 0.0f)) *
            Matrix4x4.Scale(new Vector3(0.5f, 0.5f, 1.0f));
        normal = CalculateNormal(new Vector3(0.2f, 0.125f, -0.4f), new Vector3(0.2f, 0.175f, -0.4f), new Vector3(0.2f, 0.125f, 0.4f));
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(0.2f, 0.125f, -0.4f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(0.2f, 0.175f, -0.4f), normal, new Vector2(0, 0.0625f)),
            meshBuilder.AddVertex(new Vector3(0.2f, 0.175f, 0.4f), normal, new Vector2(1, 0.0625f)),
            meshBuilder.AddVertex(new Vector3(0.2f, 0.125f, 0.4f), normal, new Vector2(1, 0)));

        meshBuilder.TextureMatrix =
            Matrix4x4.Translate(new Vector3(0f, 0f, 0.0f)) *
            Matrix4x4.Scale(new Vector3(0.5f, 0.5f, 1.0f));
        #region Steps
        normal = CalculateNormal(new Vector3(-0.2f, 0.125f, -0.5f), new Vector3(-0.2f, 0.125f, -0.4f), new Vector3(0.2f, 0.125f, -0.5f));
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(-0.2f, 0.125f, -0.5f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0.125f, -0.4f), normal, new Vector2(0, 0.1785f)),
            meshBuilder.AddVertex(new Vector3(0.2f, 0.125f, -0.4f), normal, new Vector2(1, 0.1785f)),
            meshBuilder.AddVertex(new Vector3(0.2f, 0.125f, -0.5f), normal, new Vector2(1, 0)));

        normal = CalculateNormal(new Vector3(0.2f, 0.125f, 0.5f), new Vector3(0.2f, 0.125f, 0.4f), new Vector3(-0.2f, 0.125f, 0.5f));
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(0.2f, 0.125f, 0.5f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(0.2f, 0.125f, 0.4f), normal, new Vector2(0, 0.1785f)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0.125f, 0.4f), normal, new Vector2(1, 0.1785f)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0.125f, 0.5f), normal, new Vector2(1, 0)));

        normal = CalculateNormal(new Vector3(-0.2f, 0f, -0.5f), new Vector3(-0.2f, 0.125f, -0.5f), new Vector3(0.2f, 0f, -0.5f));
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(-0.2f, 0f, -0.5f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0.125f, -0.5f), normal, new Vector2(0, 0.1785f)),
            meshBuilder.AddVertex(new Vector3(0.2f, 0.125f, -0.5f), normal, new Vector2(1, 0.1785f)),
            meshBuilder.AddVertex(new Vector3(0.2f, 0f, -0.5f), normal, new Vector2(1, 0)));

        normal = CalculateNormal(new Vector3(0.2f, 0f, -0.4f), new Vector3(0.2f, 0.125f, -0.4f), new Vector3(-0.2f, 0f, -0.4f));
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(0.2f, 0f, -0.4f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(0.2f, 0.125f, -0.4f), normal, new Vector2(0, 0.1785f)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0.125f, -0.4f), normal, new Vector2(1, 0.1785f)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0f, -0.4f), normal, new Vector2(1, 0)));

        normal = CalculateNormal(new Vector3(0.2f, 0f, 0.5f), new Vector3(0.2f, 0.125f, 0.5f), new Vector3(-0.2f, 0f, 0.5f));
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(0.2f, 0f, 0.5f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(0.2f, 0.125f, 0.5f), normal, new Vector2(0, 0.1785f)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0.125f, 0.5f), normal, new Vector2(1, 0.1785f)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0f, 0.5f), normal, new Vector2(1, 0)));

        normal = CalculateNormal(new Vector3(-0.2f, 0f, 0.4f), new Vector3(-0.2f, 0.125f, 0.4f), new Vector3(0.2f, 0f, 0.4f));
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(-0.2f, 0f, 0.4f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0.125f, 0.4f), normal, new Vector2(0, 0.1785f)),
            meshBuilder.AddVertex(new Vector3(0.2f, 0.125f, 0.4f), normal, new Vector2(1, 0.1785f)),
            meshBuilder.AddVertex(new Vector3(0.2f, 0f, 0.4f), normal, new Vector2(1, 0)));

        meshBuilder.TextureMatrix =
            Matrix4x4.Translate(new Vector3(0f, 0f, 0.0f)) *
            Matrix4x4.Scale(new Vector3(0.2f, 0.2f, 1.0f));
        normal = CalculateNormal(new Vector3(-0.2f, 0f, -0.4f), new Vector3(-0.2f, 0.125f, -0.4f), new Vector3(-0.2f, 0f, -0.5f));
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(-0.2f, 0f, -0.4f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0.125f, -0.4f), normal, new Vector2(0, 0.625f)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0.125f, -0.5f), normal, new Vector2(1, 0.625f)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0f, -0.5f), normal, new Vector2(1, 0)));

        normal = CalculateNormal(new Vector3(0.2f, 0f, -0.5f), new Vector3(0.2f, 0.125f, -0.5f), new Vector3(0.2f, 0f, -0.4f));
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(0.2f, 0f, -0.5f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(0.2f, 0.125f, -0.5f), normal, new Vector2(0, 0.625f)),
            meshBuilder.AddVertex(new Vector3(0.2f, 0.125f, -0.4f), normal, new Vector2(1, 0.625f)),
            meshBuilder.AddVertex(new Vector3(0.2f, 0f, -0.4f), normal, new Vector2(1, 0)));

        meshBuilder.TextureMatrix =
            Matrix4x4.Translate(new Vector3(0f, 0f, 0.0f)) *
            Matrix4x4.Scale(new Vector3(0.2f, 0.2f, 1.0f));
        normal = CalculateNormal(new Vector3(0.2f, 0f, 0.4f), new Vector3(0.2f, 0.125f, 0.4f), new Vector3(0.2f, 0f, 0.5f));
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(0.2f, 0f, 0.4f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(0.2f, 0.125f, 0.4f), normal, new Vector2(0, 0.625f)),
            meshBuilder.AddVertex(new Vector3(0.2f, 0.125f, 0.5f), normal, new Vector2(1, 0.625f)),
            meshBuilder.AddVertex(new Vector3(0.2f, 0f, 0.5f), normal, new Vector2(1, 0)));

        normal = CalculateNormal(new Vector3(-0.2f, 0f, 0.5f), new Vector3(-0.2f, 0.125f, 0.5f), new Vector3(-0.2f, 0f, 0.4f));
        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(-0.2f, 0f, 0.5f), normal, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0.125f, 0.5f), normal, new Vector2(0, 0.625f)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0.125f, 0.4f), normal, new Vector2(1, 0.625f)),
            meshBuilder.AddVertex(new Vector3(-0.2f, 0f, 0.4f), normal, new Vector2(1, 0)));
        #endregion
    }
}
