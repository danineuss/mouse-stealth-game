using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class VisionConeMeshGeneratorMono : MonoBehaviour
{
    [SerializeField] private VisionConeMono visionConeMono = null;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        MakeMesh();
    }

    public void MakeMesh()
    {
        CreateMesh();
        UpdateMesh();

        var a = TrianglesFromHexagons(4);
    }

    void CreateMesh()
    {
        Vector3 startPoint = visionConeMono.ControlPointsMono.PatrolPoints[0].Position - transform.position;

        vertices = new Vector3[]
        {
            new Vector3 (0, 0, 0),
            new Vector3 (0, 1, 0),
            new Vector3 (1, 0, 0)
        }
            .Select(x => x + startPoint)
            .ToArray();

        triangles = new int[]
        {
            0, 1, 2
        };
    }

    int[] TrianglesFromHexagons(int numberOfRings)
    {
        List<int> triangles = new List<int>();
        for (int i = 2; i <= numberOfRings; i++)
        {
            triangles.AddRange(TrianglesFromHexagon(i));
        }
        return triangles.ToArray();
    }

    int[] TrianglesFromHexagon(int depthOfRing)
    {
        if (depthOfRing < 2)
            throw new ArgumentException();

        int[] innerPoints = PointsForDepth(depthOfRing - 1);
        int[] outerPoints = PointsForDepth(depthOfRing);
        int[] cornerPoints = CornerPointsForDepth(depthOfRing);

        return new int[1]{1};
    }

    private int[] PointsForDepth(int depthOfRing)
    {
        int[] outerPoints = new int[6 * depthOfRing];
        for (int i = 0; i < outerPoints.Count(); i++)
            outerPoints[i] = 6 * (depthOfRing * (depthOfRing - 1) / 2) + 1 + i;
        
        return outerPoints;
    }

    private int[] CornerPointsForDepth(int depthOfRing)
    {
        int[] cornerPoints = new int[6];
        for (int i = 0; i < cornerPoints.Count(); i++)
            cornerPoints[i] = 6 * (depthOfRing * (depthOfRing - 1) / 2) + 1 + depthOfRing * i;
        
        return cornerPoints;
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}
