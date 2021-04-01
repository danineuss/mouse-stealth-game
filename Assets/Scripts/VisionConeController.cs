using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class VisionConeController : MonoBehaviour
{
    public float FieldOfView = 50;
    public float Range = 10;
    public float TiltAngle = 40;
    public float PanAngle = 0;
    public Transform Player;

    private Transform panParent, tiltParent, scaleParent;
    private Light spotLight;

    void Start()
    {
        InitializeComponents();
        UpdateVisionConeOrientation();

        // transform.LookAt(target);
        // float angle = (TiltAngle + 90) * Mathf.Deg2Rad;
        // Vector3 rayDirection = transform.forward * Mathf.Sin(angle) + transform.right * Mathf.Cos(angle);
        Debug.DrawLine(tiltParent.position, tiltParent.position + tiltParent.right * 7, Color.red, 3f);
        // Debug.DrawRay(transform.position, transform.up, Color.green, 3f);
        // Debug.DrawRay(transform.position, transform.right, Color.blue, 3f);
        // Debug.DrawLine(transform.position, transform.position + rayDirection * 7, Color.red, 3f);
    }

    void Update()
    {
        UpdateVisionConeOrientation();

        float angleToPlayer = Vector3.Angle(tiltParent.right, Player.transform.position - transform.position);
        
    }

    void InitializeComponents() {
        spotLight = GetComponentsInChildren<Light>().Where(x => x.name == "Spot Light").First();
        panParent = GetComponentsInChildren<Transform>().Where(x => x.name == "Pan Parent").First();
        tiltParent = GetComponentsInChildren<Transform>().Where(x => x.name == "Tilt Parent").First();
        scaleParent = GetComponentsInChildren<Transform>().Where(x => x.name == "Scale Parent").First();

        Assert.IsNotNull(Player, "Player is null.");
        Assert.IsNotNull(spotLight, "Spot light is null.");
        Assert.IsNotNull(panParent, "Pan parent is null.");
        Assert.IsNotNull(tiltParent, "Tilt parent is null.");
        Assert.IsNotNull(scaleParent, "Scale Parent is null.");
    }

    void UpdateVisionConeOrientation() {
        panParent.localEulerAngles = new Vector3(0, 0, PanAngle);
        tiltParent.localEulerAngles = new Vector3(0, TiltAngle, 0);

        spotLight.spotAngle = FieldOfView;
        spotLight.range = Range;

        var newScaleYZ = 2 * Range / 6 * Mathf.Tan(FieldOfView / 2 * Mathf.Deg2Rad);
        scaleParent.localScale = new Vector3(Range / 6, newScaleYZ, newScaleYZ);
    }
}
