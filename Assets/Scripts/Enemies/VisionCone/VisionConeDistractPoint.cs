﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionConeDistractPoint : MonoBehaviour, IVisionConeControlPoint {
    [SerializeField] private float fieldOfView; 
    public float FieldOfView {
        get { return fieldOfView; }
        private set { fieldOfView = value; }
    }

    public Vector3 Position {
        get { 
            return transform.position; 
        }
    }
    
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}