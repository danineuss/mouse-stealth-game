using System.Collections.Generic;
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

        InitializeVertices(numberOfHexagonRings);
        HexagonalVertices(fieldOfViewRad, radius, numberOfHexagonRings);
        CircularVertices(fieldOfViewRad, radius, numberOfHexagonRings);

        return vertices;
    }

    private void InitializeVertices(int numberOfHexagonRings)
    {
        verticesIndex = 0;
        int numberOfVertices = NumberOfVertices(numberOfHexagonRings);
        if (vertices == null || vertices.Count() != numberOfVertices)
            vertices = new Vector3[numberOfVertices];
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
        var angles = new SphereAngles[6 + 6 * (depthOfHex - 1)];
        CornerAnglesForDepth(angles, fieldOfViewRad, depthOfHex, numberOfHexagonRings);
        if (depthOfHex > 1)
            InsertEdgeAngles(angles, depthOfHex);

        return angles;
    }

    private void CornerAnglesForDepth(
        SphereAngles[] angles, float fieldOfViewRad, int depthOfHex, int numberOfHexagonRings)
    {
        float outerDeltaAlpha = depthOfHex / (float)numberOfHexagonRings * fieldOfViewRad / 2f;
        float innerDeltaAlpha = outerDeltaAlpha * RatioInnerToOuterRadius;

        int i = 0;
        new List<SphereAngles>() {
            new SphereAngles(-outerDeltaAlpha, 0f),
            new SphereAngles(-outerDeltaAlpha / 2, -innerDeltaAlpha),
            new SphereAngles(outerDeltaAlpha / 2, -innerDeltaAlpha),
            new SphereAngles(outerDeltaAlpha, 0f),
            new SphereAngles(outerDeltaAlpha / 2, innerDeltaAlpha),
            new SphereAngles(-outerDeltaAlpha / 2, innerDeltaAlpha)
        }
            .ForEach(x => angles[depthOfHex * i++] = x);
    }

    private void InsertEdgeAngles(SphereAngles[] angles, int depth)
    {
        for (int cornerIndex = 0; cornerIndex < 6; cornerIndex++)
        {
            SphereAngles currentCorner = angles[cornerIndex * depth];
            SphereAngles nextCorner = angles[((cornerIndex + 1) * depth) % angles.Count()];
            for (int i = 0; i < depth; i++)
                angles[cornerIndex * depth + i] =
                    currentCorner + (i / (float)depth) * (nextCorner - currentCorner);
        }
    }

    private void CircularVertices(float fieldOfViewRad, float radius, int numberOfHexagonRings)
    {
        Vector3 cornerPointInPlaneZ = new SphereAngles(-fieldOfViewRad / 2f, 0f).Vertice(radius);
        float radiusInPlaneZ = Mathf.Abs(cornerPointInPlaneZ.y);
        float radianBetweenPoints = 2 * Mathf.PI / (float)(6 * (1 + numberOfHexagonRings));
        for (int hexOffset = 0; hexOffset < 6; hexOffset++)
        {
            var currentAngle = hexOffset * Mathf.PI / 3f;
            for (int i = 0; i < numberOfHexagonRings; i++)
            {
                currentAngle += radianBetweenPoints;
                vertices[verticesIndex++] = new Vector3(
                    -radiusInPlaneZ * Mathf.Sin(currentAngle),
                    radiusInPlaneZ * Mathf.Cos(currentAngle),
                    cornerPointInPlaneZ.z
                );
            }
        }
    }
}
