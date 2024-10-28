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
            IVisionConeViewModel visionConeViewModel,
            IEvents events) 
            : base(playerDetector, visionConeViewModel, events) {}

        public override bool AttemptDistraction(float distractionDuration)
        {
            return false;
        }

        public override void UpdateDetectionState()
        {
            Events.EnemyEvents.FailGame();
        }
    }
}