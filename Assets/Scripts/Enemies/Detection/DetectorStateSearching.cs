using Audio;
using Enemies.VisionCone;
using Infrastructure;
using UnityEngine;

namespace Enemies.Detection
{
    public class DetectorStateSearching : DetectorState
    {
        public override EnemyIOTextColor EnemyIOTextColor => EnemyIOTextColor.Inactive;
        public override EnemySound EnemySound => EnemySound.Searching;

        private readonly float DetectionEscalationSpeed;
        private readonly float DetectionDeescalationSpeed;
        private readonly float FloatingPointDelta = 0.005f;
        private float detectionMeter;

        public DetectorStateSearching(
            IPlayerDetector playerDetector, 
            IVisionConeVM visionConeVM,
            IEvents events, 
            float DetectionEscalationSpeed,
            float DetectionDeescalationSpeed)
            : base(playerDetector, visionConeVM, events)
        {
            this.DetectionEscalationSpeed = DetectionEscalationSpeed;
            this.DetectionDeescalationSpeed = DetectionDeescalationSpeed;
        
            this.detectionMeter = 0f;
            visionConeVM.TransitionTo(new VisionConeStateFollowingPlayer());
        }

        public override bool AttemptDistraction(float distractionDuration)
        {
            return false;
        }

        public override void UpdateDetectionState()
        {
            UpdateDetectionMeter();
            EscalateOrDeescalateDetection();

            visionConeVM.UpdateDetectionMeter(detectionMeter);
        }

        private void UpdateDetectionMeter()
        {
            if (visionConeVM.IsPlayerObstructed())
                detectionMeter -= Time.deltaTime * DetectionDeescalationSpeed;
            else
                detectionMeter += Time.deltaTime * DetectionEscalationSpeed;
        
            detectionMeter = Mathf.Clamp(detectionMeter, 0f, 1f);
        }

        private void EscalateOrDeescalateDetection()
        {
            if (detectionMeter >= 1f - FloatingPointDelta)
            {
                playerDetector.TransitionTo(new DetectorStateAlarmed(
                    playerDetector, 
                    visionConeVM,
                    events)
                );
            }
            else if (detectionMeter <= 0f + FloatingPointDelta)
            {
                playerDetector.TransitionTo(new DetectorStateIdle(
                    playerDetector, 
                    visionConeVM,
                    events)
                );
            }
        }
    }
}