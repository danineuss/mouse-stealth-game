using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VisionConeState
{
    protected IVisionConeVM visionConeVM;
    protected IConeVisualizer coneVisualizer;

    public abstract void SetupVisionConeState(
        IVisionConeVM visionConeVM, 
        IConeVisualizer coneVisualizer,
        VisionConePatrolPoint patrolPoint, 
        VisionConeDistractPoint distractPoint,
        Transform player
    );

    public abstract void UpdateDetectionMeter(float detectionMeter);
}

public class VisionConeStateIdle : VisionConeState
{
    private VisionConePatrolPoint nextPatrolPoint;
    public override void SetupVisionConeState(
        IVisionConeVM visionConeVM, 
        IConeVisualizer coneVisualizer,
        VisionConePatrolPoint patrolPoint, 
        VisionConeDistractPoint distractPoint,
        Transform player)
    {
        this.visionConeVM = visionConeVM;
        this.coneVisualizer = coneVisualizer;        
        this.nextPatrolPoint = patrolPoint;

        coneVisualizer.SetSpotState(SpotLightState.Idle);
        EvaluatePatrolStart();
    }

    public override void UpdateDetectionMeter(float detectionEscalationMeter) {}

    private void EvaluatePatrolStart()
    {    
        if (nextPatrolPoint.transform.position == visionConeVM.CurrentLookatTarget && 
            nextPatrolPoint.FieldOfView == visionConeVM.FieldOfView)
            return;
        
        visionConeVM.TransitionTo(new VisionConeStatePatrolling());
    }
}

public class VisionConeStatePatrolling : VisionConeState
{
    private VisionConePatrolPoint currentPatrolPoint;

    public override void SetupVisionConeState(
        IVisionConeVM visionConeVM, 
        IConeVisualizer coneVisualizer,
        VisionConePatrolPoint patrolPoint, 
        VisionConeDistractPoint distractPoint,
        Transform player)
    {
        this.visionConeVM = visionConeVM;
        this.coneVisualizer = coneVisualizer;
        this.currentPatrolPoint = patrolPoint;

        coneVisualizer.SetSpotState(SpotLightState.Idle);
        MoveTowardsNextControlPoint();
    }

    public override void UpdateDetectionMeter(float detectionEscalationMeter) {}

    void MoveTowardsNextControlPoint()
    {
        var lerpLookatTarget = PatrolAndWait(
            currentPatrolPoint.transform.position, 
            currentPatrolPoint.FieldOfView, 
            currentPatrolPoint.DurationTowardsPoint
        );
        visionConeVM.StartLookatCoroutine(lerpLookatTarget);
    }

    IEnumerator PatrolAndWait(
        Vector3 newLookatTarget, float newFieldOfView, float durationSeconds)
    {
        var patrol = visionConeVM.LerpTowardsTarget(
            newLookatTarget, newFieldOfView, durationSeconds);
        visionConeVM.StartLookatCoroutine(patrol);

        yield return new WaitForSeconds(durationSeconds);

        visionConeVM.StartLookatCoroutine(WaitAtPatrolPoint());
    }

    private IEnumerator WaitAtPatrolPoint()
    {
        yield return new WaitForSeconds(currentPatrolPoint.WaitTimeAtTarget);

        visionConeVM.IterateControlPointIndex();
        visionConeVM.TransitionTo(new VisionConeStateIdle());
    }
}

public class VisionConeStateFollowingPlayer : VisionConeState
{
    private Transform player;
    private float EvaluationWaitTime = 2f;
    private const float FollowPlayerClampValue = 0.1f;

    public override void SetupVisionConeState(
        IVisionConeVM visionConeVM,
        IConeVisualizer coneVisualizer,
        VisionConePatrolPoint nextPatrolPoint, 
        VisionConeDistractPoint distractPoint,
        Transform player)
    {
        this.visionConeVM = visionConeVM;
        this.coneVisualizer = coneVisualizer;
        this.player = player;

        coneVisualizer.SetSpotState(SpotLightState.Searching);
        visionConeVM.StartLookatCoroutine(FollowPlayer());
    }

    public override void UpdateDetectionMeter(float detectionEscalationMeter) 
    {
        coneVisualizer.SetSpotState(SpotLightState.Searching, detectionEscalationMeter);
    }

    private IEnumerator FollowPlayer()
    {
        while (!visionConeVM.IsPlayerObstructed())
        {
            var deltaVector = player.position - visionConeVM.CurrentLookatTarget;
            if (deltaVector.magnitude > FollowPlayerClampValue)
                deltaVector *= FollowPlayerClampValue / deltaVector.magnitude;
            var newLookatTarget = visionConeVM.CurrentLookatTarget + deltaVector;
            
            visionConeVM.UpdateCone(newLookatTarget, visionConeVM.FieldOfView);
            yield return null;
        }

        visionConeVM.StartLookatCoroutine(EvaluateReturningToPatrolling());
    }

    private IEnumerator EvaluateReturningToPatrolling()
    {
        var startTime = Time.time;
        while(Time.time - startTime < EvaluationWaitTime)
        {
            if (visionConeVM.IsPlayerInsideVisionCone() && !visionConeVM.IsPlayerObstructed())
                visionConeVM.StartLookatCoroutine(FollowPlayer());

            yield return null;
        }

        visionConeVM.TransitionTo(new VisionConeStatePatrolling());
    }
}

public class VisionConeStateDistracted : VisionConeState
{
    private VisionConeDistractPoint distractPoint;
    private readonly float LerpToDistractDuration = 1f;

    public override void SetupVisionConeState(
        IVisionConeVM visionConeVM, 
        IConeVisualizer coneVisualizer,
        VisionConePatrolPoint nextPatrolPoint, 
        VisionConeDistractPoint distractPoint,
        Transform player)
    {
        this.visionConeVM = visionConeVM;
        this.coneVisualizer = coneVisualizer;
        this.distractPoint = distractPoint;

        LookToDistraction();
    }

    public override void UpdateDetectionMeter(float detectionEscalationMeter) {}

    void LookToDistraction()
    {
        coneVisualizer.SetSpotState(SpotLightState.Distracted);

        var distraction = visionConeVM.LerpTowardsTarget(
            distractPoint.Position, distractPoint.FieldOfView, LerpToDistractDuration);
        visionConeVM.StartLookatCoroutine(distraction);
    }
}