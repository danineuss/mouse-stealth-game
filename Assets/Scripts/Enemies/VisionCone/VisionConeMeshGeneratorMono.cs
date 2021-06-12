using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class VisionConeMeshGeneratorMono : MonoBehaviour
{
    [Range(0.1f, 20)] public float Radius;
    [Range(2, 100)] public int MeshResolution;

    [SerializeField] private VisionConeMono visionConeMono = null;
    [SerializeField] private Mesh mesh;
    [SerializeField] private Vector3[] vertices;
    [SerializeField] private int[] triangles;

    private IHexMeshVerticesGeneratable hexVerticesGenerator;
    private IHexMeshTrianglesGeneratable hexTrianglesGenerator;

    void Start()
    {
        hexVerticesGenerator = new HexMeshVerticesGenerator(MeshResolution);
        hexTrianglesGenerator = new HexMeshTrianglesGenerator();

        GenerateMesh();
    }

    public void GenerateMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateVerticesTriangles();
        UpdateMesh();
    }

    void CreateVerticesTriangles()
    {
        Vector3 startPoint = visionConeMono.ControlPointsMono.PatrolPoints[0].Position - transform.position;

        int numberOfHexagons = Mathf.RoundToInt(Radius / MeshResolution);
        vertices = hexVerticesGenerator.HexagonVerticesForRadius(Radius)
            .ToList()
            .Select(x => x + startPoint)
            .ToArray();
        triangles = hexTrianglesGenerator.TrianglesForHexagons(numberOfHexagons);
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}
