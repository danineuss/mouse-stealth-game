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

            // var vertices = hexVerticeGenerator.GenerateHexagonVertices(Mathf.PI / 3f, 1f, 1);
            // var expectedHexagonVertice_0 = new Vector3(0, 0, 1f);
            // var expectedHexagonVertice_1 = new Vector3(0, 0.5f, Mathf.Cos(Mathf.PI / 6f));
            // var expectedHexagonVertice_2 = new Vector3(0, 0, 1f);
            // var expectedHexagonVertice_3 = new Vector3(0, 0, 1f);
            // var expectedHexagonVertice_4 = new Vector3(0, 0, 1f);
            // var expectedHexagonVertice_5 = new Vector3(0, 0, 1f);
            // var expectedHexagonVertice_6 = new Vector3(0, 0, 1f);
            // var expectedCircleVertice_7 = new Vector3(-Mathf.Sin(Mathf.PI / 6), Mathf.Cos(Mathf.PI / 6), 0f);
            // var expectedCircleVertice_8 = new Vector3(-1f, 0f, 0f);
            // var expectedCircleVertice_9 = new Vector3(-Mathf.Sin(Mathf.PI / 6), -Mathf.Cos(Mathf.PI / 6), 0f);
            // var expectedCircleVertice_10 = new Vector3(Mathf.Sin(Mathf.PI / 6), -Mathf.Cos(Mathf.PI / 6), 0f);
            // var expectedCircleVertice_11 = new Vector3(1f, 0f, 0f);
            // var expectedCircleVertice_12 = new Vector3(Mathf.Sin(Mathf.PI / 6), Mathf.Cos(Mathf.PI / 6), 0f);

            // Assert.AreEqual(13, vertices.Length);
            // Assert.AreEqual(new Vector3(0, 0, 1f), vertices[0]);
            // Assert.AreEqual(new Vector3(0, 0.5f, Mathf.Cos(Mathf.PI / 6f)), vertices[1]);
            // Assert.AreEqual(new Vector3(-RatioInnerToOuterRadius, 0.5f, 1), vertices[2]);
            // Assert.AreEqual(new Vector3(-RatioInnerToOuterRadius, -0.5f, 1), vertices[3]);
            // Assert.AreEqual(new Vector3(0, -1, 1), vertices[4]);
            // Assert.AreEqual(new Vector3(RatioInnerToOuterRadius, -0.5f, 1), vertices[5]);
            // Assert.AreEqual(new Vector3(RatioInnerToOuterRadius, 0.5f, 1), vertices[6]);
            // Assert.True(Vector3.Distance(expectedCircleVertice_7, vertices[7]) < FloatingPointDelta);
            // Assert.True(Vector3.Distance(expectedCircleVertice_8, vertices[8]) < FloatingPointDelta);
            // Assert.True(Vector3.Distance(expectedCircleVertice_9, vertices[9]) < FloatingPointDelta);
            // Assert.True(Vector3.Distance(expectedCircleVertice_10, vertices[10]) < FloatingPointDelta);
            // Assert.True(Vector3.Distance(expectedCircleVertice_11, vertices[11]) < FloatingPointDelta);
            // Assert.True(Vector3.Distance(expectedCircleVertice_12, vertices[12]) < FloatingPointDelta);
        }

        [Test]
        public void should_generate_correct_number_of_vertices_for_different_radii()
        {
            SetupHexMeshVerticesGenerator();

            var vertices_1 = hexVerticeGenerator.GenerateHexagonVertices(Mathf.PI / 2f, 1f, 1);
            var vertices_1_1 = hexVerticeGenerator.GenerateHexagonVertices(Mathf.PI / 2f, 1.1f, 1);
            var vertices_1_6 = hexVerticeGenerator.GenerateHexagonVertices(Mathf.PI / 2f, 1.6f, 2);
            var vertices_2 = hexVerticeGenerator.GenerateHexagonVertices(Mathf.PI / 2f, 2f, 2);
            var vertices_3 = hexVerticeGenerator.GenerateHexagonVertices(Mathf.PI / 2f, 3, 3);
            var vertices_10 = hexVerticeGenerator.GenerateHexagonVertices(Mathf.PI / 2f, 10, 10);
            var vertices_0_1 = hexVerticeGenerator.GenerateHexagonVertices(Mathf.PI / 2f, 0.1f, 1);
            var vertices_neg_1 = hexVerticeGenerator.GenerateHexagonVertices(Mathf.PI / 2f, -1f, 1);

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