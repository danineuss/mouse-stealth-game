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

        private float detectionMeter;
        
        private readonly float detectionEscalationSpeed;
        private readonly float detectionDeescalationSpeed;
        
        private const float FloatingPointDelta = 0.005f;

        public DetectorStateSearching(
            IPlayerDetector playerDetector, 
            IVisionConeViewModel visionConeViewModel,
            IEvents events, 
            float detectionEscalationSpeed,
            float detectionDeescalationSpeed)
            : base(playerDetector, visionConeViewModel, events)
        {
            this.detectionEscalationSpeed = detectionEscalationSpeed;
            this.detectionDeescalationSpeed = detectionDeescalationSpeed;
        
            detectionMeter = 0f;
            visionConeViewModel.TransitionTo(new VisionConeStateFollowingPlayer());
        }

        public override bool AttemptDistraction(float distractionDuration)
        {
            return false;
        }

        public override void UpdateDetectionState()
        {
            UpdateDetectionMeter();
            EscalateOrDeescalateDetection();

            VisionConeViewModel.UpdateDetectionMeter(detectionMeter);
        }

        private void UpdateDetectionMeter()
        {
            if (VisionConeViewModel.IsPlayerObstructed())
                detectionMeter -= Time.deltaTime * detectionDeescalationSpeed;
            else
                detectionMeter += Time.deltaTime * detectionEscalationSpeed;
        
            detectionMeter = Mathf.Clamp(detectionMeter, 0f, 1f);
        }

        private void EscalateOrDeescalateDetection()
        {
            if (detectionMeter >= 1f - FloatingPointDelta)
            {
                PlayerDetector.TransitionTo(new DetectorStateAlarmed(
                    PlayerDetector, 
                    VisionConeViewModel,
                    Events)
                );
            }
            else if (detectionMeter <= 0f + FloatingPointDelta)
            {
                PlayerDetector.TransitionTo(new DetectorStateIdle(
                    PlayerDetector, 
                    VisionConeViewModel,
                    Events)
                );
            }
        }
    }
}