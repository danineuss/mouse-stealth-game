using System.Linq;
using UnityEngine;

public interface IHexMeshVerticesGeneratable
{
    Vector3[] GenerateHexagonVertices(float fieldOfViewRad, float radius, int numberOfHexagonRings);   
}

public partial class HexMeshVerticesGenerator : IHexMeshVerticesGeneratable
{
    private Vector3[] vertices;
    private int verticesIndex;
    private readonly float RatioInnerToOuterRadius = 0.866025404f;

    public Vector3[] GenerateHexagonVertices(
        float fieldOfViewRad, float radius, int numberOfHexagonRings)
    {
        if (fieldOfViewRad <= 0f || radius <= 0 || numberOfHexagonRings < 1)
            return new Vector3[0];
        
        vertices = new Vector3[NumberOfVertices(numberOfHexagonRings)];
        verticesIndex = 0;

        HexagonalVertices(fieldOfViewRad, radius, numberOfHexagonRings);
        CircularVertices(fieldOfViewRad, radius, numberOfHexagonRings);

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

    private void HexagonalVertices(float fieldOfViewRad, float radius, int numberOfHexagonRings)
    {
        vertices[verticesIndex++] = new Vector3(0, 0, radius);
        foreach (int depthOfHex in Enumerable.Range(1, numberOfHexagonRings))
        {
            var anglesAtDepth = AnglesAtDepth(fieldOfViewRad, depthOfHex, numberOfHexagonRings);
            foreach (var sphereAngle in anglesAtDepth)
                vertices[verticesIndex++] = sphereAngle.Vertice(radius);
        }
    }

    private SphereAngles[] AnglesAtDepth(
        float fieldOfViewRad, int depthOfHex, int numberOfHexagonRings)
    {
        var cornerAngles = CornerAnglesForDepth(fieldOfViewRad, depthOfHex, numberOfHexagonRings);
        if (depthOfHex < 2)
            return cornerAngles;
        
        return InsertEdgeAngles(cornerAngles, depthOfHex);
    }

    private SphereAngles[] CornerAnglesForDepth(
        float fieldOfViewRad, int depthOfHex, int numberOfHexagonRings)
    {
        float outerDeltaAlpha = depthOfHex / (float)numberOfHexagonRings * fieldOfViewRad / 2f;
        float innerDeltaAlpha = outerDeltaAlpha * RatioInnerToOuterRadius;
        return new SphereAngles[] {
            new SphereAngles(-outerDeltaAlpha, 0f),
            new SphereAngles(-outerDeltaAlpha / 2, -innerDeltaAlpha),
            new SphereAngles(outerDeltaAlpha / 2, -innerDeltaAlpha),
            new SphereAngles(outerDeltaAlpha, 0f),
            new SphereAngles(outerDeltaAlpha / 2, innerDeltaAlpha),
            new SphereAngles(-outerDeltaAlpha / 2, innerDeltaAlpha)
        };
    }

    private SphereAngles[] InsertEdgeAngles(SphereAngles[] cornerAngles, int depth)
    {
        var angles = new SphereAngles[cornerAngles.Count() + 6 * (depth - 1)];
        for(int cornerIndex = 0; cornerIndex < cornerAngles.Count(); cornerIndex++)
        {
            SphereAngles currentCorner = cornerAngles[cornerIndex];
            SphereAngles nextCorner = cornerAngles[(cornerIndex + 1) % cornerAngles.Count()];
            for (int i = 0; i < depth; i++)
                angles[cornerIndex * depth + i] = 
                    currentCorner + (i / (float)depth) * (nextCorner - currentCorner);
        }
        return angles;
    }

    private void CircularVertices(float fieldOfViewRad, float radius, int numberOfHexagonRings)
    {
        float radianBetweenPoints = 2 * Mathf.PI / (float)(6 * (1 + numberOfHexagonRings));
        Vector3 cornerPointInPlaneZ = CornerAnglesForDepth(
            fieldOfViewRad, numberOfHexagonRings, numberOfHexagonRings).First().Vertice(radius);
        float radiusInPlaneZ = Mathf.Abs(cornerPointInPlaneZ.y);
        for (int hexOffset = 0; hexOffset < 6; hexOffset++)
        {
            var currentAngle = hexOffset * Mathf.PI / 3f;
            for (int i = 0; i < numberOfHexagonRings; i++)
            {
                currentAngle += radianBetweenPoints;
                var a = new Vector3(
                    -radiusInPlaneZ * Mathf.Sin(currentAngle),
                    radiusInPlaneZ * Mathf.Cos(currentAngle),
                    cornerPointInPlaneZ.z
                );
                vertices[verticesIndex++] = a;
            }
        }
    }
}
