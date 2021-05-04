using UnityEngine;

public interface IFirstPersonCameraController {
    void RotateForPlayerInput();
    void ChangeCursorLockedState(bool locked);
}

public class FirstPersonCameraController : IFirstPersonCameraController
{
    private Transform playerTransform;
    private Transform cameraTransform;
    private readonly IPlayerInput playerInput;
    private readonly float rotationSpeed;

    private float cursorX, cursorY;
    private float initialYRotation;
    private bool cursorLocked;

    public FirstPersonCameraController(
        Transform player, Transform camera, IPlayerInput playerInput, float rotationSpeed)
    {
        playerTransform = player;
        cameraTransform = camera;
        this.playerInput = playerInput;
        this.rotationSpeed = rotationSpeed;

        InitializeValues();
    }

    void InitializeValues() {
        cursorX = 0f; 
        cursorY = 0f;
        initialYRotation = playerTransform.localRotation.eulerAngles.y;
        cursorLocked = true;
    }
    
    public void RotateForPlayerInput()
    {
        if (!cursorLocked)
            return;

        cursorX += playerInput.CursorX * rotationSpeed;
        cursorY -= playerInput.CursorY * rotationSpeed;
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
