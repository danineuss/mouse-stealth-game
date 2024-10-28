using UnityEngine;

namespace Enemies.VisionCone
{
    public class VisionConeStateDistracted : VisionConeState
    {
        private IVisionConeControlPoint distractPoint;
        private readonly float LerpToDistractDuration = 1f;

        public override void SetupVisionConeState(
            IVisionConeViewModel visionConeViewModel, 
            IConeVisualizer coneVisualizer,
            IVisionConePatrolPoint patrolPoint, 
            IVisionConeControlPoint distractPoint,
            Transform playerTransform)
        {
            VisionConeViewModel = visionConeViewModel;
            this.ConeVisualizer = coneVisualizer;
            this.distractPoint = distractPoint;

            LookToDistraction();
        }

        public override void UpdateDetectionMeter(float detectionEscalationMeter) {}

        void LookToDistraction()
        {
            ConeVisualizer.SetSpotState(SpotLightState.Distracted);

            var distraction = VisionConeViewModel.LerpTowardsTarget(
                distractPoint.Position, distractPoint.FieldOfView, LerpToDistractDuration
            );
            VisionConeViewModel.StartLookAtCoroutine(distraction);
        }
    }
}