using System.Linq;
using UnityEngine;

public interface IHexMeshVerticesGeneratable
{
    Vector3[] HexagonVerticesForRadius(float radius, int numberOfHexagonRings);   
}

public class HexMeshVerticesGenerator : IHexMeshVerticesGeneratable
{
    private Vector3[] vertices;
    private int verticesIndex;
    private readonly float RatioInnerToOuterRadius = 0.866025404f;

    public Vector3[] HexagonVerticesForRadius(float radius, int numberOfHexagonRings)
    {
        if (radius < 0f)
            return new Vector3[0];
        
        vertices = new Vector3[NumberOfVertices(numberOfHexagonRings)];
        verticesIndex = 0;

        HexagonalVertices(radius, numberOfHexagonRings);
        CircularVertices(radius, numberOfHexagonRings);

        return vertices;
    }

    private int NumberOfVertices(int numberOfHexagonRings)
    {
        var numberOfHexagonVertices = 1 + Enumerable.Range(1, numberOfHexagonRings)
            .Select(x => 6 * x)
            .Sum();
        var numberOfCircleVertices = 6 * numberOfHexagonRings;
        return numberOfHexagonVertices + numberOfCircleVertices;
    }

    private void HexagonalVertices(float radius, int numberOfHexagonRings)
    {
        vertices[verticesIndex++] = new Vector3(0, 0, 0);
        foreach (int depthOfHex in Enumerable.Range(1, numberOfHexagonRings))
        {
            Vector3[] verticesAtDepth = VerticesAtDepth(depthOfHex, numberOfHexagonRings, radius);

            foreach (var vertice in verticesAtDepth)
                vertices[verticesIndex++] = vertice;
        }
    }

    private Vector3[] VerticesAtDepth(int depthOfHex, int numberOfHexagonRings, float radius)
    {
        var cornerVertices = CornerVerticesForDepth(depthOfHex, numberOfHexagonRings, radius);
        if (depthOfHex < 2)
            return cornerVertices;
        
        return InsertEdgeVertices(cornerVertices, depthOfHex);
    }

    private Vector3[] CornerVerticesForDepth(int depthOfHex, int numberOfHexagonRings, float radius)
    {
        float outerRadius = depthOfHex / numberOfHexagonRings * radius;
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

    private void CircularVertices(float radius, int numberOfHexagonRings)
    {
        float radianBetweenPoints = 2 * Mathf.PI / (6 * (1 + numberOfHexagonRings));
        for (int hexOffset = 0; hexOffset < 6; hexOffset++)
        {
            var currentAngle = radianBetweenPoints + hexOffset * Mathf.PI / 3;
            for (int i = 0; i < numberOfHexagonRings; i++)
            {
                vertices[verticesIndex++] = radius * new Vector3(-Mathf.Sin(currentAngle), Mathf.Cos(currentAngle), 0f);
                currentAngle += radianBetweenPoints;
            }
        }
    }
}
