using UnityEngine;

namespace Enemies.VisionCone
{
    public abstract class VisionConeState
    {
        protected IVisionConeViewModel VisionConeViewModel;
        protected IConeVisualizer ConeVisualizer;

        public abstract void SetupVisionConeState(
            IVisionConeViewModel visionConeViewModel, 
            IConeVisualizer coneVisualizer,
            IVisionConePatrolPoint patrolPoint, 
            IVisionConeControlPoint distractPoint,
            Transform playerTransform
        );

        public abstract void UpdateDetectionMeter(float detectionMeter);
    }
}