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
    public LayerMask ObstacleMask;

    private Transform panParent, tiltParent, scaleParent;
    private Light spotLight;
    private Color spotLightGreen = new Color(0f, 183f, 18f, 1f);

    void Start()
    {
        InitializeComponents();
        UpdateVisionConeOrientation();
        StartCoroutine("DetectPlayerWithDelay", 0.15f);
    }

    void Update()
    {
        UpdateVisionConeOrientation();
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

    IEnumerator DetectPlayerWithDelay(float seconds) {
        while (true) {
            yield return new WaitForSeconds(seconds);
            DetectPlayer();
        }
    }

    void DetectPlayer() {
        float angleToPlayer = Vector3.Angle(tiltParent.right, Player.transform.position - transform.position);
        float distanceToPlayer = Vector3.Distance(Player.transform.position, transform.position);
        
        if (angleToPlayer > FieldOfView / 2 || distanceToPlayer > Range) {
            spotLight.color = spotLightGreen;
            return;
        } 
        
        bool playerObstructed = Physics.Raycast(
            transform.position, (Player.transform.position - transform.position).normalized, 
            distanceToPlayer, ObstacleMask
        );
        if (playerObstructed) {
            spotLight.color = spotLightGreen;
            return;
        }

        spotLight.color = Color.red;
    }
}
