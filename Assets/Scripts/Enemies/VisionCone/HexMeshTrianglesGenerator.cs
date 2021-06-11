using System;
using System.Linq;

public interface IHexMeshTrianglesGeneratable
{
    int[] TrianglesForHexagons(int numberOfHexagonRings);
}

public class HexMeshTrianglesGenerator : IHexMeshTrianglesGeneratable
{
    private int[] innerPoints;
    private int[] outerPoints;
    private int[] innerCornerPoints;

    public int[] TrianglesForHexagons(int numberOfHexagonRings)
    {
        if (numberOfHexagonRings < 1)
            return new int[0];
        
        var triangles = new int[NumberOfIndices(numberOfHexagonRings)];

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

    private int NumberOfIndices(int numberOfHexagonRings)
    {
        return Enumerable.Range(1, numberOfHexagonRings)
            .Select(x => 36 * x - 18)
            .Sum();
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

    private int[] TrianglesForHexagon(int depthOfHex)
    {
        innerPoints = PointsForDepth(depthOfHex - 1);
        innerCornerPoints = CornerPointsForDepth(depthOfHex - 1);
        outerPoints = PointsForDepth(depthOfHex);

        int numberOfInnerEdgePoints = innerPoints.Count() - innerCornerPoints.Count();
        var triangles = new int[3 * (innerCornerPoints.Count() * 3 + numberOfInnerEdgePoints * 2)];

        return AccumulateTriangles(triangles, 0, 0, outerPoints.Count() - 1);
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
}
