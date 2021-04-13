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
    [SerializeField, Range(0.25f, 3f)] private float kDetectionEscalationSpeed = 1f;
    public DetectorState CurrentDetectorState { get; private set; }

    private VisionCone visionCone;
    private bool playerVisible;
    private float detectionEscalationMeter = 0f;
    private float kDetectionDelay = 0.1f;

    void Start()
    {
        visionCone = GetComponent<VisionCone>();
        CurrentDetectorState = DetectorState.Idle;

        IEnumerator detectPlayerCoroutine = DetectPlayerWithDelay(kDetectionDelay);
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

    public void SetStateDistracted() {
        
    }

    void DetectPlayer() {        
        if (PlayerOutsideVisibleCone()) {
            if (playerVisible) {
                visionCone.ResetToPatrolling();
            }
            playerVisible = false;
            return;
        } 
        
        bool playerObstructed = Physics.Raycast(
            transform.position, 
            (Player.transform.position - transform.position).normalized, 
            Vector3.Distance(Player.transform.position, transform.position), 
            ObstacleMask
        );

        if (playerObstructed) {
            if (playerVisible) {
                visionCone.ResetToPatrolling();
            }
            playerVisible = false;
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
        if (playerVisible) {
            detectionEscalationMeter += Time.deltaTime * kDetectionEscalationSpeed;
        } else {
            detectionEscalationMeter -= Time.deltaTime * kDetectionEscalationSpeed;
        }

        
        if (detectionEscalationMeter >= 1f) {
            EscelateDetection();
        }
        if (detectionEscalationMeter <= Time.deltaTime * kDetectionEscalationSpeed) {
            DeescalateDetection();
        }
        
        detectionEscalationMeter = Mathf.Clamp(detectionEscalationMeter, 0f, 1f);
        visionCone.SetSpotState(CurrentDetectorState, detectionEscalationMeter);
    }

    void EscelateDetection() {
        switch (CurrentDetectorState) {
            case DetectorState.Idle:
                CurrentDetectorState = DetectorState.Searching;
                break;
            case DetectorState.Searching:
                CurrentDetectorState = DetectorState.Alarmed;
                break;
        }
        detectionEscalationMeter = 0f;
    }

    void DeescalateDetection() {
        if (CurrentDetectorState == DetectorState.Searching && !playerVisible) { 
            CurrentDetectorState = DetectorState.Idle;
        }
    }
}
