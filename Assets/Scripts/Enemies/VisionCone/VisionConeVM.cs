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

public interface IVisionConeVM
{
    Vector3 CurrentLookatTarget { get; }
    float FieldOfView { get; }
    float Range { get; }

    void ResetToPatrolling();
    void SetPlayerAsTarget(Transform player);
    void SetSpotState(DetectorStateEnum newDetectorState, float lerpDuration = 0);
    void SetStateDistracted(bool distracted);
    void Update();
}

public class VisionConeVM : IVisionConeVM
{
    public Vector3 CurrentLookatTarget { get; private set; }
    public float FieldOfView { get; private set; }
    public float Range => (CurrentLookatTarget - coneTransform.position).magnitude;

    private VisionConeControlPointsMono controlPoints;
    private float visionConePeriod;
    private IConeVisualizer coneVisualizer;
    private Transform coneTransform;
    private EventsMono eventsMono;

    private IEnumerator currentCoroutine;
    private int controlPointIndex = 0;
    private VisionConeState visionConeState;
    private float kFollowPlayerClampValue = 0.1f;

    public VisionConeVM(
        VisionConeControlPointsMono controlPoints,
        float visionConePeriod,
        IConeVisualizer coneVisualizer,
        Transform coneTransform,
        EventsMono eventsMono)
    {
        this.controlPoints = controlPoints;
        this.visionConePeriod = visionConePeriod;
        this.coneVisualizer = coneVisualizer;
        this.coneTransform = coneTransform;
        this.eventsMono = eventsMono;

        InitializeCone();
        MoveTowardsNextControlPoint();
    }

    void InitializeCone()
    {
        var currentControlPoint = controlPoints.patrolPoints[controlPointIndex];
        CurrentLookatTarget = currentControlPoint.transform.position;
        FieldOfView = currentControlPoint.FieldOfView;
        IterateControlPointIndex();

        coneVisualizer.UpdateConeOrientation(CurrentLookatTarget, FieldOfView);

        visionConeState = VisionConeState.Idle;
    }

    public void Update()
    {
        if (visionConeState == VisionConeState.Idle)
        {
            MoveTowardsNextControlPoint();
        }
        coneVisualizer.UpdateConeOrientation(CurrentLookatTarget, FieldOfView);
    }

    public void SetPlayerAsTarget(Transform player)
    {
        visionConeState = VisionConeState.FollowingPlayer;
        StartNewCoroutine(FollowPlayer(player));
    }

    public void SetStateDistracted(bool distracted)
    {
        if (!distracted)
        {
            visionConeState = VisionConeState.Idle;
            return;
        }

        visionConeState = VisionConeState.Distracted;
        StartNewCoroutine(ObserveDistraction());
        eventsMono.StopCoroutine(currentCoroutine);
        currentCoroutine = ObserveDistraction();
        eventsMono.StartCoroutine(currentCoroutine);
    }

    public void ResetToPatrolling()
    {
        if (currentCoroutine != null)
        {
            eventsMono.StopCoroutine(currentCoroutine);
        }
        visionConeState = VisionConeState.Idle;
    }

    public void SetSpotState(DetectorStateEnum newDetectorState, float lerpDuration = 0f)
    {
        coneVisualizer.SetSpotState(newDetectorState, lerpDuration);
    }

    void MoveTowardsNextControlPoint()
    {
        var newControlPoint = controlPoints.patrolPoints[controlPointIndex];
        var newTarget = newControlPoint.transform.position;
        var newFieldOfView = newControlPoint.FieldOfView;
        if (newTarget == CurrentLookatTarget && newFieldOfView == FieldOfView)
            return;

        currentCoroutine = LerpLookatTarget(newTarget, newFieldOfView, visionConePeriod / 2);
        eventsMono.StartCoroutine(currentCoroutine);
    }

    IEnumerator LerpLookatTarget(Vector3 newLookatTarget, float newFieldOfView, float durationSeconds)
    {
        visionConeState = VisionConeState.Patrolling;

        float elapsedTime = 0f;
        float startFieldOfView = FieldOfView;
        Vector3 startLookatTarget = CurrentLookatTarget;
        while (elapsedTime < durationSeconds)
        {
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
    void IterateControlPointIndex()
    {
        if (controlPoints.patrolPoints.Count == 2)
        {
            controlPointIndex = 1 - controlPointIndex;
        }
    }

    void StartNewCoroutine(IEnumerator newCoroutine)
    {
        if (currentCoroutine != null)
        {
            eventsMono.StopCoroutine(currentCoroutine);
        }
        currentCoroutine = newCoroutine;
        eventsMono.StartCoroutine(currentCoroutine);
    }

    IEnumerator FollowPlayer(Transform player)
    {
        while (true)
        {
            var deltaVector = player.position - CurrentLookatTarget;
            if (deltaVector.magnitude > kFollowPlayerClampValue)
            {
                deltaVector *= kFollowPlayerClampValue / deltaVector.magnitude;
            }
            CurrentLookatTarget += deltaVector;
            yield return null;
        }
    }

    IEnumerator ObserveDistraction()
    {
        float elapsedTime = 0f;
        float lerpDuration = 1f;
        float startFieldOfView = FieldOfView;
        Vector3 startLookatTarget = CurrentLookatTarget;

        while (elapsedTime < lerpDuration)
        {
            FieldOfView = Mathf.Lerp(startFieldOfView, controlPoints.distractPoint.FieldOfView,
                                        elapsedTime / lerpDuration);
            CurrentLookatTarget = Vector3.Slerp(startLookatTarget, controlPoints.distractPoint.Position,
                                                elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
