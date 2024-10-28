using System.Collections;
using Audio;
using Enemies.VisionCone;
using Infrastructure;
using UnityEngine;

namespace Enemies.Detection
{
    public class DetectorStateDistracted : DetectorState
    {
        public override EnemyIOTextColor EnemyIOTextColor => EnemyIOTextColor.Inactive;
        public override EnemySound EnemySound => EnemySound.Distracted;

        public DetectorStateDistracted(
            IPlayerDetector playerDetector, 
            IVisionConeVM visionConeVM,
            IEvents events,
            float distractionDuration) 
            : base(playerDetector, visionConeVM, events)
        {
            visionConeVM.TransitionTo(new VisionConeStateDistracted());
            events.StartCoroutine(ResetDistraction(distractionDuration));
        }

        public override bool AttemptDistraction(float distractionDuration)
        {
            return false;
        }

        public override void UpdateDetectionState()
        {
            return;
        }

        IEnumerator ResetDistraction(float distractionDuration)
        {
            yield return new WaitForSeconds(distractionDuration);

            visionConeVM.TransitionTo(new VisionConeStatePatrolling());
            playerDetector.TransitionTo(new DetectorStateIdle(
                playerDetector, 
                visionConeVM, 
                events)
            );
        }
    }
}