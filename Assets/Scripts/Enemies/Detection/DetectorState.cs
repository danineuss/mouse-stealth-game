using Audio;
using Enemies.VisionCone;
using Infrastructure;

namespace Enemies.Detection
{
    public abstract class DetectorState
    {
        public abstract EnemyIOTextColor EnemyIOTextColor { get; }
        public abstract EnemySound EnemySound { get; }

        protected readonly IPlayerDetector PlayerDetector;
        protected readonly IVisionConeViewModel VisionConeViewModel;
        protected readonly IEvents Events;

        protected DetectorState(
            IPlayerDetector playerDetector, 
            IVisionConeViewModel visionConeViewModel,
            IEvents events) 
        {
            this.PlayerDetector = playerDetector;
            VisionConeViewModel = visionConeViewModel;
            this.Events = events;
        }

        public abstract bool AttemptDistraction(float distractionDuration);
        public abstract void UpdateDetectionState();
    }
}