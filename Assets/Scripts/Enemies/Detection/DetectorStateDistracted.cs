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
            IVisionConeViewModel visionConeViewModel,
            IEvents events,
            float distractionDuration) 
            : base(playerDetector, visionConeViewModel, events)
        {
            visionConeViewModel.TransitionTo(new VisionConeStateDistracted());
            events.StartCoroutine(ResetDistraction(distractionDuration));
        }

        public override bool AttemptDistraction(float distractionDuration)
        {
            return false;
        }

        public override void UpdateDetectionState()
        {
        }

        IEnumerator ResetDistraction(float distractionDuration)
        {
            yield return new WaitForSeconds(distractionDuration);

            VisionConeViewModel.TransitionTo(new VisionConeStatePatrolling());
            PlayerDetector.TransitionTo(new DetectorStateIdle(
                PlayerDetector, 
                VisionConeViewModel, 
                Events)
            );
        }
    }
}