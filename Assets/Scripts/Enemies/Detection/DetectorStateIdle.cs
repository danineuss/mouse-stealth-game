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
            IVisionConeViewModel visionConeViewModel,
            IEvents events) 
            : base(playerDetector, visionConeViewModel, events) 
        {
            visionConeViewModel.TransitionTo(new VisionConeStateIdle());
        }

        public override bool AttemptDistraction(float distractionDuration)
        {
            PlayerDetector.TransitionTo(new DetectorStateDistracted(
                PlayerDetector, 
                VisionConeViewModel,
                Events,
                distractionDuration)
            );
            return true;
        }

        public override void UpdateDetectionState()
        {
            if (!VisionConeViewModel.IsPlayerInsideVisionCone())
                return;
        
            if (VisionConeViewModel.IsPlayerObstructed())
                return;

            PlayerDetector.TransitionTo(new DetectorStateSearching(
                PlayerDetector, 
                VisionConeViewModel,
                Events,
                PlayerDetector.DetectionEscalationSpeed,
                PlayerDetector.DetectionDeescalationSpeed)
            );
        }
    }
}