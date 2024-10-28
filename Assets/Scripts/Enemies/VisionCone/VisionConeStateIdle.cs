using UnityEngine;

namespace Enemies.VisionCone
{
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
}