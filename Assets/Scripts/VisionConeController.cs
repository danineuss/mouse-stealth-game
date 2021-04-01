using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class VisionConeController : MonoBehaviour
{
    public float FieldOfView = 50;
    public float TiltAngle = 40;
    public float PanAngle = 15;
    private Transform panParent, tiltParent, spotLight, cone = null;

    void Start()
    {
        InitializeComponents();
        UpdateVisionCone();

        // transform.LookAt(target);
        // float angle = (TiltAngle + 90) * Mathf.Deg2Rad;
        // Vector3 rayDirection = transform.forward * Mathf.Sin(angle) + transform.right * Mathf.Cos(angle);
        // Debug.DrawRay(transform.position, transform.forward, Color.red, 3f);
        // Debug.DrawRay(transform.position, transform.up, Color.green, 3f);
        // Debug.DrawRay(transform.position, transform.right, Color.blue, 3f);
        // Debug.DrawLine(transform.position, transform.position + rayDirection * 7, Color.red, 3f);
    }

    void InitializeComponents() {
        panParent = GetComponentsInChildren<Transform>().Where(x => x.name == "Pan Parent").First();
        tiltParent = GetComponentsInChildren<Transform>().Where(x => x.name == "Tilt Parent").First();
        spotLight = GetComponentsInChildren<Transform>().Where(x => x.name == "Spot Light").First();
        cone = GetComponentsInChildren<Transform>().Where(x => x.name == "Cone").First();

        Assert.IsNotNull(panParent, "Pan parent is null.");
        Assert.IsNotNull(tiltParent, "Tilt parent is null.");
        Assert.IsNotNull(spotLight, "Spot light is null.");
        Assert.IsNotNull(cone, "Cone is null.");
    }

    void UpdateVisionCone() {
        panParent.localEulerAngles = new Vector3(0, 0, PanAngle);
        tiltParent.localEulerAngles = new Vector3(0, TiltAngle, 0);
    }

    void Update()
    {
        
    }
}
