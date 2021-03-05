using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour 
{
    public float RotationSpeed = 1;
    public Transform Target, Player ;
    float mouseX, mouseY;

    public Transform Obstruction;
    public float DefaultCameraDistance = 1.0f;
    float zoomSpeed = 2f;
    float minimumCameraDistance = 0.5f;
   

    void Start() 
    {
        Obstruction = null;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        transform.position = Target.position + Vector3.back * DefaultCameraDistance;

    }

    void LateUpdate() 
    {
        CameraControl();
        ViewObstructed();
    }

    void CameraControl() 
    {
        mouseX += Input.GetAxis("Mouse X") * RotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * RotationSpeed;
        mouseY = Mathf.Clamp(mouseY, -35, 60);

        transform.LookAt(Target);

        if (!Input.GetKey(KeyCode.LeftShift)) {
            Player.rotation = Quaternion.Euler(0, mouseX, 0);
        }
        Target.rotation = Quaternion.Euler(mouseY, mouseX, 0);
    }

    void ViewObstructed() 
    {
        if (!Physics.Raycast(Target.position, transform.position - Target.position, out RaycastHit hit, 4.5f) ||
            hit.collider.gameObject.CompareTag("Player"))
        {
            ZoomCamera(false);
            Obstruction = null;
            return;
        }

        Obstruction = hit.transform;

        if (Vector3.Distance(Obstruction.position, Target.position) >= 3f)
        {
            Obstruction.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            ZoomCamera(true);
        }
        else
        {
            Obstruction.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
    }

    void ZoomCamera(bool zoomIn)
    {
        if (zoomIn)
        {
            if (Vector3.Distance(transform.position, Target.position) <= minimumCameraDistance) { return; }
            transform.Translate(Vector3.forward * zoomSpeed * Time.deltaTime);
        } 
        else
        {
            if (Vector3.Distance(transform.position, Target.position) > DefaultCameraDistance) { return; }
            transform.Translate(Vector3.back * zoomSpeed * Time.deltaTime);
        }
    }
}
