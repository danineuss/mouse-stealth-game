using System;
using System.Collections;
using UnityEngine;

public abstract class DetectorState
{
    protected IPlayerDetector playerDetector;
    protected EventsMono eventsMono;

    protected DetectorState(IPlayerDetector playerDetector, EventsMono eventsMono) 
    {
        this.playerDetector = playerDetector;
        this.eventsMono = eventsMono;
    }

    public abstract bool AttemptDistraction(float distractionDuration);

    public abstract void UpdateDetectionState();
}

public class DetectorStateIdle: DetectorState
{
    public DetectorStateIdle(
        IPlayerDetector playerDetector,
        EventsMono eventsMono) 
        : base(playerDetector, eventsMono) 
    {
        SetupIdleState();
    }

    public override bool AttemptDistraction(float distractionDuration)
    {
        playerDetector.VisionConeVM.SetStateDistracted(true);
        playerDetector.TransitionTo(new DetectorStateDistracted(
            playerDetector, 
            eventsMono,
            distractionDuration)
        );
        return true;
    }

    public override void UpdateDetectionState()
    {
        if (!playerDetector.VisionConeVM.IsPlayerInsideVisionCone())
            return;
        
        if (playerDetector.VisionConeVM.IsPlayerObstructed())
            return;

        playerDetector.TransitionTo(new DetectorStateSearching(
            playerDetector, 
            eventsMono,
            playerDetector.DetectionEscalationSpeed,
            playerDetector.DetectionDeescalationSpeed)
        );
    }

    private void SetupIdleState()
    {
        playerDetector.VisionConeVM.SetSpotState(DetectorStateEnum.Idle);
    }
}

public class DetectorStateSearching : DetectorState
{
    private readonly float DetectionEscalationSpeed;
    private readonly float DetectionDeescalationSpeed;
    private float detectionEscalationMeter;

    public DetectorStateSearching(
        IPlayerDetector playerDetector, 
        EventsMono eventsMono, 
        float DetectionEscalationSpeed,
        float DetectionDeescalationSpeed)
        : base(playerDetector, eventsMono)
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
        if (playerDetector.VisionConeVM.IsPlayerObstructed())
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
        playerDetector.VisionConeVM.StartFollowingPlayer();
        playerDetector.VisionConeVM.SetSpotState(DetectorStateEnum.Searching);
    }

    void EscelateDetection()
    {
        playerDetector.TransitionTo(new DetectorStateAlarmed(playerDetector, eventsMono));
    }

    void DeescalateDetection()
    {
        playerDetector.VisionConeVM.ResetToPatrolling();
        playerDetector.VisionConeVM.SetSpotState(DetectorStateEnum.Idle);
        playerDetector.TransitionTo(new DetectorStateIdle(playerDetector, eventsMono));
    }
}

public class DetectorStateAlarmed : DetectorState
{
    public DetectorStateAlarmed(IPlayerDetector playerDetector, EventsMono eventsMono) 
        : base(playerDetector, eventsMono)
    {
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
    private float timeOfLastDistraction;

    public DetectorStateDistracted(
        IPlayerDetector playerDetector, 
        EventsMono eventsMono,
        float distractionDuration) 
        : base(playerDetector, eventsMono)
    {
        playerDetector.VisionConeVM.SetSpotState(DetectorStateEnum.Distracted);

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

        playerDetector.VisionConeVM.SetStateDistracted(false);
        playerDetector.TransitionTo(new DetectorStateIdle(playerDetector, eventsMono));
    }
}