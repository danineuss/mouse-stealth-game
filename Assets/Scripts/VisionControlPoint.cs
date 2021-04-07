using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionControlPoint : MonoBehaviour
{
    [SerializeField] private float fieldOfView; 
    public float FieldOfView {
        get { return fieldOfView; }
        private set { fieldOfView = value; }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
