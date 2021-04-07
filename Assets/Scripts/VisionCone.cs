using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class VisionCone : MonoBehaviour
{
    public Transform Player;
    public LayerMask ObstacleMask;
    public Transform ConeScaleParent;
    public VisionConeControlPoints VisionConeControlPoints;
    public Light spotLight;

    private Vector3 currentTarget;
    private float fieldOfView = 50;
    private float range = 10;
    private Color kSpotLightGreen = new Color(0f, 183f, 18f, 1f);
    private float kConeRangeMultiplier = 1.5f;

    void Start()
    {
        InitializeCone();        
        UpdateVisionConeOrientation();

        IEnumerator detectPlayerCoroutine = DetectPlayerWithDelay(0.2f);
        StartCoroutine(detectPlayerCoroutine);
    }

    void Update()
    {
        UpdateVisionConeOrientation();
    }

    void InitializeCone() {
        var currentControlPoint = VisionConeControlPoints.controlPoints.FirstOrDefault();
        currentTarget = currentControlPoint.transform.position;
        fieldOfView = currentControlPoint.FieldOfView;
    }

    void UpdateVisionConeOrientation() {
        var deltaVector = currentTarget - transform.position;
        range = deltaVector.magnitude * kConeRangeMultiplier;
        transform.rotation = Quaternion.LookRotation(deltaVector);

        spotLight.spotAngle = fieldOfView;
        spotLight.range = range;

        var newScaleXY = 2 * range / 6 * Mathf.Tan(fieldOfView / 2 * Mathf.Deg2Rad);
        ConeScaleParent.localScale = new Vector3(newScaleXY, newScaleXY, range / 6);
    }

    IEnumerator DetectPlayerWithDelay(float seconds) {
        while (true) {
            yield return new WaitForSeconds(seconds);
            DetectPlayer();
        }
    }

    void DetectPlayer() {
        float angleToPlayer = Vector3.Angle(transform.forward, Player.transform.position - transform.position);
        float distanceToPlayer = Vector3.Distance(Player.transform.position, transform.position);
        
        if (angleToPlayer > fieldOfView / 2 || distanceToPlayer > range) {
            spotLight.color = kSpotLightGreen;
            return;
        } 
        
        bool playerObstructed = Physics.Raycast(
            transform.position, 
            (Player.transform.position - transform.position).normalized, 
            distanceToPlayer, 
            ObstacleMask
        );
        if (playerObstructed) {
            spotLight.color = kSpotLightGreen;
            return;
        }

        spotLight.color = Color.red;
    }
}
