using System;
using Enemies.VisionCone;
using Infrastructure;

namespace Enemies.Detection
{
    public interface IPlayerDetector: IIdentifiable, IUpdatable
    {
        DetectorState DetectorState { get; }
        float DetectionEscalationSpeed { get; }
        float DetectionDeescalationSpeed { get; }

        bool AttemptDistraction(float distractionDuration);
        void TransitionTo(DetectorState detectorState);
    }

    public class PlayerDetector : IPlayerDetector
    {
        public DetectorState DetectorState => detectorState;
        public Guid ID { get; }
        public float DetectionEscalationSpeed { get; }
        public float DetectionDeescalationSpeed { get; }

        private DetectorState detectorState;
        
        private readonly IEvents events;

        public bool AttemptDistraction(float distractionDuration)
        {
            return detectorState.AttemptDistraction(distractionDuration);
        }

        public void TransitionTo(DetectorState detectorState)
        {
            this.detectorState = detectorState;
            events.EnemyEvents.ChangeDetectorState(ID);
        }

        public void Update()
        {
            detectorState.UpdateDetectionState();
        }

        public PlayerDetector(
            IVisionConeViewModel visionConeViewModel,
            IEvents events,
            float detectionEscalationSpeed,
            float detectionDeescalationSpeed)
        {
            this.events = events;
            DetectionEscalationSpeed = detectionEscalationSpeed;
            DetectionDeescalationSpeed = detectionDeescalationSpeed;
            ID = Guid.NewGuid();

            TransitionTo(new DetectorStateIdle(this, visionConeViewModel, events));
        }
    }
}