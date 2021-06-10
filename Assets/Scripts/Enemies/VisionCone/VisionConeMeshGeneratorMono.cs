using System;
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

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private int[] innerPoints;
    private int[] outerPoints;
    private int[] innerCornerPoints;

    void Start()
    {
        MakeMesh();
    }

    public void MakeMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateMesh();
        UpdateMesh();
    }

    void CreateMesh()
    {
        Vector3 startPoint = visionConeMono.ControlPointsMono.PatrolPoints[0].Position - transform.position;

        int numberOfHexagons = Mathf.RoundToInt(Radius / MeshResolution);
        vertices = VerticesForHexagons(numberOfHexagons);
        triangles = TrianglesForHexagons(numberOfHexagons);
    }

    private Vector3[] VerticesForHexagons(int numberOfHexagonRings)
    {
        int numberOfVertices = 1 + Enumerable.Range(1, numberOfHexagonRings)
            .Select(x => 6 * x)
            .Sum();

        var vertices = new Vector3[numberOfVertices];
        vertices[0] = new Vector3(0, 0, 0);
        foreach (int depth in Enumerable.Range(1, numberOfHexagonRings))
            vertices = AddVerticesForDepth(vertices, depth);

        return vertices;
    }

    private Vector3[] AddVerticesForDepth(Vector3[] vertices, int depth)
    {
        Vector3[] currentVertices = VerticesForDepth(CornerVerticesForDepth(depth), depth);

        int startingIndex = 1 + Enumerable.Range(1, depth - 1)
            .Select(x => 6 * x)
            .Sum();
        int endIndex = startingIndex + currentVertices.Count();
        for (int i = startingIndex; i < endIndex; i++)
            vertices[i] = currentVertices[i - startingIndex];

        return vertices;
    }

    private Vector3[] CornerVerticesForDepth(int depth)
    {
        float outerRadius = Radius / (float)MeshResolution * (float)depth;
        float innerRadius = outerRadius * 0.866025404f;
        return new Vector3[] {
            new Vector3(0f, outerRadius, 0f),
            new Vector3(-innerRadius, 0.5f * outerRadius, 0f),
            new Vector3(-innerRadius, -0.5f * outerRadius, 0f),
            new Vector3(0f, -outerRadius, 0f),
            new Vector3(innerRadius, -0.5f * outerRadius, 0f),
            new Vector3(innerRadius, 0.5f * outerRadius, 0f),
        };
    }

    private Vector3[] VerticesForDepth(Vector3[] cornerVertices, int depth)
    {
        if (depth < 2)
            return cornerVertices;
        
        var vertices = new Vector3[cornerVertices.Count() + 6 * (depth - 1)];
        for(int cornerIndex = 0; cornerIndex < cornerVertices.Count(); cornerIndex++)
        {
            Vector3 currentCorner = cornerVertices[cornerIndex];
            Vector3 nextCorner = cornerVertices[(cornerIndex + 1) % cornerVertices.Count()];
            vertices[cornerIndex * depth] = currentCorner;
            for (int i = 1; i < depth; i++)
                vertices[cornerIndex * depth + i] = currentCorner + i / (float)depth * (nextCorner - currentCorner);
        }

        return vertices;
    }

    private int[] TrianglesForHexagons(int numberOfHexagonRings)
    {
        int[] indicesPerDepth = NumberOfIndicesPerDepth(numberOfHexagonRings);
        var triangles = new int[indicesPerDepth.Sum()];

        int trianglesStartIndex = 0;
        foreach (int depth in Enumerable.Range(1, numberOfHexagonRings))
        {
            int[] currentTriangles;
            if (depth == 1)
                currentTriangles = TrianglesInnerMostHexagon();
            else
                currentTriangles = TrianglesForHexagon(depth);
            
            for (int i = 0; i < currentTriangles.Count(); i++)
                triangles[trianglesStartIndex + i] = currentTriangles[i];
            
            trianglesStartIndex += currentTriangles.Count();
        }
        
        return triangles;
    }

    private int[] NumberOfIndicesPerDepth(int numberOfHexagonRings)
    {
        return Enumerable.Range(1, numberOfHexagonRings)
            .Select(x => 36 * x - 18)
            .ToArray();
    }

    private int[] TrianglesForHexagon(int depthOfRing)
    {
        if (depthOfRing < 2)
            throw new ArgumentException("Triangle calculations only supported with depth >= 2.");

        innerPoints = PointsForDepth(depthOfRing - 1);
        innerCornerPoints = CornerPointsForDepth(depthOfRing - 1);
        outerPoints = PointsForDepth(depthOfRing);

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

    private int[] CornerPointsForDepth(int depthOfRing)
    {
        int[] cornerPoints = new int[6];
        for (int i = 0; i < cornerPoints.Count(); i++)
            cornerPoints[i] = 6 * (depthOfRing * (depthOfRing - 1) / 2) + 1 + depthOfRing * i;
        
        return cornerPoints;
    }

    private static int[] TrianglesInnerMostHexagon()
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
