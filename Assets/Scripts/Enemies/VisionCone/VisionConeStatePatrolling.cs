using System.Collections;
using UnityEngine;

namespace Enemies.VisionCone
{
    public class VisionConeStatePatrolling : VisionConeState
    {
        private IVisionConePatrolPoint currentPatrolPoint;

        public override void SetupVisionConeState(
            IVisionConeViewModel visionConeViewModel, 
            IConeVisualizer coneVisualizer,
            IVisionConePatrolPoint patrolPoint, 
            IVisionConeControlPoint distractPoint,
            Transform playerTransform)
        {
            VisionConeViewModel = visionConeViewModel;
            this.ConeVisualizer = coneVisualizer;
            currentPatrolPoint = patrolPoint;

            coneVisualizer.SetSpotState(SpotLightState.Idle);
            MoveTowardsNextControlPoint();
        }

        public override void UpdateDetectionMeter(float detectionEscalationMeter) {}

        void MoveTowardsNextControlPoint()
        {
            var lerpLookatTarget = VisionConeViewModel.LerpTowardsTarget(
                currentPatrolPoint.Position, 
                currentPatrolPoint.FieldOfView, 
                currentPatrolPoint.DurationTowardsPoint
            );
            VisionConeViewModel.StartLookAtCoroutine(lerpLookatTarget);
            VisionConeViewModel.StartLookAtCoroutine(WaitAndIterate(), false);
        }

        IEnumerator WaitAndIterate()
        {
            var waitTime = currentPatrolPoint.DurationTowardsPoint + currentPatrolPoint.WaitTimeAtTarget;
            yield return new WaitForSeconds(waitTime);

            VisionConeViewModel.IterateControlPointIndex();
            VisionConeViewModel.TransitionTo(new VisionConeStateIdle());
        }
    }
}