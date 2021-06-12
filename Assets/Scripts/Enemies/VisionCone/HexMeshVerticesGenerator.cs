using System.Linq;
using UnityEngine;

public interface IHexMeshVerticesGeneratable
{
    Vector3[] HexagonVerticesForRadius(float radius);   
}

public class HexMeshVerticesGenerator : IHexMeshVerticesGeneratable
{
    private int meshResolution;
    private readonly float RatioInnerToOuterRadius = 0.866025404f;

    public HexMeshVerticesGenerator(int meshResolution)
    {
        this.meshResolution = meshResolution;    
    }

    public Vector3[] HexagonVerticesForRadius(float radius)
    {
        if (radius < 0f)
            return new Vector3[0];

        int numberOfHexagonRings = Mathf.RoundToInt(radius / meshResolution);
        if (numberOfHexagonRings == 0)
            numberOfHexagonRings = 1;

        var vertices = new Vector3[NumberOfVertices(numberOfHexagonRings)];
        vertices[0] = new Vector3(0, 0, 0);
        foreach (int depthOfHex in Enumerable.Range(1, numberOfHexagonRings))
            vertices = InsertVerticesForDepth(vertices, depthOfHex, radius);

        return vertices;
    }

    private int NumberOfVertices(int numberOfHexagonRings)
    {
        return 1 + Enumerable.Range(1, numberOfHexagonRings)
            .Select(x => 6 * x)
            .Sum();
    }

    private Vector3[] InsertVerticesForDepth(Vector3[] vertices, int depthOfHex, float radius)
    {
        Vector3[] currentVertices = VerticesForDepth(depthOfHex, radius);

        int startingIndex = 1 + Enumerable.Range(1, depthOfHex - 1)
            .Select(x => 6 * x)
            .Sum();
        int endIndex = startingIndex + currentVertices.Count();
        for (int i = startingIndex; i < endIndex; i++)
            vertices[i] = currentVertices[i - startingIndex];

        return vertices;
    }

    private Vector3[] VerticesForDepth(int depthOfHex, float radius)
    {
        var cornerVertices = CornerVerticesForDepth(depthOfHex, radius);
        if (depthOfHex < 2)
            return cornerVertices;
        
        return InsertEdgeVertices(cornerVertices, depthOfHex);
    }

    private Vector3[] CornerVerticesForDepth(int depthOfHex, float radius)
    {
        float outerRadius = radius / (float)meshResolution * (float)depthOfHex;
        float innerRadius = outerRadius * RatioInnerToOuterRadius;
        return new Vector3[] {
            new Vector3(0f, outerRadius, 0f),
            new Vector3(-innerRadius, 0.5f * outerRadius, 0f),
            new Vector3(-innerRadius, -0.5f * outerRadius, 0f),
            new Vector3(0f, -outerRadius, 0f),
            new Vector3(innerRadius, -0.5f * outerRadius, 0f),
            new Vector3(innerRadius, 0.5f * outerRadius, 0f)
        };
    }

    private Vector3[] InsertEdgeVertices(Vector3[] cornerVertices, int depth)
    {
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
}
