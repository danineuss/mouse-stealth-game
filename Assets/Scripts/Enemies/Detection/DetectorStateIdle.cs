using Audio;
using Enemies.VisionCone;
using Infrastructure;

namespace Enemies.Detection
{
    public class DetectorStateIdle : DetectorState
    {
        public override EnemySound EnemySound => EnemySound.Idle;
        public override EnemyIOTextColor EnemyIOTextColor => EnemyIOTextColor.Active;

        public DetectorStateIdle(
            IPlayerDetector playerDetector,
            IVisionConeVM visionConeVM,
            IEvents events) 
            : base(playerDetector, visionConeVM, events) 
        {
            visionConeVM.TransitionTo(new VisionConeStateIdle());
        }

        public override bool AttemptDistraction(float distractionDuration)
        {
            playerDetector.TransitionTo(new DetectorStateDistracted(
                playerDetector, 
                visionConeVM,
                events,
                distractionDuration)
            );
            return true;
        }

        public override void UpdateDetectionState()
        {
            if (!visionConeVM.IsPlayerInsideVisionCone())
                return;
        
            if (visionConeVM.IsPlayerObstructed())
                return;

            playerDetector.TransitionTo(new DetectorStateSearching(
                playerDetector, 
                visionConeVM,
                events,
                playerDetector.DetectionEscalationSpeed,
                playerDetector.DetectionDeescalationSpeed)
            );
        }
    }
}