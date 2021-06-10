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
    private int[] innerPoints;
    private int[] outerPoints;
    private int[] innerCornerPoints;

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
        triangles.AddRange(TrianglesInnerMostHexagon());

        for (int i = 2; i <= numberOfRings; i++)
            triangles.AddRange(TrianglesFromHexagon(i));
        
        return triangles.ToArray();
    }

    int[] TrianglesFromHexagon(int depthOfRing)
    {
        if (depthOfRing < 2)
            throw new ArgumentException("Triangle calculations only supported with depth >= 2.");

        innerPoints = PointsForDepth(depthOfRing - 1);
        outerPoints = PointsForDepth(depthOfRing);
        innerCornerPoints = InnerCornerPointsForDepth(depthOfRing);

        int numberOfEdgePoints = innerPoints.Count() - innerCornerPoints.Count();
        var triangles = new int[3 * (innerCornerPoints.Count() * 3 + numberOfEdgePoints * 2)];
        return AccumulateTriangles(triangles, 0, 0, outerPoints.Count() - 1);
    }

    private int[] AccumulateTriangles(
        int[] triangles, int trianglesIndex, int innerIndex, int outerIndex)
    {
        if (innerIndex == innerPoints.Count())
            return triangles;
        
        if (innerCornerPoints.Contains(innerPoints[innerIndex]))
            return AccumulateTrianglesAtCorners(triangles, trianglesIndex, innerIndex, outerIndex);
        else
            return AccumulateTrianglesAtEdges(triangles, trianglesIndex, innerIndex, outerIndex);
    }

    private int[] AccumulateTrianglesAtCorners(int[] triangles, int trianglesIndex, int innerIndex, int outerIndex)
    {
        triangles[trianglesIndex++] = innerPoints[innerIndex];
        triangles[trianglesIndex++] = outerPoints[outerIndex];
        triangles[trianglesIndex++] = outerPoints[(outerIndex + 1) % outerPoints.Count()];
        triangles[trianglesIndex++] = innerPoints[innerIndex];
        triangles[trianglesIndex++] = outerPoints[(outerIndex + 1) % outerPoints.Count()];
        triangles[trianglesIndex++] = outerPoints[(outerIndex + 2) % outerPoints.Count()];
        triangles[trianglesIndex++] = innerPoints[innerIndex];
        triangles[trianglesIndex++] = outerPoints[(outerIndex + 2) % outerPoints.Count()];
        triangles[trianglesIndex++] = innerPoints[(innerIndex + 1) % innerPoints.Count()];

        return AccumulateTriangles(triangles, trianglesIndex, innerIndex + 1, (outerIndex + 2) % outerPoints.Count());
    }

    private int[] AccumulateTrianglesAtEdges(int[] triangles, int trianglesIndex, int innerIndex, int outerIndex)
    {
        triangles[trianglesIndex++] = innerPoints[innerIndex];
        triangles[trianglesIndex++] = outerPoints[outerIndex];
        triangles[trianglesIndex++] = outerPoints[(outerIndex + 1) % outerPoints.Count()];
        triangles[trianglesIndex++] = innerPoints[innerIndex];
        triangles[trianglesIndex++] = outerPoints[(outerIndex + 1) % outerPoints.Count()];
        triangles[trianglesIndex++] = innerPoints[(innerIndex + 1) % innerPoints.Count()];

        return AccumulateTriangles(triangles, trianglesIndex, innerIndex + 1, (outerIndex + 1) % outerPoints.Count());
    }

    private int[] PointsForDepth(int depthOfRing)
    {
        int[] outerPoints = new int[6 * depthOfRing];
        for (int i = 0; i < outerPoints.Count(); i++)
            outerPoints[i] = 6 * (depthOfRing * (depthOfRing - 1) / 2) + 1 + i;
        
        return outerPoints;
    }

    private int[] InnerCornerPointsForDepth(int depthOfRing)
    {
        int[] cornerPoints = new int[6];
        for (int i = 0; i < cornerPoints.Count(); i++)
            cornerPoints[i] = 6 * ((depthOfRing - 1) * (depthOfRing - 2) / 2) + 1 + (depthOfRing - 1) * i;
        
        return cornerPoints;
    }

    private static IEnumerable<int> TrianglesInnerMostHexagon()
    {
        return new int[]
        {
            0, 1, 2,
            0, 2, 3,
            0, 3, 4,
            0, 4, 5,
            0, 5, 6, 
            0, 6, 1
        };
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}
