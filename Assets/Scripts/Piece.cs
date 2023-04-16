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
    bool isParallel;
    bool isLeftCorner;
    bool isRightCorner;
    bool CentreExists;
    

    [SerializeField] Material material;
    private Mesh mesh;


    private void Start()
    {
        mesh = new Mesh();
        mesh.name = "Tile";
        var tile = GetComponent<GridTile>();

        type[0] = tile.GetProperty(GridTileProperty.Solid);
        type[1] = tile.GetProperty(GridTileProperty.Water);

    }

    protected bool CheckSameType(bool[,] neighbours, int selectedNeighbour, bool typeValue)
    {
        return neighbours[selectedNeighbour, 0] == typeValue;
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
        bool hasWaterNeighbor = false;

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
            if (CheckSameType(neighbors, i, false)) hasWaterNeighbor = true; // Check for a specific water tile type
        }


        bool isParallelSolid = (CheckSameType(neighbors, 4, true) && CheckSameType(neighbors, 0, true))
                    || (CheckSameType(neighbors, 6, true) && CheckSameType(neighbors, 2, true));



        bool isBridge = isSolid && isWater;

        if (isBridge)
        {
            if (isParallelSolid)
            {
                // Bottom face
                meshBuilder.AddQuad(
                    meshBuilder.AddVertex(new Vector3(-0.4f, 0.08f, -0.5f), Vector3.up, new Vector2(0, 0)),
                    meshBuilder.AddVertex(new Vector3(-0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(0, 1)),
                    meshBuilder.AddVertex(new Vector3(-0.3f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 1)),
                    meshBuilder.AddVertex(new Vector3(-0.3f, 0.08f, -0.5f), Vector3.up, new Vector2(1, 0)));

                // Walls
                

                if (CheckSameType(neighbors, 0, false) && CheckSameType(neighbors, 2, true) && CheckSameType(neighbors, 6, true))
                {
                    // Define rotation angle in degrees
                    float angle = 90f;

                    // Define rotation axis (in this case, y-axis)
                    Vector3 axis = Vector3.up;

                    // Define rotation matrix
                    Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.AngleAxis(angle, axis));

                    // First Square
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(-0.5f, 0, 0.25f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(-0.3f, 0, 0.25f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.3f, 0, -0.25f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.5f, 0, -0.25f), Vector3.up, new Vector2(1, 0)));
                    // Second
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(-0.3f, 0, 0.25f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(-0.025f, 0, 0.2f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.025f, 0, -0.3f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.3f, 0, -0.25f), Vector3.up, new Vector2(1, 0)));
                    // Third
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(-0.025f, 0, 0.2f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(0.25f, 0, 0.2f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(0.25f, 0, -0.3f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.025f, 0, -0.3f), Vector3.up, new Vector2(1, 0)));
                    // Fourth
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(0.25f, 0, 0.2f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(0.5f, 0, 0.25f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(0.5f, 0, -0.25f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(0.25f, 0, -0.3f), Vector3.up, new Vector2(1, 0)));

                    #region Left Side Embankment
                    // First Embankment
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.35f, 0.08f, -0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.35f, 0.08f, -0.3f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.25f, 0, -0.3f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.25f, 0, -0.5f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.5f, 0.1f, -0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.5f, 0.1f, -0.3f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.35f, 0.08f, -0.3f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.35f, 0.08f, -0.5f), Vector3.up, new Vector2(1, 0)));

                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.35f, 0.08f, -0.3f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.3f, 0.08f, -0.025f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.2f, 0, -0.025f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.25f, 0, -0.3f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.5f, 0.1f, -0.3f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.5f, 0.1f, -0.025f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.3f, 0.08f, -0.025f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.35f, 0.08f, -0.3f), Vector3.up, new Vector2(1, 0)));

                    // Third Embankment
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.3f, 0.08f, -0.025f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.3f, 0.08f, 0.25f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.2f, 0, 0.25f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.2f, 0, -0.025f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.5f, 0.1f, -0.025f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.5f, 0.1f, 0.25f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.3f, 0.08f, 0.25f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.3f, 0.08f, -0.025f), Vector3.up, new Vector2(1, 0)));

                    // Fourth Embankment
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.3f, 0.08f, 0.25f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.35f, 0.08f, 0.5f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.25f, 0, 0.5f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.2f, 0, 0.25f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.5f, 0.1f, 0.25f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.5f, 0.1f, 0.5f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.35f, 0.08f, 0.5f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.3f, 0.08f, 0.25f), Vector3.up, new Vector2(1, 0)));
                    #endregion
                    #region Right Side Embankment
                    // First Embankment
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.25f, 0f, -0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.25f, 0f, -0.3f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.35f, 0.08f, -0.3f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.35f, 0.08f, -0.5f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.35f, 0.08f, -0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.35f, 0.08f, -0.3f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.5f, 0.1f, -0.3f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.5f, 0.1f, -0.5f), Vector3.up, new Vector2(1, 0)));

                    // Second Embankment
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.25f, 0f, -0.3f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.3f, 0f, -0.025f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.4f, 0.08f, -0.025f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.35f, 0.08f, -0.3f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.35f, 0.08f, -0.3f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.4f, 0.08f, -0.025f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.5f, 0.1f, -0.025f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.5f, 0.1f, -0.3f), Vector3.up, new Vector2(1, 0)));

                    // Third Embankment
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.3f, 0f, -0.025f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.3f, 0f, 0.25f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.4f, 0.08f, 0.25f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.4f, 0.08f, -0.025f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.4f, 0.08f, -0.025f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.4f, 0.08f, 0.25f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.5f, 0.1f, 0.25f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.5f, 0.1f, -0.025f), Vector3.up, new Vector2(1, 0)));

                    // Fourth Embankment
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.30f, 0f, 0.25f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.25f, 0f, 0.5f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.35f, 0.08f, 0.5f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.40f, 0.08f, 0.25f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.4f, 0.08f, 0.25f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.35f, 0.08f, 0.5f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.5f, 0.1f, 0.5f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.5f, 0.1f, 0.25f), Vector3.up, new Vector2(1, 0)));

                    #endregion

                }
            }
            else
            {
                // Draw water
                // Comment where the mesh should be drawn
                //meshBuilder.AddQuad(
                //meshBuilder.AddVertex(new Vector3(-0.5f, 0, -0.5f), Vector3.up, new Vector2(0, 0)),
                //meshBuilder.AddVertex(new Vector3(-0.5f, 0, 0.5f), Vector3.up, new Vector2(0, 1)),
                //meshBuilder.AddVertex(new Vector3(0.5f, 0, 0.5f), Vector3.up, new Vector2(1, 1)),
                //meshBuilder.AddVertex(new Vector3(0.5f, 0, -0.5f), Vector3.up, new Vector2(1, 0)));
            }
        }
        else if (isSolid)
        {

        // Comment where the mesh should be drawn
        meshBuilder.TextureMatrix =
            Matrix4x4.Scale(new Vector3(1.0f, 1.0f, 1.0f));

        meshBuilder.AddQuad(
            meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, -0.5f), Vector3.up, new Vector2(0, 0)),
            meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, 0.5f), Vector3.up, new Vector2(0, 1)),
            meshBuilder.AddVertex(new Vector3(0.5f, 0.1f, 0.5f), Vector3.up, new Vector2(1, 1)),
            meshBuilder.AddVertex(new Vector3(0.5f, 0.1f, -0.5f), Vector3.up, new Vector2(1, 0)));
        }
        else if (isWater)
        {
            Debug.Log("is Water: " + isWater);
            if (hasSolidNeighbor)
            {
                Debug.Log("has Solid Neighbor: " + hasSolidNeighbor);
                if (isParallelSolid)
                {
                    if (CheckSameType(neighbors, 0, false) && CheckSameType(neighbors, 2, true) && CheckSameType(neighbors, 6, true))
                    {
                        if (isBridge)
                            return;

                        // Define rotation angle in degrees
                        float angle = 90f;

                        // Define rotation axis (in this case, y-axis)
                        Vector3 axis = Vector3.up;

                        // Define rotation matrix
                        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.AngleAxis(angle, axis));

                        // First Square
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(new Vector3(-0.5f, 0, 0.25f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(new Vector3(-0.3f, 0, 0.25f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(new Vector3(-0.3f, 0, -0.25f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(new Vector3(-0.5f, 0, -0.25f), Vector3.up, new Vector2(1, 0)));
                        // Second
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(new Vector3(-0.3f, 0, 0.25f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(new Vector3(-0.025f, 0, 0.2f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(new Vector3(-0.025f, 0, -0.3f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(new Vector3(-0.3f, 0, -0.25f), Vector3.up, new Vector2(1, 0)));
                        // Third
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(new Vector3(-0.025f, 0, 0.2f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(new Vector3(0.25f, 0, 0.2f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(new Vector3(0.25f, 0, -0.3f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(new Vector3(-0.025f, 0, -0.3f), Vector3.up, new Vector2(1, 0)));
                        // Fourth
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(new Vector3(0.25f, 0, 0.2f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(new Vector3(0.5f, 0, 0.25f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(new Vector3(0.5f, 0, -0.25f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(new Vector3(0.25f, 0, -0.3f), Vector3.up, new Vector2(1, 0)));

                        #region Left Side Embankment
                        // First Embankment
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.35f, 0.08f, -0.5f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.35f, 0.08f, -0.3f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.25f, 0, -0.3f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.25f, 0, -0.5f), Vector3.up, new Vector2(1, 0)));
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.5f, 0.1f, -0.5f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.5f, 0.1f, -0.3f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.35f, 0.08f, -0.3f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.35f, 0.08f, -0.5f), Vector3.up, new Vector2(1, 0)));

                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.35f, 0.08f, -0.3f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.3f, 0.08f, -0.025f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.2f, 0, -0.025f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.25f, 0, -0.3f), Vector3.up, new Vector2(1, 0)));
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.5f, 0.1f, -0.3f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.5f, 0.1f, -0.025f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.3f, 0.08f, -0.025f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.35f, 0.08f, -0.3f), Vector3.up, new Vector2(1, 0)));

                        // Third Embankment
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.3f, 0.08f, -0.025f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.3f, 0.08f, 0.25f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.2f, 0, 0.25f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.2f, 0, -0.025f), Vector3.up, new Vector2(1, 0)));
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.5f, 0.1f, -0.025f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.5f, 0.1f, 0.25f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.3f, 0.08f, 0.25f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.3f, 0.08f, -0.025f), Vector3.up, new Vector2(1, 0)));

                        // Fourth Embankment
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.3f, 0.08f, 0.25f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.35f, 0.08f, 0.5f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.25f, 0, 0.5f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.2f, 0, 0.25f), Vector3.up, new Vector2(1, 0)));
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.5f, 0.1f, 0.25f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.5f, 0.1f, 0.5f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.35f, 0.08f, 0.5f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.3f, 0.08f, 0.25f), Vector3.up, new Vector2(1, 0)));
                        #endregion
                        #region Right Side Embankment
                        // First Embankment
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.25f, 0f, -0.5f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.25f, 0f, -0.3f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.35f, 0.08f, -0.3f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.35f, 0.08f, -0.5f), Vector3.up, new Vector2(1, 0)));
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.35f, 0.08f, -0.5f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.35f, 0.08f, -0.3f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.5f, 0.1f, -0.3f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.5f, 0.1f, -0.5f), Vector3.up, new Vector2(1, 0)));

                        // Second Embankment
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.25f, 0f, -0.3f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.3f, 0f, -0.025f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.4f, 0.08f, -0.025f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.35f, 0.08f, -0.3f), Vector3.up, new Vector2(1, 0)));
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.35f, 0.08f, -0.3f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.4f, 0.08f, -0.025f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.5f, 0.1f, -0.025f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.5f, 0.1f, -0.3f), Vector3.up, new Vector2(1, 0)));

                        // Third Embankment
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.3f, 0f, -0.025f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.3f, 0f, 0.25f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.4f, 0.08f, 0.25f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.4f, 0.08f, -0.025f), Vector3.up, new Vector2(1, 0)));
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.4f, 0.08f, -0.025f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.4f, 0.08f, 0.25f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.5f, 0.1f, 0.25f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.5f, 0.1f, -0.025f), Vector3.up, new Vector2(1, 0)));

                        // Fourth Embankment
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.30f, 0f, 0.25f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.25f, 0f, 0.5f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.35f, 0.08f, 0.5f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.40f, 0.08f, 0.25f), Vector3.up, new Vector2(1, 0)));
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.4f, 0.08f, 0.25f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.35f, 0.08f, 0.5f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.5f, 0.1f, 0.5f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(rotationMatrix * new Vector3(0.5f, 0.1f, 0.25f), Vector3.up, new Vector2(1, 0)));

                        #endregion

                    }
                    else
                    {
                        //BOTTOM
                        // First Square
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(new Vector3(-0.25f, 0, -0.5f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(new Vector3(-0.25f, 0, -0.3f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(new Vector3(0.25f, 0, -0.3f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(new Vector3(0.25f, 0, -0.5f), Vector3.up, new Vector2(1, 0)));
                        // Second
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(new Vector3(-0.25f, 0, -0.3f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(new Vector3(-0.2f, 0, -0.025f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(new Vector3(0.3f, 0, -0.025f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(new Vector3(0.25f, 0, -0.3f), Vector3.up, new Vector2(1, 0)));
                        // Third
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(new Vector3(-0.2f, 0, -0.025f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(new Vector3(-0.2f, 0, 0.25f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(new Vector3(0.3f, 0, 0.25f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(new Vector3(0.3f, 0, -0.025f), Vector3.up, new Vector2(1, 0)));
                        // Fourth
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(new Vector3(-0.2f, 0, 0.25f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(new Vector3(-0.25f, 0, 0.5f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(new Vector3(0.25f, 0, 0.5f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(new Vector3(0.3f, 0, 0.25f), Vector3.up, new Vector2(1, 0)));

                        #region Left Side Embankment
                        // Left Side
                        // First Embankment
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(new Vector3(-0.35f, 0.08f, -0.5f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(new Vector3(-0.35f, 0.08f, -0.3f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(new Vector3(-0.25f, 0, -0.3f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(new Vector3(-0.25f, 0, -0.5f), Vector3.up, new Vector2(1, 0)));
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, -0.5f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, -0.3f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(new Vector3(-0.35f, 0.08f, -0.3f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(new Vector3(-0.35f, 0.08f, -0.5f), Vector3.up, new Vector2(1, 0)));

                        // Second Embankment
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(new Vector3(-0.35f, 0.08f, -0.3f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(new Vector3(-0.3f, 0.08f, -0.025f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(new Vector3(-0.2f, 0, -0.025f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(new Vector3(-0.25f, 0, -0.3f), Vector3.up, new Vector2(1, 0)));
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, -0.3f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, -0.025f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(new Vector3(-0.3f, 0.08f, -0.025f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(new Vector3(-0.35f, 0.08f, -0.3f), Vector3.up, new Vector2(1, 0)));

                        // Third Embankment
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(new Vector3(-0.3f, 0.08f, -0.025f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(new Vector3(-0.3f, 0.08f, 0.25f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(new Vector3(-0.2f, 0, 0.25f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(new Vector3(-0.2f, 0, -0.025f), Vector3.up, new Vector2(1, 0)));
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, -0.025f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, 0.25f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(new Vector3(-0.3f, 0.08f, 0.25f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(new Vector3(-0.3f, 0.08f, -0.025f), Vector3.up, new Vector2(1, 0)));

                        // Fourth Embankment
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(new Vector3(-0.3f, 0.08f, 0.25f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(new Vector3(-0.35f, 0.08f, 0.5f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(new Vector3(-0.25f, 0, 0.5f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(new Vector3(-0.2f, 0, 0.25f), Vector3.up, new Vector2(1, 0)));
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, 0.25f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, 0.5f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(new Vector3(-0.35f, 0.08f, 0.5f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(new Vector3(-0.3f, 0.08f, 0.25f), Vector3.up, new Vector2(1, 0)));

                        #endregion
                        #region Right Side Embankment
                        // First Embankment
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(new Vector3(0.25f, 0f, -0.5f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(new Vector3(0.25f, 0f, -0.3f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(new Vector3(0.35f, 0.08f, -0.3f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(new Vector3(0.35f, 0.08f, -0.5f), Vector3.up, new Vector2(1, 0)));
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(new Vector3(0.35f, 0.08f, -0.5f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(new Vector3(0.35f, 0.08f, -0.3f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(new Vector3(0.5f, 0.1f, -0.3f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(new Vector3(0.5f, 0.1f, -0.5f), Vector3.up, new Vector2(1, 0)));

                        // Second Embankment
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(new Vector3(0.25f, 0f, -0.3f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(new Vector3(0.3f, 0f, -0.025f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(new Vector3(0.4f, 0.08f, -0.025f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(new Vector3(0.35f, 0.08f, -0.3f), Vector3.up, new Vector2(1, 0)));
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(new Vector3(0.35f, 0.08f, -0.3f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(new Vector3(0.4f, 0.08f, -0.025f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(new Vector3(0.5f, 0.1f, -0.025f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(new Vector3(0.5f, 0.1f, -0.3f), Vector3.up, new Vector2(1, 0)));

                        // Third Embankment
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(new Vector3(0.3f, 0f, -0.025f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(new Vector3(0.3f, 0f, 0.25f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(new Vector3(0.4f, 0.08f, 0.25f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(new Vector3(0.4f, 0.08f, -0.025f), Vector3.up, new Vector2(1, 0)));
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(new Vector3(0.4f, 0.08f, -0.025f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(new Vector3(0.4f, 0.08f, 0.25f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(new Vector3(0.5f, 0.1f, 0.25f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(new Vector3(0.5f, 0.1f, -0.025f), Vector3.up, new Vector2(1, 0)));

                        // Fourth Embankment
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(new Vector3(0.30f, 0f, 0.25f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(new Vector3(0.25f, 0f, 0.5f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(new Vector3(0.35f, 0.08f, 0.5f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(new Vector3(0.40f, 0.08f, 0.25f), Vector3.up, new Vector2(1, 0)));
                        meshBuilder.AddQuad(
                            meshBuilder.AddVertex(new Vector3(0.4f, 0.08f, 0.25f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(new Vector3(0.35f, 0.08f, 0.5f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(new Vector3(0.5f, 0.1f, 0.5f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(new Vector3(0.5f, 0.1f, 0.25f), Vector3.up, new Vector2(1, 0)));

                        #endregion

                    }
            
                }
                ///
                /// CORNERS
                ///
                else if (CheckSameType(neighbors, 0, false) && CheckSameType(neighbors, 2, false) &&
                         CheckSameType(neighbors, 4, true) && CheckSameType(neighbors, 6, true))
                {
                    isLeftCorner = true;
                    // Bottom
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(-0.3f, 0f, -0.3f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(-0.3f, 0f, 0.5f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(0.5f, 0f, 0.5f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(0.5f, 0f, -0.3f), Vector3.up, new Vector2(1, 0)));

                    // Embankment
                    // Left
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(-0.4f, 0.08f, -0.3f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(-0.4f, 0.08f, 0.5f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.3f, 0, 0.5f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.3f, 0, -0.3f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, -0.3f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, 0.5f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.4f, 0.08f, 0.5f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.4f, 0.08f, -0.3f), Vector3.up, new Vector2(1, 0)));
                    // Down
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(-0.3f, 0.08f, -0.4f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(-0.3f, 0f, -0.3f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(0.5f, 0f, -0.3f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(0.5f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(-0.3f, 0.1f, -0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(-0.3f, 0.08f, -0.4f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(0.5f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(0.5f, 0.1f, -0.5f), Vector3.up, new Vector2(1, 0)));
                    // Corner
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(-0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(-0.4f, 0.08f, -0.3f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.3f, 0f, -0.3f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.3f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, -0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, -0.4f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.4f, 0.1f, -0.5f), Vector3.up, new Vector2(0, 0)));

                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, -0.4f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, -0.3f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.4f, 0.08f, -0.3f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(-0.4f, 0.1f, -0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(-0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.3f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.3f, 0.1f, -0.5f), Vector3.up, new Vector2(1, 0)));

                }
                else if (CheckSameType(neighbors, 0, true) && CheckSameType(neighbors, 2, false) &&
                         CheckSameType(neighbors, 4, false) && CheckSameType(neighbors, 6, true))
                {
                    isRightCorner = true;
                    // Bottom
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(-0.5f, 0f, -0.3f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(-0.5f, 0f, 0.5f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(0.3f, 0f, 0.5f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(0.3f, 0f, -0.3f), Vector3.up, new Vector2(1, 0)));

                    // Right
                    meshBuilder.AddQuad(
                         meshBuilder.AddVertex(new Vector3(0.3f, 0f, -0.3f), Vector3.up, new Vector2(0, 0)),
                         meshBuilder.AddVertex(new Vector3(0.3f, 0f, 0.5f), Vector3.up, new Vector2(0, 1)),
                         meshBuilder.AddVertex(new Vector3(0.4f, 0.08f, 0.5f), Vector3.up, new Vector2(1, 1)),
                         meshBuilder.AddVertex(new Vector3(0.4f, 0.08f, -0.3f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(0.5f, 0.1f, 0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(0.5f, 0.1f, -0.3f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(0.4f, 0.08f, -0.3f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(0.4f, 0.08f, 0.5f), Vector3.up, new Vector2(1, 0)));


                    // Down
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(-0.5f, 0.08f, -0.4f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(-0.5f, 0f, -0.3f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(0.3f, 0f, -0.3f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(0.3f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, -0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(-0.5f, 0.08f, -0.4f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(0.3f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(0.3f, 0.1f, -0.5f), Vector3.up, new Vector2(1, 0)));

                }

                ///
                /// SIDES
                ///
                else if (CheckSameType(neighbors, 0, true) && CheckSameType(neighbors, 4, false))
                {
                    meshBuilder.AddQuad(
                            meshBuilder.AddVertex(new Vector3(-0.5f, 0f, -0.5f), Vector3.up, new Vector2(0, 0)),
                            meshBuilder.AddVertex(new Vector3(-0.5f, 0f, 0.5f), Vector3.up, new Vector2(0, 1)),
                            meshBuilder.AddVertex(new Vector3(0.3f, 0f, 0.5f), Vector3.up, new Vector2(1, 1)),
                            meshBuilder.AddVertex(new Vector3(0.3f, 0f, -0.5f), Vector3.up, new Vector2(1, 0)));

                    // Embankment
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(0.3f, 0f, -0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(0.3f, 0f, 0.5f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(0.4f, 0.08f, 0.5f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(0.4f, 0.08f, -0.5f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(0.4f, 0.08f, -0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(0.4f, 0.08f, 0.5f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(0.5f, 0.1f, 0.5f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(0.5f, 0.1f, -0.5f), Vector3.up, new Vector2(1, 0)));
                }
                else if (CheckSameType(neighbors, 4, true) && CheckSameType(neighbors, 0, false))
                {
                    // Define rotation angle in degrees
                    float angle = 180f;

                    // Define rotation axis (in this case, y-axis)
                    Vector3 axis = Vector3.up;

                    // Define rotation matrix
                    Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.AngleAxis(angle, axis));

                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.5f, 0f, -0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.5f, 0f, 0.5f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.3f, 0f, 0.5f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.3f, 0f, -0.5f), Vector3.up, new Vector2(1, 0)));

                    // Embankment
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.3f, 0f, -0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.3f, 0f, 0.5f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.4f, 0.08f, 0.5f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.4f, 0.08f, -0.5f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.4f, 0.08f, -0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.4f, 0.08f, 0.5f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.5f, 0.1f, 0.5f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.5f, 0.1f, -0.5f), Vector3.up, new Vector2(1, 0)));
                }


                ///
                /// CROSSROADS = x =
                ///             | |

                        else if (CheckSameType(neighbors, 0, false) && CheckSameType(neighbors, 2, false)
                    && CheckSameType(neighbors, 4, false) && CheckSameType(neighbors, 5, true)
                    && CheckSameType(neighbors, 6, false) && CheckSameType(neighbors, 7, true))
                {
                    meshBuilder.AddQuad(
                    meshBuilder.AddVertex(new Vector3(-0.5f, 0f, -0.5f), Vector3.up, new Vector2(0, 0)),
                    meshBuilder.AddVertex(new Vector3(-0.5f, 0f, 0.5f), Vector3.up, new Vector2(0, 1)),
                    meshBuilder.AddVertex(new Vector3(0.5f, 0f, 0.5f), Vector3.up, new Vector2(1, 1)),
                    meshBuilder.AddVertex(new Vector3(0.5f, 0f, -0.5f), Vector3.up, new Vector2(1, 0)));

                    #region Left Corner
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(-0.35f, 0.08f, -0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(-0.40f, 0.08f, -0.4f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.3f, 0f, -0.35f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.25f, 0f, -0.5f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(-0.5f, 0.08f, -0.4f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(-0.5f, 0f, -0.3f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.4f, 0f, -0.3f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, -0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(-0.5f, 0.08f, -0.4f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.35f, 0.08f, -0.5f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(-0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(-0.4f, 0f, -0.3f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.3f, 0f, -0.35f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 0)));
                    #endregion
                    #region Right corner
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(0.25f, 0f, -0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(0.3f, 0f, -0.35f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(0.35f, 0.08f, -0.5f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(0.4f, 0f, -0.3f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(0.5f, 0f, -0.3f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(0.5f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(0.3f, 0f, -0.35f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(0.4f, 0f, -0.3f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(0.35f, 0.08f, -0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(0.5f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(0.5f, 0.1f, -0.5f), Vector3.up, new Vector2(1, 0)));
                    #endregion
                }

                else if (CheckSameType(neighbors, 0, false) && CheckSameType(neighbors, 2, true)
                    && CheckSameType(neighbors, 4, false) && CheckSameType(neighbors, 6, false))
                {
                    meshBuilder.AddQuad(
                    meshBuilder.AddVertex(new Vector3(-0.5f, 0f, -0.5f), Vector3.up, new Vector2(0, 0)),
                    meshBuilder.AddVertex(new Vector3(-0.5f, 0f, 0.5f), Vector3.up, new Vector2(0, 1)),
                    meshBuilder.AddVertex(new Vector3(0.5f, 0f, 0.5f), Vector3.up, new Vector2(1, 1)),
                    meshBuilder.AddVertex(new Vector3(0.5f, 0f, -0.5f), Vector3.up, new Vector2(1, 0)));

                    #region Left Corner
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(-0.35f, 0.08f, -0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(-0.40f, 0.08f, -0.4f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.3f, 0f, -0.35f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.25f, 0f, -0.5f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(-0.5f, 0.08f, -0.4f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(-0.5f, 0f, -0.3f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.4f, 0f, -0.3f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(-0.5f, 0.1f, -0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(-0.5f, 0.08f, -0.4f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.35f, 0.08f, -0.5f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(-0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(-0.4f, 0f, -0.3f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.3f, 0f, -0.35f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(-0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 0)));
                    #endregion
                    #region Right corner
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(0.25f, 0f, -0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(0.3f, 0f, -0.35f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(0.35f, 0.08f, -0.5f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(0.4f, 0f, -0.3f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(0.5f, 0f, -0.3f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(0.5f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(0.3f, 0f, -0.35f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(0.4f, 0f, -0.3f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(new Vector3(0.35f, 0.08f, -0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(new Vector3(0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(new Vector3(0.5f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(new Vector3(0.5f, 0.1f, -0.5f), Vector3.up, new Vector2(1, 0)));
                    #endregion
                }

                else if (CheckSameType(neighbors, 0, false) && CheckSameType(neighbors, 1, true) 
                    && CheckSameType(neighbors, 2, false) && CheckSameType(neighbors, 4, false) 
                    && CheckSameType(neighbors, 6, false) && CheckSameType(neighbors, 7, true))
                {
                    // Define rotation angle in degrees
                    float angle = 270f;

                    // Define rotation axis (in this case, y-axis)
                    Vector3 axis = Vector3.up;

                    // Define rotation matrix
                    Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.AngleAxis(angle, axis));

                    meshBuilder.AddQuad(
                    meshBuilder.AddVertex(new Vector3(-0.5f, 0f, -0.5f), Vector3.up, new Vector2(0, 0)),
                    meshBuilder.AddVertex(new Vector3(-0.5f, 0f, 0.5f), Vector3.up, new Vector2(0, 1)),
                    meshBuilder.AddVertex(new Vector3(0.5f, 0f, 0.5f), Vector3.up, new Vector2(1, 1)),
                    meshBuilder.AddVertex(new Vector3(0.5f, 0f, -0.5f), Vector3.up, new Vector2(1, 0)));

                    #region Left Corner
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.35f, 0.08f, -0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.40f, 0.08f, -0.4f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.3f, 0f, -0.35f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.25f, 0f, -0.5f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.5f, 0.08f, -0.4f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.5f, 0f, -0.3f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.4f, 0f, -0.3f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.5f, 0.1f, -0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.5f, 0.08f, -0.4f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.35f, 0.08f, -0.5f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.4f, 0f, -0.3f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.3f, 0f, -0.35f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(-0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 0)));
                    #endregion
                    #region Right corner
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.25f, 0f, -0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.3f, 0f, -0.35f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.35f, 0.08f, -0.5f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.4f, 0f, -0.3f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.5f, 0f, -0.3f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.5f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.3f, 0f, -0.35f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.4f, 0f, -0.3f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 0)));
                    meshBuilder.AddQuad(
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.35f, 0.08f, -0.5f), Vector3.up, new Vector2(0, 0)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.4f, 0.08f, -0.4f), Vector3.up, new Vector2(0, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.5f, 0.08f, -0.4f), Vector3.up, new Vector2(1, 1)),
                        meshBuilder.AddVertex(rotationMatrix * new Vector3(0.5f, 0.1f, -0.5f), Vector3.up, new Vector2(1, 0)));
                    #endregion
                }
            }
            else if (hasWaterNeighbor)
            {
                Debug.Log("Has water Neighbor:" + hasWaterNeighbor);
                if (CheckSameType(neighbors, 0, false) && CheckSameType(neighbors, 1, false) &&
                   CheckSameType(neighbors, 2, false) && CheckSameType(neighbors, 3, false) &&
                   CheckSameType(neighbors, 4, false) && CheckSameType(neighbors, 5, false) &&
                   CheckSameType(neighbors, 6, false) && CheckSameType(neighbors, 7, false))
                {
                    CentreExists = true;
                    meshBuilder.AddQuad(
                    meshBuilder.AddVertex(new Vector3(-0.5f, 0f, -0.5f), Vector3.up, new Vector2(0, 0)),
                    meshBuilder.AddVertex(new Vector3(-0.5f, 0f, 0.5f), Vector3.up, new Vector2(0, 1)),
                    meshBuilder.AddVertex(new Vector3(0.5f, 0f, 0.5f), Vector3.up, new Vector2(1, 1)),
                    meshBuilder.AddVertex(new Vector3(0.5f, 0f, -0.5f), Vector3.up, new Vector2(1, 0)));
                } 
            }

        }
        meshBuilder.Build(mesh);
        meshFilter.sharedMesh = mesh;
    }

    [SerializeField] private bool showGizmos = true;

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


}
