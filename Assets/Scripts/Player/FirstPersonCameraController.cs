﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCameraController : MonoBehaviour 
{
    // Camera setup
    public float RotationSpeed = 1;
    public Transform Player;
    float mouseX, mouseY;

    void Start() 
    {
        ToggleCursorLocked(true);
    }

    void LateUpdate() 
    {
        CameraControl();
    }

    public void ToggleCursorLocked(bool locked) {
        Cursor.visible = !locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
    }

    void CameraControl() 
    {
        mouseX += Input.GetAxis("Mouse X") * RotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * RotationSpeed;
        mouseY = Mathf.Clamp(mouseY, -35, 60);


        if (!Input.GetKey(KeyCode.LeftShift)) {
            Player.rotation = Quaternion.Euler(0, mouseX, 0);
        }
        transform.rotation = Quaternion.Euler(mouseY, mouseX, 0);
    }
}