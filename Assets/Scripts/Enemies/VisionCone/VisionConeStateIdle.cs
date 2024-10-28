using UnityEngine;

namespace Enemies.VisionCone
{
    public class VisionConeStateIdle : VisionConeState
    {
        private IVisionConePatrolPoint nextPatrolPoint;
        public override void SetupVisionConeState(
            IVisionConeViewModel visionConeViewModel, 
            IConeVisualizer coneVisualizer,
            IVisionConePatrolPoint patrolPoint, 
            IVisionConeControlPoint distractPoint,
            Transform playerTransform)
        {
            VisionConeViewModel = visionConeViewModel;
            this.ConeVisualizer = coneVisualizer;        
            nextPatrolPoint = patrolPoint;

            coneVisualizer.SetSpotState(SpotLightState.Idle);
            EvaluatePatrolStart();
        }

        public override void UpdateDetectionMeter(float detectionEscalationMeter) {}

        private void EvaluatePatrolStart()
        {    
            if (nextPatrolPoint.Position == VisionConeViewModel.CurrentLookAtTarget && 
                nextPatrolPoint.FieldOfView == VisionConeViewModel.FieldOfView)
                return;
        
            VisionConeViewModel.TransitionTo(new VisionConeStatePatrolling());
        }
    }
}