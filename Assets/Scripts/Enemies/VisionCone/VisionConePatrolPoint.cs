using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionConePatrolPoint : MonoBehaviour, IVisionConeControlPoint {
    [SerializeField] private float fieldOfView; 
    public float FieldOfView {
        get => fieldOfView; 
        private set => fieldOfView = value;
    }

    public Vector3 Position {
        get => transform.position;
    }

    public void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
