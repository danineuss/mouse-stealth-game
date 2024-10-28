using Audio;
using Enemies.VisionCone;
using Infrastructure;

namespace Enemies.Detection
{
    public abstract class DetectorState
    {
        public abstract EnemyIOTextColor EnemyIOTextColor { get; }
        public abstract EnemySound EnemySound { get; }

        protected IPlayerDetector playerDetector;
        protected IVisionConeViewModel VisionConeViewModel;
        protected IEvents events;

        protected DetectorState(
            IPlayerDetector playerDetector, 
            IVisionConeViewModel visionConeViewModel,
            IEvents events) 
        {
            this.playerDetector = playerDetector;
            this.VisionConeViewModel = visionConeViewModel;
            this.events = events;
        }

        public abstract bool AttemptDistraction(float distractionDuration);
        public abstract void UpdateDetectionState();
    }
}