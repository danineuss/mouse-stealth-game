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
            IVisionConeViewModel visionConeViewModel,
            IConeVisualizer coneVisualizer,
            IVisionConePatrolPoint patrolPoint, 
            IVisionConeControlPoint distractPoint,
            Transform playerTransform)
        {
            this.VisionConeViewModel = visionConeViewModel;
            this.coneVisualizer = coneVisualizer;
            this.playerTransform = playerTransform;

            coneVisualizer.SetSpotState(SpotLightState.Searching);
            visionConeViewModel.StartLookAtCoroutine(FollowPlayer());
        }

        public override void UpdateDetectionMeter(float detectionEscalationMeter) 
        {
            coneVisualizer.SetSpotState(SpotLightState.Searching, detectionEscalationMeter);
        }

        private IEnumerator FollowPlayer()
        {
            while (!VisionConeViewModel.IsPlayerObstructed())
            {
                var deltaVector = playerTransform.position - VisionConeViewModel.CurrentLookAtTarget;
                if (deltaVector.magnitude > FollowPlayerClampValue)
                    deltaVector *= FollowPlayerClampValue / deltaVector.magnitude;
                var newLookatTarget = VisionConeViewModel.CurrentLookAtTarget + deltaVector;
            
                VisionConeViewModel.UpdateCone(newLookatTarget, VisionConeViewModel.FieldOfView);
                yield return null;
            }

            VisionConeViewModel.StartLookAtCoroutine(EvaluateReturningToPatrolling());
        }

        private IEnumerator EvaluateReturningToPatrolling()
        {
            var startTime = Time.time;
            while(Time.time - startTime < EvaluationWaitTime)
            {
                if (VisionConeViewModel.IsPlayerInsideVisionCone() && !VisionConeViewModel.IsPlayerObstructed())
                    VisionConeViewModel.StartLookAtCoroutine(FollowPlayer());

                yield return null;
            }

            VisionConeViewModel.TransitionTo(new VisionConeStatePatrolling());
        }
    }
}