using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVisionConePatrolPoint: IVisionConeControlPoint
{
    float DurationTowardsPoint { get; }
    float WaitTimeAtTarget { get; }
}

public class VisionConePatrolPoint : IVisionConePatrolPoint
{
    public float FieldOfView { get; }
    public Vector3 Position { get; }
    public float DurationTowardsPoint { get; }
    public float WaitTimeAtTarget { get; }

    public VisionConePatrolPoint(
        float fieldOfView, 
        Vector3 position, 
        float durationTowardsPoint, 
        float waitTimeAtTarget)
    {
        FieldOfView = fieldOfView;
        Position = position;
        DurationTowardsPoint = durationTowardsPoint;
        WaitTimeAtTarget = waitTimeAtTarget;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Position, 0.5f);
    }
}
