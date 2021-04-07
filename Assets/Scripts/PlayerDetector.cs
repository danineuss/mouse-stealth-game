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
    public DetectorState CurrentDetectorState { get; private set; }
    private VisionCone visionCone;

    void Start()
    {
        visionCone = GetComponentInChildren<VisionCone>();
        CurrentDetectorState = DetectorState.Idle;

        IEnumerator detectPlayerCoroutine = DetectPlayerWithDelay(0.2f);
        StartCoroutine(detectPlayerCoroutine);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DetectPlayerWithDelay(float seconds) {
        while (true) {
            yield return new WaitForSeconds(seconds);
            DetectPlayer();
            visionCone.SetSpotState(CurrentDetectorState);
        }
    }

    void DetectPlayer() {
        float angleToPlayer = Vector3.Angle(visionCone.CurrentLookatTarget - transform.position, 
                                            Player.transform.position - transform.position);
        float distanceToPlayer = Vector3.Distance(Player.transform.position, transform.position);
        
        if (angleToPlayer > visionCone.FieldOfView / 2 || distanceToPlayer > visionCone.Range) {
            CurrentDetectorState = DetectorState.Idle;
            return;
        } 
        
        bool playerObstructed = Physics.Raycast(
            transform.position, 
            (Player.transform.position - transform.position).normalized, 
            distanceToPlayer, 
            ObstacleMask
        );
        if (playerObstructed) {
            CurrentDetectorState = DetectorState.Idle;
            return;
        }

        CurrentDetectorState = DetectorState.Alarmed;
    }
}
