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
}

public class PlayerDetector : MonoBehaviour, IPlayerDetector
{
    [SerializeField] private Transform player = null;
    [SerializeField] private EventsMono eventsMono = null;
    [SerializeField] private LayerMask obstacleMask = new LayerMask();
    [SerializeField, Range(0.01f, 0.5f)] private float kDetectionEscalationSpeed = 0.1f;
    [SerializeField, Range(0.01f, 0.5f)] private float kDetectionDeescalationSpeed = 0.02f;
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

    private DetectorStateEnum detectorStateEnum;
    private VisionConeVM visionConeVM;
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
        StartCoroutine(ResetDistraction());
    }

    IEnumerator ResetDistraction()
    {
        while (Time.time - timeOfLastDistraction < kDistractionDuration)
            yield return null;

        DetectorStateEnum = DetectorStateEnum.Idle;
        visionConeVM.SetStateDistracted(false);
    }

    void Awake()
    {
        visionConeVM = GetComponent<VisionConeVM>();
    }

    void Start()
    {
        DetectorStateEnum = DetectorStateEnum.Idle;

        StartCoroutine(DetectPlayerWithDelay(kDetectPlayerRepetitionDelay));
    }

    void Update()
    {
        SetDetectionState();
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
            transform.position,
            (player.transform.position - transform.position).normalized,
            Vector3.Distance(player.transform.position, transform.position),
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
        float angleToPlayer = Vector3.Angle(visionConeVM.CurrentLookatTarget - transform.position,
                                            player.transform.position - transform.position);
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
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
