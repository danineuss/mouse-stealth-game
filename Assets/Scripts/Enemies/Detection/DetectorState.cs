using System.Collections;
using UnityEngine;

public abstract class DetectorState
{
    public abstract EnemyIOTextColor EnemyIOTextColor { get; }
    public abstract EnemySound EnemySound { get; }

    protected IPlayerDetector playerDetector;
    protected IVisionConeVM visionConeVM;
    protected IEvents events;

    protected DetectorState(
        IPlayerDetector playerDetector, 
        IVisionConeVM visionConeVM,
        IEvents events) 
    {
        this.playerDetector = playerDetector;
        this.visionConeVM = visionConeVM;
        this.events = events;
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
        IEvents events) 
        : base(playerDetector, visionConeVM, events) 
    {
        visionConeVM.SetSpotState(SpotLightState.Idle);
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
        IEvents events, 
        float DetectionEscalationSpeed,
        float DetectionDeescalationSpeed)
        : base(playerDetector, visionConeVM, events)
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
            events)
        );
    }

    void DeescalateDetection()
    {
        visionConeVM.ResetToPatrolling();
        playerDetector.TransitionTo(new DetectorStateIdle(
            playerDetector, 
            visionConeVM,
            events)
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
        IEvents events) 
        : base(playerDetector, visionConeVM, events)
    {
        visionConeVM.SetSpotState(SpotLightState.Alarmed);
    }

    public override bool AttemptDistraction(float distractionDuration)
    {
        return false;
    }

    public override void UpdateDetectionState()
    {
        events.EnemyEvents.FailGame();
    }
}

public class DetectorStateDistracted : DetectorState
{
    public override EnemyIOTextColor EnemyIOTextColor => EnemyIOTextColor.Inactive;
    public override EnemySound EnemySound => EnemySound.Distracted;

    public DetectorStateDistracted(
        IPlayerDetector playerDetector, 
        IVisionConeVM visionConeVM,
        IEvents events,
        float distractionDuration) 
        : base(playerDetector, visionConeVM, events)
    {
        visionConeVM.SetSpotState(SpotLightState.Distracted);
        visionConeVM.SetStateDistracted(true);

        events.StartCoroutine(ResetDistraction(distractionDuration));
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
        yield return new WaitForSeconds(distractionDuration);

        visionConeVM.SetStateDistracted(false);
        playerDetector.TransitionTo(new DetectorStateIdle(
            playerDetector, 
            visionConeVM, 
            events)
        );
    }
}