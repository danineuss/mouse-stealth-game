using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVisionConeVM: IUpdatable
{
    Vector3 CurrentLookatTarget { get; }
    float FieldOfView { get; }
    float Range { get; }

    bool IsPlayerInsideVisionCone();
    bool IsPlayerObstructed();
    void IterateControlPointIndex();
    IEnumerator LerpTowardsTarget(
        Vector3 newLookatTarget, float newFieldOfView, float durationSeconds);
    void StartLookatCoroutine(IEnumerator lerpCoroutine, bool interrupt = true);
    void TransitionTo(VisionConeState visionConeState);
    void UpdateCone(Vector3 target, float fieldOfView);
    void UpdateDetectionMeter(float detectionMeter);
}

public class VisionConeVM : IVisionConeVM
{
    public Vector3 CurrentLookatTarget { get; private set; }
    public float FieldOfView { get; private set; }
    public float Range => (CurrentLookatTarget - coneTransform.position).magnitude;

    private List<IVisionConePatrolPoint> patrolPoints;
    private IVisionConeControlPoint distractPoint;
    private IConeVisualizer coneVisualizer;
    private Transform coneTransform;
    private Transform playerTransform;
    private LayerMask obstacleMask;
    private IEvents events;

    private VisionConeState visionConeState;
    private List<IEnumerator> currentCoroutines = new List<IEnumerator>();
    private int controlPointIndex = 0;

    public VisionConeVM(
        List<IVisionConePatrolPoint> patrolPoints,
        IVisionConeControlPoint distractPoint,
        IConeVisualizer coneVisualizer,
        Transform coneTransform,
        Transform playerTransform,
        LayerMask obstacleMask,
        IEvents events)
    {
        this.patrolPoints = patrolPoints;
        this.distractPoint = distractPoint;
        this.coneVisualizer = coneVisualizer;
        this.coneTransform = coneTransform;
        this.playerTransform = playerTransform;
        this.obstacleMask = obstacleMask;
        this.events = events;

        InitializeCone();

        IterateControlPointIndex();
        TransitionTo(new VisionConeStateIdle());
    }

    void InitializeCone()
    {
        var currentControlPoint = patrolPoints[controlPointIndex];
        CurrentLookatTarget = currentControlPoint.Position;
        FieldOfView = currentControlPoint.FieldOfView;

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
    
    public void IterateControlPointIndex()
    {
        if (patrolPoints.Count == 2)
            controlPointIndex = 1 - controlPointIndex;
    }

    public IEnumerator LerpTowardsTarget(
        Vector3 newLookatTarget, float newFieldOfView, float durationSeconds)
    {
        Vector3 startLookatTarget = CurrentLookatTarget;
        float startFieldOfView = FieldOfView;
        float elapsedTime = 0f;

        while (elapsedTime < durationSeconds)
        {
            CurrentLookatTarget = Vector3.Slerp(
                startLookatTarget, newLookatTarget, elapsedTime / durationSeconds);
            FieldOfView = Mathf.Lerp(
                startFieldOfView, newFieldOfView, elapsedTime / durationSeconds);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    public void StartLookatCoroutine(IEnumerator newCoroutine, bool interrupt = true)
    {
        if (currentCoroutines.Count != 0 && interrupt)
            currentCoroutines.ForEach(x => events.StopCoroutine(x));
        
        currentCoroutines.Add(newCoroutine);
        events.StartCoroutine(newCoroutine);
    }

    public void TransitionTo(VisionConeState visionConeState)
    {
        visionConeState.SetupVisionConeState(
            this, 
            coneVisualizer,
            patrolPoints[controlPointIndex],
            distractPoint,
            playerTransform
        );
        this.visionConeState = visionConeState;
    }

    public void Update()
    {
        coneVisualizer.UpdateConeOrientation(CurrentLookatTarget, FieldOfView);
    }

    public void UpdateCone(Vector3 target, float fieldOfView)
    {
        CurrentLookatTarget = target;
        FieldOfView = fieldOfView;
    }

    public void UpdateDetectionMeter(float detectionMeter)
    {
        visionConeState.UpdateDetectionMeter(detectionMeter);
    }
}
