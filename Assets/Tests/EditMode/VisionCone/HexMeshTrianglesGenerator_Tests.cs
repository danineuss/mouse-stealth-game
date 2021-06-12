using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class HexMeshTrianglesGenerator_Tests
    {
        private IHexMeshTrianglesGeneratable hexTrianglesGenerator = new HexMeshTrianglesGenerator();
        
        [Test]
        public void should_generate_inner_most_hexagon_with_correct_triangles()
        {
            var innerMostHexTrianges = hexTrianglesGenerator.TrianglesForHexagons(1);

            Assert.AreEqual(36, innerMostHexTrianges.Length);
            Assert.True(ContainsSequenceInArray(innerMostHexTrianges, new int[]{0, 1, 2}));
            Assert.True(ContainsSequenceInArray(innerMostHexTrianges, new int[]{0, 2, 3}));
            Assert.True(ContainsSequenceInArray(innerMostHexTrianges, new int[]{0, 3, 4}));
            Assert.True(ContainsSequenceInArray(innerMostHexTrianges, new int[]{0, 4, 5}));
            Assert.True(ContainsSequenceInArray(innerMostHexTrianges, new int[]{0, 5, 6}));
            Assert.True(ContainsSequenceInArray(innerMostHexTrianges, new int[]{0, 6, 1}));

            Assert.True(ContainsSequenceInArray(innerMostHexTrianges, new int[]{1, 7, 2}));
            Assert.True(ContainsSequenceInArray(innerMostHexTrianges, new int[]{2, 8, 3}));
            Assert.True(ContainsSequenceInArray(innerMostHexTrianges, new int[]{3, 9, 4}));
            Assert.True(ContainsSequenceInArray(innerMostHexTrianges, new int[]{4, 10, 5}));
            Assert.True(ContainsSequenceInArray(innerMostHexTrianges, new int[]{5, 11, 6}));
            Assert.True(ContainsSequenceInArray(innerMostHexTrianges, new int[]{6, 12, 1}));
        }

        [Test]
        public void should_contain_correct_triangles_for_larger_hexagon()
        {
            var largerHexagonTriangles = hexTrianglesGenerator.TrianglesForHexagons(3);
            var expectedNumberOfTriangles = 162 + 18 * 5;

            Assert.AreEqual(expectedNumberOfTriangles, largerHexagonTriangles.Length);
            Assert.True(ContainsSequenceInArray(largerHexagonTriangles, new int[]{0, 1, 2}));
            Assert.True(ContainsSequenceInArray(largerHexagonTriangles, new int[]{2, 10, 3}));
            Assert.True(ContainsSequenceInArray(largerHexagonTriangles, new int[]{15, 16, 5}));
            Assert.True(ContainsSequenceInArray(largerHexagonTriangles, new int[]{14, 29, 30}));
            Assert.True(ContainsSequenceInArray(largerHexagonTriangles, new int[]{9, 22, 23}));
            Assert.True(ContainsSequenceInArray(largerHexagonTriangles, new int[]{31, 32, 15}));
            Assert.False(ContainsSequenceInArray(largerHexagonTriangles, new int[]{47, 25, 46}));
            Assert.True(ContainsSequenceInArray(largerHexagonTriangles, new int[]{19, 37, 20}));
            Assert.True(ContainsSequenceInArray(largerHexagonTriangles, new int[]{26, 43, 44}));
            Assert.True(ContainsSequenceInArray(largerHexagonTriangles, new int[]{26, 44, 27}));
        }

        [Test]
        public void should_generate_hexagons_with_correct_number_of_triangles()
        {
            var noTriangles_0 = hexTrianglesGenerator.TrianglesForHexagons(0);
            var noTriangles_neg_0 = hexTrianglesGenerator.TrianglesForHexagons(-1);
            var triangles_2 = hexTrianglesGenerator.TrianglesForHexagons(2);
            var triangles_4 = hexTrianglesGenerator.TrianglesForHexagons(4);
            var triangles_10 = hexTrianglesGenerator.TrianglesForHexagons(10);
            
            var expectedCount_2 = 72 + 18 * 3;
            var expectedCount_4 = 288 + 18 * 7;
            var expectedCount_10 = Enumerable.Range(1, 10).Select(x => 36 * x - 18).Sum() + 18 * 19;

            Assert.AreEqual(new int[0], noTriangles_0);
            Assert.AreEqual(new int[0], noTriangles_neg_0);
            Assert.AreEqual(expectedCount_2, triangles_2.Length);
            Assert.AreEqual(expectedCount_4, triangles_4.Length);
            Assert.AreEqual(expectedCount_10, triangles_10.Length);
        }

        private bool ContainsSequenceInArray(int[] array, int[] sequence)
        {
            for (int i = 0; i <= array.Length - sequence.Length; i++)
                if (array.Skip(i).Take(sequence.Length).SequenceEqual(sequence))
                    return true;
            
            return false;
        }
    }
}