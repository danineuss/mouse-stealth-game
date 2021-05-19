using System.Collections;
using UnityEngine;

public abstract class DetectorState
{
    public abstract EnemyIOTextColor EnemyIOTextColor { get; }
    public abstract EnemySound EnemySound { get; }

    protected IPlayerDetector playerDetector;
    protected IVisionConeVM visionConeVM;
    protected EventsMono eventsMono;

    protected DetectorState(
        IPlayerDetector playerDetector, 
        IVisionConeVM visionConeVM,
        EventsMono eventsMono) 
    {
        this.playerDetector = playerDetector;
        this.visionConeVM = visionConeVM;
        this.eventsMono = eventsMono;
    }

    public abstract bool AttemptDistraction(float distractionDuration);
    public abstract void UpdateDetectionState();
}

public class DetectorStateIdle: DetectorState
{
    public override EnemySound EnemySound => EnemySound.Idle;
    public override EnemyIOTextColor EnemyIOTextColor => EnemyIOTextColor.Active;

    public DetectorStateIdle(
        IPlayerDetector playerDetector,
        IVisionConeVM visionConeVM,
        EventsMono eventsMono) 
        : base(playerDetector, visionConeVM, eventsMono) 
    {
        visionConeVM.SetSpotState(SpotLightState.Idle);
    }

    public override bool AttemptDistraction(float distractionDuration)
    {
        playerDetector.TransitionTo(new DetectorStateDistracted(
            playerDetector, 
            visionConeVM,
            eventsMono,
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
            eventsMono,
            playerDetector.DetectionEscalationSpeed,
            playerDetector.DetectionDeescalationSpeed)
        );
    }
}

public class DetectorStateSearching : DetectorState
{
    public override EnemyIOTextColor EnemyIOTextColor => EnemyIOTextColor.Inactive;
    public override EnemySound EnemySound => EnemySound.Searching;

    private readonly float DetectionEscalationSpeed;
    private readonly float DetectionDeescalationSpeed;
    private float detectionEscalationMeter;

    public DetectorStateSearching(
        IPlayerDetector playerDetector, 
        IVisionConeVM visionConeVM,
        EventsMono eventsMono, 
        float DetectionEscalationSpeed,
        float DetectionDeescalationSpeed)
        : base(playerDetector, visionConeVM, eventsMono)
    {
        this.DetectionEscalationSpeed = DetectionEscalationSpeed;
        this.DetectionDeescalationSpeed = DetectionDeescalationSpeed;
        
        StartFollowingPlayer();
    }

    public override bool AttemptDistraction(float distractionDuration)
    {
        return false;
    }

    public override void UpdateDetectionState()
    {
        if (visionConeVM.IsPlayerObstructed())
            detectionEscalationMeter -= Time.deltaTime * DetectionDeescalationSpeed;
        else
            detectionEscalationMeter += Time.deltaTime * DetectionEscalationSpeed;

        if (detectionEscalationMeter >= 1f)
            EscelateDetection();

        if (detectionEscalationMeter <= 0.001f)
            DeescalateDetection();

        detectionEscalationMeter = Mathf.Clamp(detectionEscalationMeter, 0f, 1f);
    }

    private void StartFollowingPlayer()
    {
        this.detectionEscalationMeter = 0f;
        visionConeVM.StartFollowingPlayer();
        visionConeVM.SetSpotState(SpotLightState.Searching);
    }

    void EscelateDetection()
    {
        playerDetector.TransitionTo(new DetectorStateAlarmed(
            playerDetector, 
            visionConeVM,
            eventsMono)
        );
    }

    void DeescalateDetection()
    {
        visionConeVM.ResetToPatrolling();
        playerDetector.TransitionTo(new DetectorStateIdle(
            playerDetector, 
            visionConeVM,
            eventsMono)
        );
    }
}

public class DetectorStateAlarmed : DetectorState
{
    public override EnemyIOTextColor EnemyIOTextColor => EnemyIOTextColor.Inactive;
    public override EnemySound EnemySound => EnemySound.Alarmed;

    public DetectorStateAlarmed(
        IPlayerDetector playerDetector,
        IVisionConeVM visionConeVM,
        EventsMono eventsMono) 
        : base(playerDetector, visionConeVM, eventsMono)
    {
        visionConeVM.SetSpotState(SpotLightState.Alarmed);
    }

    public override bool AttemptDistraction(float distractionDuration)
    {
        return false;
    }

    public override void UpdateDetectionState()
    {
        eventsMono.EnemyEvents.FailGame();
    }
}

public class DetectorStateDistracted : DetectorState
{
    public override EnemyIOTextColor EnemyIOTextColor => EnemyIOTextColor.Inactive;
    public override EnemySound EnemySound => EnemySound.Distracted;

    private float timeOfLastDistraction;

    public DetectorStateDistracted(
        IPlayerDetector playerDetector, 
        IVisionConeVM visionConeVM,
        EventsMono eventsMono,
        float distractionDuration) 
        : base(playerDetector, visionConeVM, eventsMono)
    {
        visionConeVM.SetSpotState(SpotLightState.Distracted);
        visionConeVM.SetStateDistracted(true);

        timeOfLastDistraction = Time.time;
        eventsMono.StartCoroutine(ResetDistraction(distractionDuration));
    }

    public override bool AttemptDistraction(float distractionDuration)
    {
        return false;
    }

    public override void UpdateDetectionState()
    {
        return;
    }

    IEnumerator ResetDistraction(float distractionDuration)
    {
        while (Time.time - timeOfLastDistraction < distractionDuration)
            yield return null;

        visionConeVM.SetStateDistracted(false);
        playerDetector.TransitionTo(new DetectorStateIdle(
            playerDetector, 
            visionConeVM, 
            eventsMono)
        );
    }
}