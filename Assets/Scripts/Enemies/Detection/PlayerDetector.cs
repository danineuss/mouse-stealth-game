using System;

public enum DetectorStateEnum {
    Idle,
    Searching,
    Alarmed,
    Distracted
}

public interface IPlayerDetector: IIdentifiable
{
    DetectorStateEnum DetectorStateEnum { get; }
    IVisionConeVM VisionConeVM { get; }
    float DetectionEscalationSpeed { get; }
    float DetectionDeescalationSpeed { get; }

    bool AttemptDistraction(float distractionDuration);
    void TransitionTo(DetectorState detectorState);
    void Update();
}

public class PlayerDetector : IPlayerDetector
{
    public DetectorStateEnum DetectorStateEnum
    {
        get => detectorStateEnum;
        private set
        {
            if (detectorStateEnum == value)
                return;

            detectorStateEnum = value;
            eventsMono.EnemyEvents.ChangeDetectorState(this.ID);
        }
    }
    public DetectorState DetectorState => detectorState;
    public IVisionConeVM VisionConeVM => visionConeVM;
    public Guid ID { get; }
    public float DetectionEscalationSpeed { get; }
    public float DetectionDeescalationSpeed { get; }

    private IVisionConeVM visionConeVM;
    private EventsMono eventsMono;

    private DetectorStateEnum detectorStateEnum;
    private DetectorState detectorState;

    public bool AttemptDistraction(float distractionDuration)
    {
        return detectorState.AttemptDistraction(distractionDuration);
    }

    public void TransitionTo(DetectorState detectorState)
    {
        this.detectorState = detectorState;
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

        DetectorStateEnum = DetectorStateEnum.Idle;
        detectorState = new DetectorStateIdle(this, eventsMono);
    }
}
