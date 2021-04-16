using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DetectorState {
    Idle,
    Searching,
    Alarmed,
    Distracted
}

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] private Transform Player;
    [SerializeField] private LayerMask ObstacleMask;
    [SerializeField, Range(0.01f, 0.5f)] private float kDetectionEscalationSpeed = 0.1f;
    [SerializeField, Range(0.01f, 0.5f)] private float kDetectionDeescalationSpeed = 0.02f;
    public DetectorState DetectorState { 
        get => detectorState;
        private set {
            detectorState = value;
            enemyEvents.DetectorChangedState(this);
        } 
    }

    private DetectorState detectorState;
    private EnemyEvents enemyEvents;
    private VisionCone visionCone;
    private bool playerVisible;
    private float detectionEscalationMeter = 0f;
    private float kDetectPlayerRepetitionDelay = 0.1f;
    private float kDistractionDuration = 5f;
    private float timeOfLastDistraction = 0f;

    public void SetStateDistracted() {
        if (DetectorState == DetectorState.Searching) {
            return;
        }
        
        DetectorState = DetectorState.Distracted;
        visionCone.SetStateDistracted(true);    

        timeOfLastDistraction = Time.time;
        StartCoroutine(ResetDistraction());
    }

    IEnumerator ResetDistraction() {
        while (Time.time - timeOfLastDistraction < kDistractionDuration) {
            yield return null;
        }

        DetectorState = DetectorState.Idle;
        visionCone.SetStateDistracted(false);
    }

    void Start()
    {
        visionCone = GetComponent<VisionCone>();
        enemyEvents = GetComponentInParent<EnemyVM>().EnemyEvents;
        DetectorState = DetectorState.Idle;

        IEnumerator detectPlayerCoroutine = DetectPlayerWithDelay(kDetectPlayerRepetitionDelay);
        StartCoroutine(detectPlayerCoroutine);
    }

    void Update()
    {
        SetDetectionState();
    }

    IEnumerator DetectPlayerWithDelay(float seconds) {
        while (true) {
            yield return new WaitForSeconds(seconds);
            DetectPlayer();
        }
    }

    void DetectPlayer() {    
        if (DetectorState == DetectorState.Distracted) {
            return;
        }

        bool wasPlayerPreviouslyVisible = playerVisible;
        if (PlayerOutsideVisibleCone()) {
            playerVisible = false;
            if (wasPlayerPreviouslyVisible) {
                visionCone.ResetToPatrolling();
            }
            return;
        } 
        
        bool playerObstructed = Physics.Raycast(
            transform.position, 
            (Player.transform.position - transform.position).normalized, 
            Vector3.Distance(Player.transform.position, transform.position), 
            ObstacleMask
        );

        if (playerObstructed) {
            playerVisible = false;
            if (wasPlayerPreviouslyVisible) {
                visionCone.ResetToPatrolling();
            }
            return;
        } 
        
        playerVisible = true;
        visionCone.SetPlayerAsTarget(Player);
    }

    bool PlayerOutsideVisibleCone() {
        float angleToPlayer = Vector3.Angle(visionCone.CurrentLookatTarget - transform.position, 
                                            Player.transform.position - transform.position);
        float distanceToPlayer = Vector3.Distance(Player.transform.position, transform.position);
        return (angleToPlayer > visionCone.FieldOfView / 2 || distanceToPlayer > visionCone.Range);
    }

    void SetDetectionState() {
        if (DetectorState == DetectorState.Idle && playerVisible) {
            DetectorState = DetectorState.Searching;
        }

        if (DetectorState != DetectorState.Distracted) {
            SetDetectionEscalationMeter();
        }
        
        visionCone.SetSpotState(DetectorState, detectionEscalationMeter);
    }

    void SetDetectionEscalationMeter() {
        if (playerVisible) {
            detectionEscalationMeter += Time.deltaTime * kDetectionEscalationSpeed;
        } else {
            detectionEscalationMeter -= Time.deltaTime * kDetectionDeescalationSpeed;
        }

        
        if (detectionEscalationMeter >= 1f) {
            EscelateDetection();
        }
        if (detectionEscalationMeter <= Time.deltaTime * kDetectionDeescalationSpeed) {
            DeescalateDetection();
        }
        
        detectionEscalationMeter = Mathf.Clamp(detectionEscalationMeter, 0f, 1f);
    }

    void EscelateDetection() {
        if (DetectorState == DetectorState.Searching) {
            DetectorState = DetectorState.Alarmed;
        }
        detectionEscalationMeter = 0f;
    }

    void DeescalateDetection() {
        if (DetectorState == DetectorState.Searching && !playerVisible) { 
            DetectorState = DetectorState.Idle;
        }
    }
}
