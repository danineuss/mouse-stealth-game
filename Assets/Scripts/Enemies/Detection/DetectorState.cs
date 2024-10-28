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
        protected IVisionConeVM visionConeVM;
        protected IEvents events;

        protected DetectorState(
            IPlayerDetector playerDetector, 
            IVisionConeVM visionConeVM,
            IEvents events) 
        {
            this.playerDetector = playerDetector;
            this.visionConeVM = visionConeVM;
            this.events = events;
        }

        public abstract bool AttemptDistraction(float distractionDuration);
        public abstract void UpdateDetectionState();
    }
}