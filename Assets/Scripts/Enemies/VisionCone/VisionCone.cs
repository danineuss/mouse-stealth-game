using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum VisionConeState {
    Idle,
    Patrolling,
    FollowingPlayer
}

public class VisionCone : MonoBehaviour
{
    [SerializeField] private VisionConeControlPoints VisionConeControlPoints;
    [SerializeField] private float VisionConePeriod = 5f;

    public Vector3 CurrentLookatTarget { get; private set; }
    public float FieldOfView { get; private set; }
    public float Range { 
        get {
            return (CurrentLookatTarget - transform.position).magnitude;
        }
    }
 
    private ConeVisualizer coneVisualizer;
    private IEnumerator currentCoroutine;
    private int controlPointIndex = 0;
    private VisionConeState visionConeState;
    private float kFollowPlayerClampValue = 0.1f;

    void Start()
    {
        InitializeCone();        
        MoveTowardsNextControlPoint();
    }

    void InitializeCone() {
        var currentControlPoint = VisionConeControlPoints.patrolPoints[controlPointIndex];
        CurrentLookatTarget = currentControlPoint.transform.position;
        FieldOfView = currentControlPoint.FieldOfView;

        coneVisualizer = GetComponent<ConeVisualizer>();
        coneVisualizer.UpdateConeVisualization(CurrentLookatTarget, FieldOfView);
        
        visionConeState = VisionConeState.Idle;
    }

    void Update()
    {
        if (visionConeState == VisionConeState.Idle) {
            MoveTowardsNextControlPoint();
        }
        coneVisualizer.UpdateConeVisualization(CurrentLookatTarget, FieldOfView);
    }

    void MoveTowardsNextControlPoint() {
        var newControlPoint = VisionConeControlPoints.patrolPoints[controlPointIndex];
        var newTarget = newControlPoint.transform.position;
        var newFieldOfView = newControlPoint.FieldOfView;

        currentCoroutine = LerpLookatTarget(newTarget, newFieldOfView, VisionConePeriod / 2);
        StartCoroutine(currentCoroutine);
    }

    IEnumerator LerpLookatTarget(Vector3 newLookatTarget, float newFieldOfView, float durationSeconds) {
        visionConeState = VisionConeState.Patrolling;

        float elapsedTime = 0f;
        float startFieldOfView = FieldOfView;
        Vector3 startLookatTarget = CurrentLookatTarget;
        while (elapsedTime < durationSeconds) {
            FieldOfView = Mathf.Lerp(startFieldOfView, newFieldOfView, elapsedTime / durationSeconds);
            CurrentLookatTarget = Vector3.Slerp(startLookatTarget, newLookatTarget, 
                                                elapsedTime / durationSeconds);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        visionConeState = VisionConeState.Idle;
        IterateControlPointIndex();
    }
    
    void IterateControlPointIndex() {
        // Currently supports one or two Control Points.
        if (VisionConeControlPoints.patrolPoints.Count == 2) {
            controlPointIndex = 1 - controlPointIndex;
        }
    }

    IEnumerator FollowPlayer(Transform player) {
        while (true) {
            var deltaVector = player.position - CurrentLookatTarget;
            if (deltaVector.magnitude > kFollowPlayerClampValue) {
                deltaVector *= kFollowPlayerClampValue / deltaVector.magnitude;
            }
            CurrentLookatTarget += deltaVector;
            yield return null;
        }
    }

    public void SetPlayerAsTarget(Transform player) {
        StopCoroutine(currentCoroutine);
        currentCoroutine = FollowPlayer(player);
        visionConeState = VisionConeState.FollowingPlayer;
        StartCoroutine(currentCoroutine);
    }

    public void ResetToPatrolling() {
        StopCoroutine(currentCoroutine);
        visionConeState = VisionConeState.Idle;
    }

    public void SetSpotState(DetectorState newDetectorState, float lerpDuration = 0f) {
        coneVisualizer.SetSpotState(newDetectorState, lerpDuration);
    }
}
