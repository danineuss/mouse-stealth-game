using System;
using System.Collections;
using UnityEngine;

public enum DetectorStateEnum {
    Idle,
    Searching,
    Alarmed,
    Distracted
}

public interface IPlayerDetector: IIdentifiable
{
    DetectorStateEnum DetectorStateEnum { get; }

    bool AttemptDistraction();
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
    public Guid ID { get; private set; }

    private IVisionConeVM visionConeVM;
    private EventsMono eventsMono;
    private float kDetectionEscalationSpeed;
    private float kDetectionDeescalationSpeed;

    private DetectorStateEnum detectorStateEnum;
    private bool playerVisible; //TODO: move to Vision Cone.
    private float detectionEscalationMeter = 0.5f;
    private float kDetectPlayerRepetitionDelay = 0.075f;
    private DetectorState detectorState;

    public bool AttemptDistraction()
    {
        return detectorState.AttemptDistraction(visionConeVM);
    }

    public void TransitionTo(DetectorState detectorState)
    {
        this.detectorState = detectorState;
    }

    public void Update()
    {
        SetDetectionState();
    }

    public PlayerDetector(
        IVisionConeVM visionConeVM,
        EventsMono eventsMono,
        float kDetectionEscalationSpeed,
        float kDetectionDeescalationSpeed)
    {
        this.visionConeVM = visionConeVM;
        this.eventsMono = eventsMono;
        this.kDetectionEscalationSpeed = kDetectionEscalationSpeed;
        this.kDetectionDeescalationSpeed = kDetectionDeescalationSpeed;

        DetectorStateEnum = DetectorStateEnum.Idle;
        detectorState = new DetectorStateIdle(this, eventsMono);

        eventsMono.StartCoroutine(DetectPlayerWithDelay(kDetectPlayerRepetitionDelay));
    }

    IEnumerator DetectPlayerWithDelay(float seconds)
    {
        while (true)
        {
            yield return new WaitForSeconds(seconds);
            DetectPlayer();
        }
    }

    void DetectPlayer()
    {
        if (DetectorStateEnum == DetectorStateEnum.Distracted)
            return;

        bool wasPlayerPreviouslyVisible = playerVisible;
        if (!visionConeVM.IsPlayerInsideVisionCone())
        {
            playerVisible = false;
            if (wasPlayerPreviouslyVisible)
                visionConeVM.ResetToPatrolling();
            return;
        }

        if (visionConeVM.IsPlayerObstructed())
        {
            playerVisible = false;
            if (wasPlayerPreviouslyVisible)
                visionConeVM.ResetToPatrolling();
            return;
        }

        playerVisible = true;
        visionConeVM.StartFollowingPlayer();
    }

    void SetDetectionState()
    {
        if (DetectorStateEnum == DetectorStateEnum.Idle && playerVisible)
            DetectorStateEnum = DetectorStateEnum.Searching;

        if (DetectorStateEnum != DetectorStateEnum.Distracted)
            SetDetectionEscalationMeter();

        visionConeVM.SetSpotState(DetectorStateEnum, detectionEscalationMeter);
    }

    void SetDetectionEscalationMeter()
    {
        if (playerVisible)
            detectionEscalationMeter += Time.deltaTime * kDetectionEscalationSpeed;
        else
            detectionEscalationMeter -= Time.deltaTime * kDetectionDeescalationSpeed;


        if (detectionEscalationMeter >= 1f)
            EscelateDetection();

        if (detectionEscalationMeter <= 0.01f)
            DeescalateDetection();

        detectionEscalationMeter = Mathf.Clamp(detectionEscalationMeter, 0f, 1f);
    }

    void EscelateDetection()
    {
        if (DetectorStateEnum == DetectorStateEnum.Searching)
            DetectorStateEnum = DetectorStateEnum.Alarmed;
        detectionEscalationMeter = 0f;
    }

    void DeescalateDetection()
    {
        if (DetectorStateEnum == DetectorStateEnum.Searching && !playerVisible)
            DetectorStateEnum = DetectorStateEnum.Idle;
    }
}
