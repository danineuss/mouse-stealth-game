using System.Collections;
using UnityEngine;

namespace Enemies.VisionCone
{
    public class VisionConeStatePatrolling : VisionConeState
    {
        private IVisionConePatrolPoint currentPatrolPoint;

        public override void SetupVisionConeState(
            IVisionConeVM visionConeVM, 
            IConeVisualizer coneVisualizer,
            IVisionConePatrolPoint patrolPoint, 
            IVisionConeControlPoint distractPoint,
            Transform playerTransform)
        {
            this.visionConeVM = visionConeVM;
            this.coneVisualizer = coneVisualizer;
            this.currentPatrolPoint = patrolPoint;

            coneVisualizer.SetSpotState(SpotLightState.Idle);
            MoveTowardsNextControlPoint();
        }

        public override void UpdateDetectionMeter(float detectionEscalationMeter) {}

        void MoveTowardsNextControlPoint()
        {
            var lerpLookatTarget = visionConeVM.LerpTowardsTarget(
                currentPatrolPoint.Position, 
                currentPatrolPoint.FieldOfView, 
                currentPatrolPoint.DurationTowardsPoint
            );
            visionConeVM.StartLookatCoroutine(lerpLookatTarget);
            visionConeVM.StartLookatCoroutine(WaitAndIterate(), false);
        }

        IEnumerator WaitAndIterate()
        {
            var waitTime = currentPatrolPoint.DurationTowardsPoint + currentPatrolPoint.WaitTimeAtTarget;
            yield return new WaitForSeconds(waitTime);

            visionConeVM.IterateControlPointIndex();
            visionConeVM.TransitionTo(new VisionConeStateIdle());
        }
    }
}