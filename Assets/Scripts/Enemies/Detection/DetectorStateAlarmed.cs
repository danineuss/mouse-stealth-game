using Audio;
using Enemies.VisionCone;
using Infrastructure;

namespace Enemies.Detection
{
    public class DetectorStateAlarmed : DetectorState
    {
        public override EnemyIOTextColor EnemyIOTextColor => EnemyIOTextColor.Inactive;
        public override EnemySound EnemySound => EnemySound.Alarmed;

        public DetectorStateAlarmed(
            IPlayerDetector playerDetector,
            IVisionConeVM visionConeVM,
            IEvents events) 
            : base(playerDetector, visionConeVM, events) {}

        public override bool AttemptDistraction(float distractionDuration)
        {
            return false;
        }

        public override void UpdateDetectionState()
        {
            events.EnemyEvents.FailGame();
        }
    }
}