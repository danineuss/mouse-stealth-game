using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DetectorStateEnum {
    Idle,
    Searching,
    Alarmed,
    Distracted
}

public abstract class DetectorState
{
    
}

public interface IPlayerDetector
{
    DetectorStateEnum DetectorStateEnum { get; }

    void SetStateDistracted();
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
            eventsMono.EnemyEvents.ChangeDetectorState(this);
        }
    }

    private IVisionConeVM visionConeVM;
    private EventsMono eventsMono;
    private LayerMask obstacleMask;
    private Transform player;
    private Transform detectorTransform;
    private float kDetectionEscalationSpeed;
    private float kDetectionDeescalationSpeed;

    private DetectorStateEnum detectorStateEnum;
    private bool playerVisible;
    private float detectionEscalationMeter = 0.5f;
    private float kDetectPlayerRepetitionDelay = 0.075f;
    private float kDistractionDuration = 5f;
    private float timeOfLastDistraction = 0f;

    public void SetStateDistracted()
    {
        if (DetectorStateEnum == DetectorStateEnum.Searching)
            return;

        DetectorStateEnum = DetectorStateEnum.Distracted;
        visionConeVM.SetStateDistracted(true);

        timeOfLastDistraction = Time.time;
        eventsMono.StartCoroutine(ResetDistraction());
    }

    public void Update()
    {
        SetDetectionState();
    }

    public PlayerDetector(
        IVisionConeVM visionConeVM,
        EventsMono eventsMono,
        LayerMask obstacleMask,
        Transform player,
        Transform detectorTransform,
        float kDetectionEscalationSpeed,
        float kDetectionDeescalationSpeed)
    {
        this.visionConeVM = visionConeVM;
        this.eventsMono = eventsMono;
        this.obstacleMask = obstacleMask;
        this.player = player;
        this.detectorTransform = detectorTransform;
        this.kDetectionEscalationSpeed = kDetectionEscalationSpeed;
        this.kDetectionDeescalationSpeed = kDetectionDeescalationSpeed;

        DetectorStateEnum = DetectorStateEnum.Idle;

        eventsMono.StartCoroutine(DetectPlayerWithDelay(kDetectPlayerRepetitionDelay));
    }
    IEnumerator ResetDistraction()
    {
        while (Time.time - timeOfLastDistraction < kDistractionDuration)
            yield return null;

        DetectorStateEnum = DetectorStateEnum.Idle;
        visionConeVM.SetStateDistracted(false);
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
        if (PlayerOutsideVisibleCone())
        {
            playerVisible = false;
            if (wasPlayerPreviouslyVisible)
                visionConeVM.ResetToPatrolling();
            return;
        }

        bool playerObstructed = Physics.Raycast(
            detectorTransform.position,
            (player.transform.position - detectorTransform.position).normalized,
            Vector3.Distance(player.transform.position, detectorTransform.position),
            obstacleMask
        );

        if (playerObstructed)
        {
            playerVisible = false;
            if (wasPlayerPreviouslyVisible)
                visionConeVM.ResetToPatrolling();
            return;
        }

        playerVisible = true;
        visionConeVM.SetPlayerAsTarget(player);
    }

    bool PlayerOutsideVisibleCone()
    {
        float angleToPlayer = Vector3.Angle(
            visionConeVM.CurrentLookatTarget - detectorTransform.position,
            player.transform.position - detectorTransform.position
        );
        float distanceToPlayer = Vector3.Distance(player.transform.position, detectorTransform.position);
        return (angleToPlayer > visionConeVM.FieldOfView / 2 || distanceToPlayer > visionConeVM.Range);
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
