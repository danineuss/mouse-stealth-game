using System;

public interface IPlayerDetector: IIdentifiable
{
    DetectorState DetectorState { get; }
    float DetectionEscalationSpeed { get; }
    float DetectionDeescalationSpeed { get; }

    bool AttemptDistraction(float distractionDuration);
    void TransitionTo(DetectorState detectorState);
    void Update();
}

public class PlayerDetector : IPlayerDetector
{
    public DetectorState DetectorState => detectorState;
    public Guid ID { get; }
    public float DetectionEscalationSpeed { get; }
    public float DetectionDeescalationSpeed { get; }

    private IVisionConeVM visionConeVM;
    private EventsMono eventsMono;
    private DetectorState detectorState;

    public bool AttemptDistraction(float distractionDuration)
    {
        return detectorState.AttemptDistraction(distractionDuration);
    }

    public void TransitionTo(DetectorState detectorState)
    {
        this.detectorState = detectorState;
        eventsMono.EnemyEvents.ChangeDetectorState(this.ID);
    }

    public void Update()
    {
        detectorState.UpdateDetectionState();
    }

    public PlayerDetector(
        IVisionConeVM visionConeVM,
        EventsMono eventsMono,
        float DetectionEscalationSpeed,
        float DetectionDeescalationSpeed)
    {
        this.visionConeVM = visionConeVM;
        this.eventsMono = eventsMono;
        this.DetectionEscalationSpeed = DetectionEscalationSpeed;
        this.DetectionDeescalationSpeed = DetectionDeescalationSpeed;
        this.ID = Guid.NewGuid();

        detectorState = new DetectorStateIdle(this, visionConeVM, eventsMono);
    }
}
