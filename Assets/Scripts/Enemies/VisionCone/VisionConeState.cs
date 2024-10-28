using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies.VisionCone
{
    public abstract class VisionConeState
    {
        protected IVisionConeVM visionConeVM;
        protected IConeVisualizer coneVisualizer;

        public abstract void SetupVisionConeState(
            IVisionConeVM visionConeVM, 
            IConeVisualizer coneVisualizer,
            IVisionConePatrolPoint patrolPoint, 
            IVisionConeControlPoint distractPoint,
            Transform playerTransform
        );

        public abstract void UpdateDetectionMeter(float detectionMeter);
    }

    public class VisionConeStateIdle : VisionConeState
    {
        private IVisionConePatrolPoint nextPatrolPoint;
        public override void SetupVisionConeState(
            IVisionConeVM visionConeVM, 
            IConeVisualizer coneVisualizer,
            IVisionConePatrolPoint patrolPoint, 
            IVisionConeControlPoint distractPoint,
            Transform playerTransform)
        {
            this.visionConeVM = visionConeVM;
            this.coneVisualizer = coneVisualizer;        
            this.nextPatrolPoint = patrolPoint;

            coneVisualizer.SetSpotState(SpotLightState.Idle);
            EvaluatePatrolStart();
        }

        public override void UpdateDetectionMeter(float detectionEscalationMeter) {}

        private void EvaluatePatrolStart()
        {    
            if (nextPatrolPoint.Position == visionConeVM.CurrentLookatTarget && 
                nextPatrolPoint.FieldOfView == visionConeVM.FieldOfView)
                return;
        
            visionConeVM.TransitionTo(new VisionConeStatePatrolling());
        }
    }

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

    public class VisionConeStateDistracted : VisionConeState
    {
        private IVisionConeControlPoint distractPoint;
        private readonly float LerpToDistractDuration = 1f;

        public override void SetupVisionConeState(
            IVisionConeVM visionConeVM, 
            IConeVisualizer coneVisualizer,
            IVisionConePatrolPoint patrolPoint, 
            IVisionConeControlPoint distractPoint,
            Transform playerTransform)
        {
            this.visionConeVM = visionConeVM;
            this.coneVisualizer = coneVisualizer;
            this.distractPoint = distractPoint;

            LookToDistraction();
        }

        public override void UpdateDetectionMeter(float detectionEscalationMeter) {}

        void LookToDistraction()
        {
            coneVisualizer.SetSpotState(SpotLightState.Distracted);

            var distraction = visionConeVM.LerpTowardsTarget(
                distractPoint.Position, distractPoint.FieldOfView, LerpToDistractDuration
            );
            visionConeVM.StartLookatCoroutine(distraction);
        }
    }
}