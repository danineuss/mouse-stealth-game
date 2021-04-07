using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    public Transform Player;
    public LayerMask ObstacleMask;
    public Transform ConeScaleParent;
    public VisionConeControlPoints VisionConeControlPoints;
    public Light spotLight;
    public float VisionConePeriod = 5f;

    private Vector3 currentLookatTarget;
    private float fieldOfView;
    private float range;
    private int controlPointIndex = 0;
    private VisionConeControlPoint targetControlPoint;
    private Color kSpotLightGreen = new Color(0f, 183f, 18f, 1f);
    private float kConeRangeMultiplier = 1.5f;

    void Start()
    {
        InitializeCone();        
        UpdateVisionConeOrientation();
        SetNextControlPoint();

        IEnumerator detectPlayerCoroutine = DetectPlayerWithDelay(0.2f);
        StartCoroutine(detectPlayerCoroutine);
    }

    void Update()
    {
        UpdateVisionConeOrientation();
    }

    void InitializeCone() {
        var currentControlPoint = VisionConeControlPoints.controlPoints[controlPointIndex];
        currentLookatTarget = currentControlPoint.transform.position;
        fieldOfView = currentControlPoint.FieldOfView;
    }

    void UpdateVisionConeOrientation() {
        var deltaVector = currentLookatTarget - transform.position;
        range = deltaVector.magnitude * kConeRangeMultiplier;
        transform.rotation = Quaternion.LookRotation(deltaVector);

        spotLight.spotAngle = fieldOfView;
        spotLight.range = range;

        // Some of the following numbers are the result of the scale of the Cone 3D Model.
        var newScaleXY = 2 * range / 6 * Mathf.Tan(fieldOfView / 2 * Mathf.Deg2Rad);
        ConeScaleParent.localScale = new Vector3(newScaleXY, newScaleXY, range / 6);
    }

    void SetNextControlPoint() {
        controlPointIndex = 1 - controlPointIndex;
        var newControlPoint = VisionConeControlPoints.controlPoints[controlPointIndex];
        var newTarget = newControlPoint.transform.position;
        var newFieldOfView = newControlPoint.FieldOfView;
        IEnumerator lerpLookatTarget = LerpLookatTarget(newTarget, newFieldOfView, VisionConePeriod / 2);
        StartCoroutine(lerpLookatTarget);
    }

    IEnumerator LerpLookatTarget(Vector3 newLookatTarget, float newFieldOfView, float durationSeconds) {
        float elapsedTime = 0f;
        float startFieldOfView = fieldOfView;
        Vector3 startLookatTarget = currentLookatTarget;
        while (elapsedTime < durationSeconds) {
            var t = elapsedTime / durationSeconds;
            fieldOfView = Mathf.Lerp(startFieldOfView, newFieldOfView, t);
            currentLookatTarget = Vector3.Lerp(startLookatTarget, newLookatTarget, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetNextControlPoint();
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
