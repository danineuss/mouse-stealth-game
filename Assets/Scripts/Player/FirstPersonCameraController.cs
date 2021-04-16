using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCameraController : MonoBehaviour 
{
    [SerializeField] private float RotationSpeed = 1;
    [SerializeField] private Transform Player;
    
    private float cursorX, cursorY;
    private float initialYRotation;

    void Start() 
    {
        initialYRotation = Player.localRotation.eulerAngles.y;
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
        cursorX += Input.GetAxis("Mouse X") * RotationSpeed;
        cursorY -= Input.GetAxis("Mouse Y") * RotationSpeed;
        cursorY = Mathf.Clamp(cursorY, -35, 60);


        if (!Input.GetKey(KeyCode.LeftShift)) {
            Player.rotation = Quaternion.Euler(0, initialYRotation + cursorX, 0);
        }
        transform.rotation = Quaternion.Euler(cursorY, initialYRotation + cursorX, 0);
    }
}
