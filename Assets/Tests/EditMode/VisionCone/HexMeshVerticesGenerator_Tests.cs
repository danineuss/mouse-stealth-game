using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class HexMeshVerticesGenerator_Tests
    {
        private IHexMeshVerticesGeneratable hexVerticeGenerator;
        private readonly float RatioInnerToOuterRadius = 0.866025404f;
        private readonly float FloatingPointDelta = 0.005f;
        
        private void SetupHexMeshVerticesGenerator()
        {
            hexVerticeGenerator = new HexMeshVerticesGenerator();
        }

        [Test]
        public void should_generate_one_hexagon_with_correct_vertices_when_resolution_1_radius_1()
        {
            SetupHexMeshVerticesGenerator();

            var vertices = hexVerticeGenerator.HexagonVerticesForRadius(1f, 1);
            var expectedCircleVertice_0 = new Vector3(-Mathf.Sin(Mathf.PI / 6), Mathf.Cos(Mathf.PI / 6), 0f);
            var expectedCircleVertice_1 = new Vector3(-1f, 0f, 0f);
            var expectedCircleVertice_2 = new Vector3(-Mathf.Sin(Mathf.PI / 6), -Mathf.Cos(Mathf.PI / 6), 0f);
            var expectedCircleVertice_3 = new Vector3(Mathf.Sin(Mathf.PI / 6), -Mathf.Cos(Mathf.PI / 6), 0f);
            var expectedCircleVertice_4 = new Vector3(1f, 0f, 0f);
            var expectedCircleVertice_5 = new Vector3(Mathf.Sin(Mathf.PI / 6), Mathf.Cos(Mathf.PI / 6), 0f);

            Assert.AreEqual(13, vertices.Length);
            Assert.AreEqual(new Vector3(0, 0, 0), vertices[0]);
            Assert.AreEqual(new Vector3(0, 1, 0), vertices[1]);
            Assert.AreEqual(new Vector3(-RatioInnerToOuterRadius, 0.5f, 0), vertices[2]);
            Assert.AreEqual(new Vector3(-RatioInnerToOuterRadius, -0.5f, 0), vertices[3]);
            Assert.AreEqual(new Vector3(0, -1, 0), vertices[4]);
            Assert.AreEqual(new Vector3(RatioInnerToOuterRadius, -0.5f, 0), vertices[5]);
            Assert.AreEqual(new Vector3(RatioInnerToOuterRadius, 0.5f, 0), vertices[6]);
            Assert.True(Vector3.Distance(expectedCircleVertice_0, vertices[7]) < FloatingPointDelta);
            Assert.True(Vector3.Distance(expectedCircleVertice_1, vertices[8]) < FloatingPointDelta);
            Assert.True(Vector3.Distance(expectedCircleVertice_2, vertices[9]) < FloatingPointDelta);
            Assert.True(Vector3.Distance(expectedCircleVertice_3, vertices[10]) < FloatingPointDelta);
            Assert.True(Vector3.Distance(expectedCircleVertice_4, vertices[11]) < FloatingPointDelta);
            Assert.True(Vector3.Distance(expectedCircleVertice_5, vertices[12]) < FloatingPointDelta);
        }

        [Test]
        public void should_generate_correct_number_of_vertices_for_different_radii()
        {
            SetupHexMeshVerticesGenerator();

            var vertices_1 = hexVerticeGenerator.HexagonVerticesForRadius(1f, 1);
            var vertices_1_1 = hexVerticeGenerator.HexagonVerticesForRadius(1.1f, 1);
            var vertices_1_6 = hexVerticeGenerator.HexagonVerticesForRadius(1.6f, 2);
            var vertices_2 = hexVerticeGenerator.HexagonVerticesForRadius(2f, 2);
            var vertices_3 = hexVerticeGenerator.HexagonVerticesForRadius(3, 3);
            var vertices_10 = hexVerticeGenerator.HexagonVerticesForRadius(10, 10);
            var vertices_0_1 = hexVerticeGenerator.HexagonVerticesForRadius(0.1f, 1);
            var vertices_neg_1 = hexVerticeGenerator.HexagonVerticesForRadius(-1f, 1);

            Assert.AreEqual(7 + 6, vertices_1.Length);
            Assert.AreEqual(7 + 6, vertices_1_1.Length);
            Assert.AreEqual(19 + 12, vertices_1_6.Length);
            Assert.AreEqual(19 + 12, vertices_2.Length);
            Assert.AreEqual(37 + 18, vertices_3.Length);
            Assert.AreEqual(1 + Enumerable.Range(1, 10).Select(x => x * 6).Sum() + 6 * 10, vertices_10.Length);
            Assert.AreEqual(7 + 6, vertices_0_1.Length);
            Assert.AreEqual(0, vertices_neg_1.Length);
        }
    }
}