using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionConePatrolPoint : MonoBehaviour, IVisionConeControlPoint 
{
    [SerializeField] private float fieldOfView = 0f; 
    [SerializeField] private float durationTowardsPoint = 1f;
    [SerializeField] private float waitTimeAtTarget = 0.5f;

    public Vector3 Position => transform.position;
    public float FieldOfView => fieldOfView; 
    public float DurationTowardsPoint => durationTowardsPoint;
    public float WaitTimeAtTarget => waitTimeAtTarget;

    public void OnDrawGizmos() 
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
