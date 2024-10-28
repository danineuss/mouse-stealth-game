using System;
using System.Collections;
using System.Collections.Generic;
using Enemies.VisionCone;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class VisionConeViewModel_Tests
    {
        private GameObject visionConeObject = new GameObject("VisionCone");
        private GameObject playerObject = new GameObject("Player");
        private MonoBehaviour_Mock visionConeMono_Mock;
        private VisionConeViewModel visionConeViewModel;
        private List<IVisionConePatrolPoint> patrolPoints;
        private IVisionConeControlPoint distractPoint;
        private IConeVisualizer coneVisualizer;
        private LayerMask layerMask = LayerMask.NameToLayer("Obstacles");
        private EventsMono_Mock eventsMono;

        private void SetupVisionCone()
        {
            visionConeObject.transform.position = Vector3.zero;
            coneVisualizer = Substitute.For<IConeVisualizer>();
            visionConeViewModel = new VisionConeViewModel(
                patrolPoints,
                distractPoint,
                coneVisualizer,
                visionConeObject.transform,
                playerObject.transform,
                layerMask,
                eventsMono
            );
            visionConeMono_Mock = visionConeObject.AddComponent<MonoBehaviour_Mock>();
            visionConeMono_Mock.Updatables.Add(visionConeViewModel);
        }

        [Test]
        public void should_detect_player_if_inside_range_and_vision_cone()
        {
            distractPoint = Substitute.For<IVisionConeControlPoint>();
            eventsMono = EventsMono_Mock.NewSubstitute();

            var patrolPoint = new VisionConePatrolPoint(50, new Vector3(2, 0, 0), 1f, 1f);
            patrolPoints = new List<IVisionConePatrolPoint>() { patrolPoint };
            playerObject.transform.position = new Vector3(1, 0, 0);
            SetupVisionCone();

            Assert.True(visionConeViewModel.IsPlayerInsideVisionCone());
        }

        [Test]
        public void should_not_detect_player_if_outside_range()
        {
            distractPoint = Substitute.For<IVisionConeControlPoint>();
            eventsMono = EventsMono_Mock.NewSubstitute();

            var patrolPoint = new VisionConePatrolPoint(50, new Vector3(2, 0, 0), 1f, 1f);
            patrolPoints = new List<IVisionConePatrolPoint>() { patrolPoint };
            playerObject.transform.position = new Vector3(3, 0, 0);
            SetupVisionCone();

            Assert.True(!visionConeViewModel.IsPlayerInsideVisionCone());
        }

        [Test]
        public void should_not_detect_player_if_outside_field_of_vision()
        {
            distractPoint = Substitute.For<IVisionConeControlPoint>();
            eventsMono = EventsMono_Mock.NewSubstitute();

            var patrolPoint = new VisionConePatrolPoint(50, new Vector3(2, 0, 0), 1f, 1f);
            patrolPoints = new List<IVisionConePatrolPoint>() { patrolPoint };
            playerObject.transform.position = new Vector3(-1, 0, 0);
            SetupVisionCone();

            Assert.True(!visionConeViewModel.IsPlayerInsideVisionCone());

            playerObject.transform.position = new Vector3(0, 1, 0);

            Assert.True(!visionConeViewModel.IsPlayerInsideVisionCone());
        }

        [Test]
        public void should_not_be_obstructed_without_obstacle()
        {
            distractPoint = Substitute.For<IVisionConeControlPoint>();
            eventsMono = EventsMono_Mock.NewSubstitute();

            var patrolPoint = new VisionConePatrolPoint(50, new Vector3(2, 0, 0), 1f, 1f);
            patrolPoints = new List<IVisionConePatrolPoint>() { patrolPoint };
            playerObject.transform.position = new Vector3(1, 0, 0);
            SetupVisionCone();

            Assert.True(visionConeViewModel.IsPlayerInsideVisionCone());
            Assert.True(!visionConeViewModel.IsPlayerObstructed());
        }
    }
}