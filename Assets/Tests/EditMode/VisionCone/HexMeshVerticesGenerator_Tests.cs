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
        
        [Test]
        public void should_generate_one_hexagon_with_correct_vertices_when_resolution_1_radius_1()
        {
            hexVerticeGenerator = new HexMeshVerticesGenerator(1);
            var vertices = hexVerticeGenerator.HexagonVerticesForRadius(1f);

            Assert.AreEqual(7, vertices.Length);
            Assert.AreEqual(new Vector3(0, 0, 0), vertices[0]);
            Assert.AreEqual(new Vector3(0, 1, 0), vertices[1]);
            Assert.AreEqual(new Vector3(-RatioInnerToOuterRadius, 0.5f, 0), vertices[2]);
            Assert.AreEqual(new Vector3(-RatioInnerToOuterRadius, -0.5f, 0), vertices[3]);
            Assert.AreEqual(new Vector3(0, -1, 0), vertices[4]);
            Assert.AreEqual(new Vector3(RatioInnerToOuterRadius, -0.5f, 0), vertices[5]);
            Assert.AreEqual(new Vector3(RatioInnerToOuterRadius, 0.5f, 0), vertices[6]);
        }

        [Test]
        public void should_generate_correct_number_of_vertices_for_different_radii()
        {
            hexVerticeGenerator = new HexMeshVerticesGenerator(1);

            var vertices_1 = hexVerticeGenerator.HexagonVerticesForRadius(1f);
            var vertices_1_1 = hexVerticeGenerator.HexagonVerticesForRadius(1.1f);
            var vertices_1_6 = hexVerticeGenerator.HexagonVerticesForRadius(1.6f);
            var vertices_2 = hexVerticeGenerator.HexagonVerticesForRadius(2f);
            var vertices_3 = hexVerticeGenerator.HexagonVerticesForRadius(3);
            var vertices_10 = hexVerticeGenerator.HexagonVerticesForRadius(10);
            var vertices_0_1 = hexVerticeGenerator.HexagonVerticesForRadius(0.1f);
            var vertices_neg_1 = hexVerticeGenerator.HexagonVerticesForRadius(-1f);

            Assert.AreEqual(7, vertices_1.Length);
            Assert.AreEqual(7, vertices_1_1.Length);
            Assert.AreEqual(19, vertices_1_6.Length);
            Assert.AreEqual(19, vertices_2.Length);
            Assert.AreEqual(37, vertices_3.Length);
            Assert.AreEqual(1 + Enumerable.Range(1, 10).Select(x => x * 6).Sum(), vertices_10.Length);
            Assert.AreEqual(7, vertices_0_1.Length);
            Assert.AreEqual(0, vertices_neg_1.Length);
        }
    }
}