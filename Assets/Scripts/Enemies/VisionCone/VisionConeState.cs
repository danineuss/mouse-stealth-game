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
}