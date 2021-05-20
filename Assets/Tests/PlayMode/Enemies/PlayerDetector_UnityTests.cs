using System;
using System.Collections;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PlayerDetector_UnityTests
    {
        private GameObject gameObject = new GameObject("PlayerDetector");
        private MonoBehaviour_Mock playerDetectorMono_Mock;
        private PlayerDetector playerDetector;
        private IVisionConeVM visionConeCM;
        private EventsMono_Mock eventsMono;
        private float DetectionEscalationSpeed = 5f;
        private float DetectionDeescalationSpeed = 5f;
        private float distractionDuration = 0.1f;

        private void SetupVisionConeMock()
        {
            playerDetector = new PlayerDetector(
                visionConeCM, 
                eventsMono, 
                DetectionEscalationSpeed, 
                DetectionDeescalationSpeed
            );
            playerDetectorMono_Mock = gameObject.AddComponent<MonoBehaviour_Mock>();
            playerDetectorMono_Mock.Updatable = playerDetector;
        }

        [UnityTest]
        public IEnumerator should_with_initialization_only_trigger_idle_state()
        {
            eventsMono = EventsMono_Mock.NewSubstitute();
            visionConeCM = Substitute.For<IVisionConeVM>();
            SetupVisionConeMock();
            
            yield return null;

            Assert.AreEqual(0, eventsMono.CoroutineStartCounter);
            eventsMono.EnemyEvents.DidNotReceiveWithAnyArgs().FailGame();
            eventsMono.EnemyEvents.Received(1).ChangeDetectorState(playerDetector.ID);
            visionConeCM.Received(1).SetSpotState(SpotLightState.Idle);
            visionConeCM.DidNotReceive().ResetToPatrolling();
            visionConeCM.DidNotReceive().StartFollowingPlayer();
        }

        [UnityTest]
        public IEnumerator should_with_visible_unobstructed_player_transition_to_searching()
        {
            eventsMono = EventsMono_Mock.NewSubstitute();
            visionConeCM = Substitute.For<IVisionConeVM>();
            visionConeCM.IsPlayerInsideVisionCone().Returns(true);
            visionConeCM.IsPlayerObstructed().Returns(false);
            SetupVisionConeMock();

            yield return null;

            Assert.AreEqual(0, eventsMono.CoroutineStartCounter);
            eventsMono.EnemyEvents.DidNotReceiveWithAnyArgs().FailGame();
            eventsMono.EnemyEvents.Received(2).ChangeDetectorState(playerDetector.ID);
            visionConeCM.Received(1).SetSpotState(SpotLightState.Searching);
            visionConeCM.Received(1).StartFollowingPlayer();
            visionConeCM.DidNotReceive().ResetToPatrolling();
        }

        [UnityTest]
        public IEnumerator should_with_visible_player_when_becomes_obstructed_second_frame_transition_to_searching_and_back_to_idle()
        {
            eventsMono = EventsMono_Mock.NewSubstitute();
            visionConeCM = Substitute.For<IVisionConeVM>();
            visionConeCM.IsPlayerInsideVisionCone().Returns(true);
            visionConeCM.IsPlayerObstructed().Returns(false);
            SetupVisionConeMock();

            yield return null;

            visionConeCM.IsPlayerObstructed().Returns(true);

            yield return null;

           Assert.AreEqual(0, eventsMono.CoroutineStartCounter);
            eventsMono.EnemyEvents.DidNotReceiveWithAnyArgs().FailGame();
            eventsMono.EnemyEvents.Received(3).ChangeDetectorState(playerDetector.ID);
            visionConeCM.Received(1).SetSpotState(SpotLightState.Searching);
            visionConeCM.Received(1).StartFollowingPlayer();
            visionConeCM.Received(2).SetSpotState(SpotLightState.Idle);
            visionConeCM.Received(1).ResetToPatrolling();
        }

        [UnityTest]
        public IEnumerator should_with_visible_player_after_enough_time_transition_to_alarmed()
        {
            eventsMono = EventsMono_Mock.NewSubstitute();
            visionConeCM = Substitute.For<IVisionConeVM>();
            visionConeCM.IsPlayerInsideVisionCone().Returns(true);
            visionConeCM.IsPlayerObstructed().Returns(false);
            SetupVisionConeMock();

            yield return new WaitForSeconds(0.5f);

            Assert.AreEqual(0, eventsMono.CoroutineStartCounter);
            eventsMono.EnemyEvents.Received().FailGame();
            eventsMono.EnemyEvents.Received(3).ChangeDetectorState(playerDetector.ID);
            visionConeCM.Received(1).SetSpotState(SpotLightState.Idle);
            visionConeCM.Received(1).SetSpotState(SpotLightState.Searching);
            visionConeCM.Received(1).StartFollowingPlayer();
            visionConeCM.Received(1).SetSpotState(SpotLightState.Alarmed);
        }

        [UnityTest]
        public IEnumerator should_with_idle_player_when_attempting_distraction_transition_to_distracted()
        {
            eventsMono = EventsMono_Mock.NewSubstitute();
            visionConeCM = Substitute.For<IVisionConeVM>();
            SetupVisionConeMock();

            yield return null;

            playerDetector.AttemptDistraction(0.1f);

            yield return null;

            Assert.AreEqual(1, eventsMono.CoroutineStartCounter);
            eventsMono.EnemyEvents.DidNotReceiveWithAnyArgs().FailGame();
            eventsMono.EnemyEvents.Received(2).ChangeDetectorState(playerDetector.ID);
            visionConeCM.Received(1).SetSpotState(SpotLightState.Idle);
            visionConeCM.Received(1).SetSpotState(SpotLightState.Distracted);
            visionConeCM.Received(1).SetStateDistracted(true);
            visionConeCM.DidNotReceive().ResetToPatrolling();
            visionConeCM.DidNotReceive().StartFollowingPlayer();
        }

        [UnityTest]
        public IEnumerator should_when_distracted_return_to_idle_after_distract_duration()
        {
            eventsMono = EventsMono_Mock.NewSubstitute();
            visionConeCM = Substitute.For<IVisionConeVM>();
            SetupVisionConeMock();

            yield return null;

            playerDetector.AttemptDistraction(distractionDuration);

            yield return new WaitForSeconds(distractionDuration);

            Assert.AreEqual(1, eventsMono.CoroutineStartCounter);
            eventsMono.EnemyEvents.DidNotReceiveWithAnyArgs().FailGame();
            eventsMono.EnemyEvents.Received(3).ChangeDetectorState(playerDetector.ID);
            visionConeCM.Received(2).SetSpotState(SpotLightState.Idle);
            visionConeCM.Received(1).SetSpotState(SpotLightState.Distracted);
            visionConeCM.Received(1).SetStateDistracted(true);
            visionConeCM.Received(1).SetStateDistracted(false);
            visionConeCM.DidNotReceive().ResetToPatrolling();
            visionConeCM.DidNotReceive().StartFollowingPlayer();
        }

        [UnityTest]
        public IEnumerator should_with_distracted_detector_not_react_to_visible_player()
        {
            eventsMono = EventsMono_Mock.NewSubstitute();
            visionConeCM = Substitute.For<IVisionConeVM>();
            SetupVisionConeMock();

            yield return null;

            playerDetector.AttemptDistraction(distractionDuration);

            yield return null;

            visionConeCM.IsPlayerInsideVisionCone().Returns(true);
            visionConeCM.IsPlayerObstructed().Returns(false);

            yield return null;

            Assert.AreEqual(1, eventsMono.CoroutineStartCounter);
            eventsMono.EnemyEvents.DidNotReceiveWithAnyArgs().FailGame();
            eventsMono.EnemyEvents.Received(2).ChangeDetectorState(playerDetector.ID);
            visionConeCM.Received(1).SetSpotState(SpotLightState.Idle);
            visionConeCM.Received(1).SetSpotState(SpotLightState.Distracted);
            visionConeCM.Received(1).SetStateDistracted(true);
            visionConeCM.DidNotReceive().SetSpotState(SpotLightState.Searching);
            visionConeCM.DidNotReceive().StartFollowingPlayer();
        }

        [UnityTest]
        public IEnumerator should_with_alarmed_detector_not_react_to_distraction()
        {
            eventsMono = EventsMono_Mock.NewSubstitute();
            visionConeCM = Substitute.For<IVisionConeVM>();
            visionConeCM.IsPlayerInsideVisionCone().Returns(true);
            visionConeCM.IsPlayerObstructed().Returns(false);
            SetupVisionConeMock();

            yield return new WaitForSeconds(0.5f);

            playerDetector.AttemptDistraction(distractionDuration);

            yield return null;

            Assert.AreEqual(0, eventsMono.CoroutineStartCounter);
            eventsMono.EnemyEvents.Received().FailGame();
            eventsMono.EnemyEvents.Received(3).ChangeDetectorState(playerDetector.ID);
            visionConeCM.Received(1).SetSpotState(SpotLightState.Idle);
            visionConeCM.Received(1).SetSpotState(SpotLightState.Searching);
            visionConeCM.Received(1).SetSpotState(SpotLightState.Alarmed);
            visionConeCM.Received(1).StartFollowingPlayer();
            visionConeCM.DidNotReceive().SetSpotState(SpotLightState.Distracted);
        }
    }
}