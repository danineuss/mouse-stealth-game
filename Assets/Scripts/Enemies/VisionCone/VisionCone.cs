using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum VisionConeState {
    Idle,
    Patrolling,
    FollowingPlayer,
    Distracted
}

public class VisionCone : MonoBehaviour
{
    [SerializeField] private VisionConeControlPoints ControlPoints;
    [SerializeField] private float VisionConePeriod = 5f;

    public Vector3 CurrentLookatTarget { get; private set; }
    public float FieldOfView { get; private set; }
    public float Range { 
        get => (CurrentLookatTarget - transform.position).magnitude;
    }
 
    private ConeVisualizer coneVisualizer;
    private IEnumerator currentCoroutine;
    private int controlPointIndex = 0;
    private VisionConeState visionConeState;
    private float kFollowPlayerClampValue = 0.1f;

    public void SetPlayerAsTarget(Transform player) {
        visionConeState = VisionConeState.FollowingPlayer;
        StartNewCoroutine(FollowPlayer(player));
    }

    public void SetStateDistracted(bool distracted) {
        if (!distracted) {
            visionConeState = VisionConeState.Idle;
            return;
        }
        
        visionConeState = VisionConeState.Distracted;
        StartNewCoroutine(ObserveDistraction());
        StopCoroutine(currentCoroutine);
        currentCoroutine = ObserveDistraction();
        StartCoroutine(currentCoroutine);
    }

    public void ResetToPatrolling() {
        if (currentCoroutine != null) {
            StopCoroutine(currentCoroutine);
        }
        visionConeState = VisionConeState.Idle;
    }

    public void SetSpotState(DetectorState newDetectorState, float lerpDuration = 0f) {
        coneVisualizer.SetSpotState(newDetectorState, lerpDuration);
    }

    void Start()
    {
        InitializeCone();        
        MoveTowardsNextControlPoint();
    }

    void InitializeCone() {
        var currentControlPoint = ControlPoints.patrolPoints[controlPointIndex];
        CurrentLookatTarget = currentControlPoint.transform.position;
        FieldOfView = currentControlPoint.FieldOfView;
        IterateControlPointIndex();

        coneVisualizer = GetComponent<ConeVisualizer>();
        coneVisualizer.UpdateConeOrientation(CurrentLookatTarget, FieldOfView);
        
        visionConeState = VisionConeState.Idle;
    }

    void Update() {
        if (visionConeState == VisionConeState.Idle) {
            MoveTowardsNextControlPoint();
        }
        coneVisualizer.UpdateConeOrientation(CurrentLookatTarget, FieldOfView);
    }

    void MoveTowardsNextControlPoint() {
        var newControlPoint = ControlPoints.patrolPoints[controlPointIndex];
        var newTarget = newControlPoint.transform.position;
        var newFieldOfView = newControlPoint.FieldOfView;
        if (newTarget == CurrentLookatTarget && newFieldOfView == FieldOfView) {
            return;
        }

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

        IterateControlPointIndex();
        visionConeState = VisionConeState.Idle;
    }
    
    // Currently supports one or two Control Points: iterate iff Count == 2, otherwise stay.
    void IterateControlPointIndex() {
        if (ControlPoints.patrolPoints.Count == 2) {
            controlPointIndex = 1 - controlPointIndex;
        }
    }

    void StartNewCoroutine(IEnumerator newCoroutine) {
        if (currentCoroutine != null) {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = newCoroutine;
        StartCoroutine(currentCoroutine);
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

    IEnumerator ObserveDistraction() {
        float elapsedTime = 0f;
        float lerpDuration = 1f;
        float startFieldOfView = FieldOfView;
        Vector3 startLookatTarget = CurrentLookatTarget;

        while (elapsedTime < lerpDuration) {
            FieldOfView = Mathf.Lerp(startFieldOfView, ControlPoints.distractPoint.FieldOfView, 
                                        elapsedTime / lerpDuration);
            CurrentLookatTarget = Vector3.Slerp(startLookatTarget, ControlPoints.distractPoint.Position, 
                                                elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
