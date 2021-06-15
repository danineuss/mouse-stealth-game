using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class VisionConeMeshGeneratorMono : MonoBehaviour
{
    [Range(5f, 90f)] public float FieldOfViewDeg;
    [Range(0.01f, 0.5f)] public float MeshResolutionDeg;
    public VisionConeMono visionConeMono = null;
    
    private Vector3[] vertices;
    private int[] triangles;
    public Mesh mesh;
    private MeshFilter meshFilter;
    private IHexMeshVerticesGeneratable hexVerticesGenerator;
    private IHexMeshTrianglesGeneratable hexTrianglesGenerator;

    void Start()
    {
        InitializeComponents();
        GenerateMesh();
    }

    void Update()
    {
        // GenerateMesh();
    }

    public void GenerateMesh()
    {
        CreateVerticesTriangles();
        UpdateMesh();
    }

    private void InitializeComponents()
    {        
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        hexVerticesGenerator = new HexMeshVerticesGenerator();
        hexTrianglesGenerator = new HexMeshTrianglesGenerator();
    }

    void CreateVerticesTriangles()
    {
        float radius = (visionConeMono.ControlPointsMono.PatrolPoints[0].Position
            - transform.position).magnitude;
        int numberOfHexagonRings = Mathf.RoundToInt(FieldOfViewDeg * MeshResolutionDeg);
        numberOfHexagonRings = numberOfHexagonRings < 1 ? 1 : numberOfHexagonRings;

        vertices = hexVerticesGenerator.GenerateHexagonVertices(
            FieldOfViewDeg * Mathf.Deg2Rad, radius, numberOfHexagonRings);
        triangles = hexTrianglesGenerator.TrianglesForHexagons(numberOfHexagonRings);
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;        
        meshFilter.mesh = mesh;

        mesh.RecalculateNormals();
    }
}
