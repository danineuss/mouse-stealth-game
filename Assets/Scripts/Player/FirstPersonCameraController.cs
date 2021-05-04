using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFirstPersonCameraController {
    void CameraControl();
    void ChangeCursorLockedState(bool locked);
}

public class FirstPersonCameraController : IFirstPersonCameraController
{
    private Transform playerTransform;
    private Transform cameraTransform;
    private readonly IPlayerInput playerInput;
    private readonly float RotationSpeed = 1;
    private float cursorX, cursorY;
    private float initialYRotation;
    private bool cursorLocked;

    public FirstPersonCameraController(Transform player, Transform camera, IPlayerInput playerInput)
    {
        playerTransform = player;
        cameraTransform = camera;
        this.playerInput = playerInput;
        
        initialYRotation = playerTransform.localRotation.eulerAngles.y;
    }
    
    public void CameraControl()
    {
        if (!cursorLocked)
            return;

        cursorX += playerInput.CursorX * RotationSpeed;
        cursorY -= playerInput.CursorY * RotationSpeed;
        cursorY = Mathf.Clamp(cursorY, -35, 60);

        playerTransform.rotation = Quaternion.Euler(0, initialYRotation + cursorX, 0);
        cameraTransform.rotation = Quaternion.Euler(cursorY, initialYRotation + cursorX, 0);
    }

    public void ChangeCursorLockedState(bool locked)
    {
        Cursor.visible = !locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        cursorLocked = locked;
    }
}
