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
        private IVisionConeVM visionConeVM;
        private EventsMono_Mock eventsMono;
        private float DetectionEscalationSpeed = 5f;
        private float DetectionDeescalationSpeed = 5f;
        private float distractionDuration = 0.1f;

        private void SetupPlayerDetector()
        {
            playerDetector = new PlayerDetector(
                visionConeVM, 
                eventsMono, 
                DetectionEscalationSpeed, 
                DetectionDeescalationSpeed
            );
            playerDetectorMono_Mock = gameObject.AddComponent<MonoBehaviour_Mock>();
            playerDetectorMono_Mock.Updatables.Add(playerDetector);
        }

        [UnityTest]
        public IEnumerator should_with_initialization_only_trigger_idle_state()
        {
            eventsMono = EventsMono_Mock.NewSubstitute();
            visionConeVM = Substitute.For<IVisionConeVM>();
            SetupPlayerDetector();
            
            yield return null;

            Assert.AreEqual(0, eventsMono.CoroutineStartCounter);
            eventsMono.EnemyEvents.DidNotReceiveWithAnyArgs().FailGame();
            eventsMono.EnemyEvents.Received(1).ChangeDetectorState(playerDetector.ID);
            visionConeVM.ReceivedWithAnyArgs(1).TransitionTo(default);
            visionConeVM.Received(1).TransitionTo(Arg.Any<VisionConeStateIdle>());
        }

        [UnityTest]
        public IEnumerator should_with_visible_unobstructed_player_transition_to_searching()
        {
            eventsMono = EventsMono_Mock.NewSubstitute();
            visionConeVM = Substitute.For<IVisionConeVM>();
            visionConeVM.IsPlayerInsideVisionCone().Returns(true);
            visionConeVM.IsPlayerObstructed().Returns(false);
            SetupPlayerDetector();

            yield return null;

            Assert.AreEqual(0, eventsMono.CoroutineStartCounter);
            eventsMono.EnemyEvents.DidNotReceiveWithAnyArgs().FailGame();
            eventsMono.EnemyEvents.Received(2).ChangeDetectorState(playerDetector.ID);
            visionConeVM.ReceivedWithAnyArgs(2).TransitionTo(default);
            visionConeVM.Received(1).TransitionTo(Arg.Any<VisionConeStateIdle>());
            visionConeVM.Received(1).TransitionTo(Arg.Any<VisionConeStateFollowingPlayer>());
        }

        [UnityTest]
        public IEnumerator should_with_visible_player_be_searching_when_becomes_obstructed_second_frame_transition_back_to_idle()
        {
            eventsMono = EventsMono_Mock.NewSubstitute();
            visionConeVM = Substitute.For<IVisionConeVM>();
            visionConeVM.IsPlayerInsideVisionCone().Returns(true);
            visionConeVM.IsPlayerObstructed().Returns(false);
            SetupPlayerDetector();

            yield return null;

            visionConeVM.IsPlayerObstructed().Returns(true);

            yield return null;

            Assert.AreEqual(0, eventsMono.CoroutineStartCounter);
            eventsMono.EnemyEvents.DidNotReceiveWithAnyArgs().FailGame();
            eventsMono.EnemyEvents.Received(3).ChangeDetectorState(playerDetector.ID);
            visionConeVM.ReceivedWithAnyArgs(3).TransitionTo(default);
            visionConeVM.Received(2).TransitionTo(Arg.Any<VisionConeStateIdle>());
            visionConeVM.Received(1).TransitionTo(Arg.Any<VisionConeStateFollowingPlayer>());
        }

        [UnityTest]
        public IEnumerator should_with_visible_player_after_enough_time_transition_to_alarmed()
        {
            eventsMono = EventsMono_Mock.NewSubstitute();
            visionConeVM = Substitute.For<IVisionConeVM>();
            visionConeVM.IsPlayerInsideVisionCone().Returns(true);
            visionConeVM.IsPlayerObstructed().Returns(false);
            SetupPlayerDetector();

            yield return new WaitForSeconds(0.5f);

            Assert.AreEqual(0, eventsMono.CoroutineStartCounter);
            eventsMono.EnemyEvents.Received().FailGame();
            eventsMono.EnemyEvents.Received(3).ChangeDetectorState(playerDetector.ID);
            visionConeVM.ReceivedWithAnyArgs(2).TransitionTo(default);
            visionConeVM.Received(1).TransitionTo(Arg.Any<VisionConeStateIdle>());
            visionConeVM.Received(1).TransitionTo(Arg.Any<VisionConeStateFollowingPlayer>());
        }

        [UnityTest]
        public IEnumerator should_with_idle_player_when_attempting_distraction_transition_to_distracted()
        {
            eventsMono = EventsMono_Mock.NewSubstitute();
            visionConeVM = Substitute.For<IVisionConeVM>();
            SetupPlayerDetector();

            yield return null;

            playerDetector.AttemptDistraction(0.1f);

            yield return null;

            Assert.AreEqual(1, eventsMono.CoroutineStartCounter);
            eventsMono.EnemyEvents.DidNotReceiveWithAnyArgs().FailGame();
            eventsMono.EnemyEvents.Received(2).ChangeDetectorState(playerDetector.ID);
            visionConeVM.ReceivedWithAnyArgs(2).TransitionTo(default);
            visionConeVM.Received(1).TransitionTo(Arg.Any<VisionConeStateIdle>());
            visionConeVM.Received(1).TransitionTo(Arg.Any<VisionConeStateDistracted>());
        }

        [UnityTest]
        public IEnumerator should_when_distracted_return_to_idle_after_distract_duration()
        {
            eventsMono = EventsMono_Mock.NewSubstitute();
            visionConeVM = Substitute.For<IVisionConeVM>();
            SetupPlayerDetector();

            yield return null;

            playerDetector.AttemptDistraction(distractionDuration);

            yield return new WaitForSeconds(distractionDuration);

            Assert.AreEqual(1, eventsMono.CoroutineStartCounter);
            eventsMono.EnemyEvents.DidNotReceiveWithAnyArgs().FailGame();
            eventsMono.EnemyEvents.Received(3).ChangeDetectorState(playerDetector.ID);
            visionConeVM.ReceivedWithAnyArgs(4).TransitionTo(default);
            visionConeVM.Received(2).TransitionTo(Arg.Any<VisionConeStateIdle>());
            visionConeVM.Received(1).TransitionTo(Arg.Any<VisionConeStateDistracted>());
            visionConeVM.Received(1).TransitionTo(Arg.Any<VisionConeStatePatrolling>());
        }

        [UnityTest]
        public IEnumerator should_with_distracted_detector_not_react_to_visible_player()
        {
            eventsMono = EventsMono_Mock.NewSubstitute();
            visionConeVM = Substitute.For<IVisionConeVM>();
            SetupPlayerDetector();

            yield return null;

            playerDetector.AttemptDistraction(distractionDuration);

            yield return null;

            visionConeVM.IsPlayerInsideVisionCone().Returns(true);
            visionConeVM.IsPlayerObstructed().Returns(false);

            yield return null;

            Assert.AreEqual(1, eventsMono.CoroutineStartCounter);
            eventsMono.EnemyEvents.DidNotReceiveWithAnyArgs().FailGame();
            eventsMono.EnemyEvents.Received(2).ChangeDetectorState(playerDetector.ID);
            visionConeVM.ReceivedWithAnyArgs(2).TransitionTo(default);
            visionConeVM.Received(1).TransitionTo(Arg.Any<VisionConeStateIdle>());
            visionConeVM.Received(1).TransitionTo(Arg.Any<VisionConeStateDistracted>());
        }

        [UnityTest]
        public IEnumerator should_with_alarmed_detector_not_react_to_distraction()
        {
            eventsMono = EventsMono_Mock.NewSubstitute();
            visionConeVM = Substitute.For<IVisionConeVM>();
            visionConeVM.IsPlayerInsideVisionCone().Returns(true);
            visionConeVM.IsPlayerObstructed().Returns(false);
            SetupPlayerDetector();

            yield return new WaitForSeconds(0.5f);

            playerDetector.AttemptDistraction(distractionDuration);

            yield return null;

            Assert.AreEqual(0, eventsMono.CoroutineStartCounter);
            eventsMono.EnemyEvents.Received().FailGame();
            eventsMono.EnemyEvents.Received(3).ChangeDetectorState(playerDetector.ID);
            visionConeVM.ReceivedWithAnyArgs(2).TransitionTo(default);
            visionConeVM.Received(1).TransitionTo(Arg.Any<VisionConeStateIdle>());
            visionConeVM.Received(1).TransitionTo(Arg.Any<VisionConeStateFollowingPlayer>());
        }
    }
}