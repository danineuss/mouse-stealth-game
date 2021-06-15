using System;
using System.Collections.Generic;
using System.Linq;

public interface IHexMeshTrianglesGeneratable
{
    int[] TrianglesForHexagons(int numberOfHexagonRings);
}

public class HexMeshTrianglesGenerator : IHexMeshTrianglesGeneratable
{
    private int[] triangles;
    private int trianglesIndex;
    private int[] innerPoints;
    private int[] outerPoints;
    private int[] innerCornerPoints;

    public int[] TrianglesForHexagons(int numberOfHexagonRings)
    {
        if (numberOfHexagonRings < 1)
            return new int[0];

        triangles = new int[NumberOfIndices(numberOfHexagonRings)];    
        trianglesIndex = 0;

        HexagonalTriangles(numberOfHexagonRings);
        OutermostCircularTriangles(numberOfHexagonRings);

        return triangles;
    }

    private int NumberOfIndices(int numberOfHexagonRings)
    {
        var numberOfHexagonalTrianges = Enumerable.Range(1, numberOfHexagonRings)
            .Select(x => 36 * x - 18)
            .Sum();
        var numberOfOutermostCircularTriangles = 18 * (1 + (numberOfHexagonRings - 1) * 2);
        return numberOfHexagonalTrianges + numberOfOutermostCircularTriangles;
    }

    private void HexagonalTriangles(int numberOfHexagonRings)
    {
        foreach (int depth in Enumerable.Range(1, numberOfHexagonRings))
        {
            FindInnerOuterAndCornerPoints(depth);

            if (depth == 1)
                TrianglesInnerMostHexagon();
            else
                AccumulateTriangles(0, outerPoints.Count() - 1);
        }
    }

    private void FindInnerOuterAndCornerPoints(int depthOfHex)
    {
        innerPoints = PointsForDepth(depthOfHex - 1);
        innerCornerPoints = CornerPointsForDepth(depthOfHex - 1);
        outerPoints = PointsForDepth(depthOfHex);
    }

    private int[] PointsForDepth(int depthOfHex)
    {
        int[] outerPoints = new int[6 * depthOfHex];
        for (int i = 0; i < outerPoints.Count(); i++)
            outerPoints[i] = 1 + 6 * (depthOfHex * (depthOfHex - 1) / 2) + i;
        
        return outerPoints;
    }

    private int[] CornerPointsForDepth(int depthOfHex)
    {
        int[] cornerPoints = new int[6];
        for (int i = 0; i < cornerPoints.Count(); i++)
            cornerPoints[i] = 1 + 6 * (depthOfHex * (depthOfHex - 1) / 2) + depthOfHex * i;
        
        return cornerPoints;
    }

    private void TrianglesInnerMostHexagon()
    {
        new List<int>()
        {
            0, 1, 2,
            0, 2, 3,
            0, 3, 4,
            0, 4, 5,
            0, 5, 6, 
            0, 6, 1
        }
            .ForEach(x => triangles[trianglesIndex++] = x);
    }

    private void AccumulateTriangles(int innerIndex, int outerIndex)
    {
        if (innerIndex == innerPoints.Count())
            return;
        
        if (innerCornerPoints.Contains(innerPoints[innerIndex]))
            AccumulateTrianglesAtCorners(innerIndex, outerIndex);
        else
            AccumulateTrianglesAtEdges(innerIndex, outerIndex);
    }

    private void AccumulateTrianglesAtCorners(int innerIndex, int outerIndex)
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

        AccumulateTriangles(innerIndex + 1, (outerIndex + 2) % outerPoints.Count());
    }

    private void AccumulateTrianglesAtEdges(int innerIndex, int outerIndex)
    {
        triangles[trianglesIndex++] = innerPoints[innerIndex];
        triangles[trianglesIndex++] = outerPoints[outerIndex];
        triangles[trianglesIndex++] = outerPoints[(outerIndex + 1) % outerPoints.Count()];
        triangles[trianglesIndex++] = innerPoints[innerIndex];
        triangles[trianglesIndex++] = outerPoints[(outerIndex + 1) % outerPoints.Count()];
        triangles[trianglesIndex++] = innerPoints[(innerIndex + 1) % innerPoints.Count()];

        AccumulateTriangles(innerIndex + 1, (outerIndex + 1) % outerPoints.Count());
    }

    private void OutermostCircularTriangles(int numberOfHexagonRings)
    {
        var outerCornerPoints = CornerPointsForDepth(numberOfHexagonRings);
        var pointsOnCircle = 
            Enumerable.Range(outerPoints.Last() + 1, 6 * numberOfHexagonRings).ToArray();

        for (int outerIndex = 0; outerIndex < outerPoints.Count(); outerIndex++)
        {
            if (outerCornerPoints.Contains(outerPoints[outerIndex]))
                AddCircularTrianglesAtCorner(pointsOnCircle, outerIndex);
            else
                AddCircularTrianglesOnEdge(pointsOnCircle, outerIndex);
        }
    }

    private void AddCircularTrianglesAtCorner(int[] pointsOnCircle, int outerIndex)
    {
        triangles[trianglesIndex++] = outerPoints[outerIndex];
        triangles[trianglesIndex++] = pointsOnCircle[outerIndex];
        triangles[trianglesIndex++] = outerPoints[(outerIndex + 1) % outerPoints.Count()];
    }

    private void AddCircularTrianglesOnEdge(int[] pointsOnCircle, int outerIndex)
    {
        triangles[trianglesIndex++] = outerPoints[outerIndex];
        triangles[trianglesIndex++] = pointsOnCircle[outerIndex - 1];
        triangles[trianglesIndex++] = pointsOnCircle[outerIndex];
        triangles[trianglesIndex++] = outerPoints[outerIndex];
        triangles[trianglesIndex++] = pointsOnCircle[outerIndex];
        triangles[trianglesIndex++] = outerPoints[(outerIndex + 1) % outerPoints.Count()];
    }
}
