using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum VisionConeStateEnum {
    Idle,
    Patrolling,
    FollowingPlayer,
    Distracted
}

public interface IVisionConeVM: IUpdatable
{
    Vector3 CurrentLookatTarget { get; }
    float FieldOfView { get; }
    float Range { get; }

    bool IsPlayerInsideVisionCone();
    bool IsPlayerObstructed();
    void ResetToPatrolling();
    void StartFollowingPlayer();
    void SetSpotState(SpotLightState spotLightState, float lerpDuration = 0);
    void SetStateDistracted(bool distracted);
    void TransitionTo(VisionConeState visionConeState);
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
    private Transform playerTransform;
    private LayerMask obstacleMask;
    private EventsMono eventsMono;

    private VisionConeState visionConeState;
    private IEnumerator currentCoroutine;
    private int controlPointIndex = 0;
    private VisionConeStateEnum visionConeStateEnum;
    private float kFollowPlayerClampValue = 0.1f;

    public VisionConeVM(
        VisionConeControlPointsMono controlPoints,
        float visionConePeriod,
        IConeVisualizer coneVisualizer,
        Transform coneTransform,
        Transform playerTransform,
        LayerMask obstacleMask,
        EventsMono eventsMono)
    {
        this.controlPoints = controlPoints;
        this.visionConePeriod = visionConePeriod;
        this.coneVisualizer = coneVisualizer;
        this.coneTransform = coneTransform;
        this.playerTransform = playerTransform;
        this.obstacleMask = obstacleMask;
        this.eventsMono = eventsMono;

        visionConeStateEnum = VisionConeStateEnum.Idle;
        TransitionTo(new VisionConeStateIdle(this));

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
    }

    public bool IsPlayerInsideVisionCone()
    {
        float angleToPlayer = Vector3.Angle(
            CurrentLookatTarget - coneTransform.position,
            playerTransform.position - coneTransform.position
        );
        float distanceToPlayer = Vector3.Distance(playerTransform.position, coneTransform.position);
        return (angleToPlayer < FieldOfView / 2 && distanceToPlayer < Range);
    }

    public bool IsPlayerObstructed()
    {
        return Physics.Raycast(
            coneTransform.position,
            (playerTransform.position - coneTransform.position).normalized,
            Vector3.Distance(playerTransform.position, coneTransform.position),
            obstacleMask,
            QueryTriggerInteraction.Ignore
        );
    }

    public void TransitionTo(VisionConeState visionConeState)
    {
        this.visionConeState = visionConeState;
    }

    public void Update()
    {
        if (visionConeStateEnum == VisionConeStateEnum.Idle)
            MoveTowardsNextControlPoint();
        
        coneVisualizer.UpdateConeOrientation(CurrentLookatTarget, FieldOfView);
    }

    public void StartFollowingPlayer()
    {
        visionConeStateEnum = VisionConeStateEnum.FollowingPlayer;
        StartNewCoroutine(FollowPlayer(playerTransform));
    }

    public void SetStateDistracted(bool distracted)
    {
        if (!distracted)
        {
            visionConeStateEnum = VisionConeStateEnum.Idle;
            return;
        }

        visionConeStateEnum = VisionConeStateEnum.Distracted;
        StartNewCoroutine(ObserveDistraction());
    }

    public void ResetToPatrolling()
    {
        if (currentCoroutine != null)
            eventsMono.StopCoroutine(currentCoroutine);
        
        visionConeStateEnum = VisionConeStateEnum.Idle;
    }

    public void SetSpotState(SpotLightState spotLightState, float lerpDuration = 0f)
    {
        coneVisualizer.SetSpotState(spotLightState, lerpDuration);
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
        visionConeStateEnum = VisionConeStateEnum.Patrolling;

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
        visionConeStateEnum = VisionConeStateEnum.Idle;
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
            eventsMono.StopCoroutine(currentCoroutine);
        
        currentCoroutine = newCoroutine;
        eventsMono.StartCoroutine(currentCoroutine);
    }

    IEnumerator FollowPlayer(Transform player)
    {
        while (true)
        {
            var deltaVector = player.position - CurrentLookatTarget;
            if (deltaVector.magnitude > kFollowPlayerClampValue)
                deltaVector *= kFollowPlayerClampValue / deltaVector.magnitude;
            
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
