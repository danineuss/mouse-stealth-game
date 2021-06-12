using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class VisionConeMeshGeneratorMono : MonoBehaviour
{
    [Range(0.1f, 20)] public float Radius;
    [Range(0.1f, 10)] public float MeshResolution;

    [SerializeField] private VisionConeMono visionConeMono = null;
    [SerializeField] private Mesh mesh;
    [SerializeField] private Vector3[] vertices;
    [SerializeField] private int[] triangles;

    private IHexMeshVerticesGeneratable hexVerticesGenerator;
    private IHexMeshTrianglesGeneratable hexTrianglesGenerator;

    void Start()
    {
        GenerateMesh();
    }

    public void GenerateMesh()
    {

        InitializeMeshGenerators();
        CreateVerticesTriangles();
        UpdateMesh();
    }

    private void InitializeMeshGenerators()
    {        
        hexVerticesGenerator = new HexMeshVerticesGenerator();
        hexTrianglesGenerator = new HexMeshTrianglesGenerator();
    }

    void CreateVerticesTriangles()
    {
        Vector3 startPoint = visionConeMono.ControlPointsMono.PatrolPoints[0].Position - transform.position;

        int numberOfHexagonRings = Mathf.RoundToInt(Radius * MeshResolution);
        numberOfHexagonRings = numberOfHexagonRings < 1 ? 1 : numberOfHexagonRings;

        vertices = hexVerticesGenerator.HexagonVerticesForRadius(Radius, numberOfHexagonRings)
            .ToList()
            .Select(x => x + startPoint)
            .ToArray();
        triangles = hexTrianglesGenerator.TrianglesForHexagons(numberOfHexagonRings);
    }

    void UpdateMesh()
    {
        mesh = new Mesh();
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;        
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.RecalculateNormals();
    }
}
