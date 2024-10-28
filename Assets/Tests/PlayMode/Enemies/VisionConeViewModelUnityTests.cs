using System.Collections;
using System.Collections.Generic;
using Enemies.VisionCone;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode.Enemies
{
    public class VisionConeViewModelUnityTests
    {
        private readonly GameObject visionConeObject = new GameObject("VisionCone");
        private readonly GameObject playerObject = new GameObject("Player");
        private MonoBehaviour_Mock visionConeMono_Mock;
        private VisionConeViewModel visionConeViewModel;
        private List<IVisionConePatrolPoint> patrolPoints;
        private IVisionConeControlPoint distractPoint;
        private IConeVisualizer coneVisualizer;
        private readonly LayerMask layerMask = LayerMask.NameToLayer("Obstacles");
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

        [UnityTest]
        public IEnumerator should_with_one_patrol_point_stay_idle()
        {
            distractPoint = Substitute.For<IVisionConeControlPoint>();
            eventsMono = EventsMono_Mock.NewSubstitute();

            var patrolPoint = new VisionConePatrolPoint(50, new Vector3(2, 0, 0), 1f, 1f);
            patrolPoints = new List<IVisionConePatrolPoint>() { patrolPoint };
            playerObject.transform.position = new Vector3(1, 0, 0);
            SetupVisionCone();

            yield return null;

            coneVisualizer.ReceivedWithAnyArgs(1).SetSpotState(default);
            coneVisualizer.Received(1).SetSpotState(SpotLightState.Idle);            
        }

        [UnityTest]
        public IEnumerator should_with_two_patrol_points_switch_to_patrolling()
        {
            distractPoint = Substitute.For<IVisionConeControlPoint>();
            eventsMono = EventsMono_Mock.NewSubstitute();

            var patrolPoint = new VisionConePatrolPoint(50, new Vector3(2, 0, 0), 1f, 1f);
            var secondPatrolPoint = new VisionConePatrolPoint(50, new Vector3(2, 2, 0), 1f, 1f);
            patrolPoints = new List<IVisionConePatrolPoint>() { patrolPoint, secondPatrolPoint };
            playerObject.transform.position = new Vector3(1, 0, 0);
            SetupVisionCone();

            yield return null;

            coneVisualizer.ReceivedWithAnyArgs(2).SetSpotState(default);
            coneVisualizer.Received(2).SetSpotState(SpotLightState.Idle);
            Assert.AreEqual(2, eventsMono.CoroutineStartCounter);
        }

        [UnityTest]
        public IEnumerator should_when_searching_update_color_according_to_detection_meter()
        {
            distractPoint = Substitute.For<IVisionConeControlPoint>();
            eventsMono = EventsMono_Mock.NewSubstitute();

            var patrolPoint = new VisionConePatrolPoint(50, new Vector3(2, 0, 0), 1f, 1f);
            patrolPoints = new List<IVisionConePatrolPoint>() { patrolPoint };
            playerObject.transform.position = new Vector3(1, 0, 0);
            SetupVisionCone();
            visionConeViewModel.TransitionTo(new VisionConeStateFollowingPlayer());

            yield return null;

            visionConeViewModel.UpdateDetectionMeter(0.5f);
            visionConeViewModel.UpdateDetectionMeter(1.0f);

            yield return null;

            coneVisualizer.ReceivedWithAnyArgs(4).SetSpotState(default);
            coneVisualizer.Received(1).SetSpotState(SpotLightState.Idle);
            coneVisualizer.Received(1).SetSpotState(SpotLightState.Searching);
            coneVisualizer.Received(1).SetSpotState(SpotLightState.Searching, 0.5f);
            coneVisualizer.Received(1).SetSpotState(SpotLightState.Searching, 1.0f);
            Assert.AreEqual(1, eventsMono.CoroutineStartCounter);
        }
    }
}