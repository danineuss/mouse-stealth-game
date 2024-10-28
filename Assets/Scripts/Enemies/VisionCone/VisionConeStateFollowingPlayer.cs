using System.Collections;
using UnityEngine;

namespace Enemies.VisionCone
{
    public class VisionConeStateFollowingPlayer : VisionConeState
    {
        private Transform playerTransform;
        private float EvaluationWaitTime = 2f;
        private const float FollowPlayerClampValue = 0.1f;

        public override void SetupVisionConeState(
            IVisionConeVM visionConeVM,
            IConeVisualizer coneVisualizer,
            IVisionConePatrolPoint patrolPoint, 
            IVisionConeControlPoint distractPoint,
            Transform playerTransform)
        {
            this.visionConeVM = visionConeVM;
            this.coneVisualizer = coneVisualizer;
            this.playerTransform = playerTransform;

            coneVisualizer.SetSpotState(SpotLightState.Searching);
            visionConeVM.StartLookatCoroutine(FollowPlayer());
        }

        public override void UpdateDetectionMeter(float detectionEscalationMeter) 
        {
            coneVisualizer.SetSpotState(SpotLightState.Searching, detectionEscalationMeter);
        }

        private IEnumerator FollowPlayer()
        {
            while (!visionConeVM.IsPlayerObstructed())
            {
                var deltaVector = playerTransform.position - visionConeVM.CurrentLookatTarget;
                if (deltaVector.magnitude > FollowPlayerClampValue)
                    deltaVector *= FollowPlayerClampValue / deltaVector.magnitude;
                var newLookatTarget = visionConeVM.CurrentLookatTarget + deltaVector;
            
                visionConeVM.UpdateCone(newLookatTarget, visionConeVM.FieldOfView);
                yield return null;
            }

            visionConeVM.StartLookatCoroutine(EvaluateReturningToPatrolling());
        }

        private IEnumerator EvaluateReturningToPatrolling()
        {
            var startTime = Time.time;
            while(Time.time - startTime < EvaluationWaitTime)
            {
                if (visionConeVM.IsPlayerInsideVisionCone() && !visionConeVM.IsPlayerObstructed())
                    visionConeVM.StartLookatCoroutine(FollowPlayer());

                yield return null;
            }

            visionConeVM.TransitionTo(new VisionConeStatePatrolling());
        }
    }
}