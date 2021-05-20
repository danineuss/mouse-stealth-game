using System;

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

    private IVisionConeVM visionConeVM;
    private IEvents events;
    private DetectorState detectorState;

    public bool AttemptDistraction(float distractionDuration)
    {
        return detectorState.AttemptDistraction(distractionDuration);
    }

    public void TransitionTo(DetectorState detectorState)
    {
        this.detectorState = detectorState;
        events.EnemyEvents.ChangeDetectorState(this.ID);
    }

    public void Update()
    {
        detectorState.UpdateDetectionState();
    }

    public PlayerDetector(
        IVisionConeVM visionConeVM,
        IEvents events,
        float DetectionEscalationSpeed,
        float DetectionDeescalationSpeed)
    {
        this.visionConeVM = visionConeVM;
        this.events = events;
        this.DetectionEscalationSpeed = DetectionEscalationSpeed;
        this.DetectionDeescalationSpeed = DetectionDeescalationSpeed;
        this.ID = Guid.NewGuid();

        TransitionTo(new DetectorStateIdle(this, visionConeVM, events));
    }
}
