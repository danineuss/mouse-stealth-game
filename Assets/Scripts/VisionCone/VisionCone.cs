using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    [SerializeField] private Transform ConeScaleParent;
    [SerializeField] private VisionConeControlPoints VisionConeControlPoints;
    [SerializeField] private Light spotLight;
    [SerializeField] private float VisionConePeriod = 5f;
    [SerializeField] private Color kSpotLightGreen = new Color(0f, 183f, 18f, 1f);
    [SerializeField] private Color kSpotLightOrange = new Color(183f, 102f, 0f, 1f);
    [SerializeField] private Color kSpotLightRed = new Color(191, 0f, 10f, 1f);
    [SerializeField] private Color kSpotLightBlue = new Color(0f, 23f, 183f, 1f);

    public Vector3 CurrentLookatTarget { get; private set; }
    public float FieldOfView { get; private set; }
    public float Range { get; private set; }
    
    private int controlPointIndex = 0;
    private VisionConeControlPoint targetControlPoint;
    private float kConeRangeMultiplier = 1.5f;

    void Start()
    {
        InitializeCone();        
        UpdateVisionConeOrientation();
        SetNextControlPoint();
    }

    void Update()
    {
        UpdateVisionConeOrientation();
    }

    void InitializeCone() {
        var currentControlPoint = VisionConeControlPoints.controlPoints[controlPointIndex];
        CurrentLookatTarget = currentControlPoint.transform.position;
        FieldOfView = currentControlPoint.FieldOfView;
    }

    void UpdateVisionConeOrientation() {
        var deltaVector = CurrentLookatTarget - transform.position;
        Range = deltaVector.magnitude * kConeRangeMultiplier;
        transform.rotation = Quaternion.LookRotation(deltaVector);

        spotLight.spotAngle = FieldOfView;
        spotLight.range = Range;

        // Some of the following numbers are the result of the scale of the Cone 3D Model.
        var newScaleXY = 2 * Range / 6 * Mathf.Tan(FieldOfView / 2 * Mathf.Deg2Rad);
        ConeScaleParent.localScale = new Vector3(newScaleXY, newScaleXY, Range / 6);
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
        float startFieldOfView = FieldOfView;
        Vector3 startLookatTarget = CurrentLookatTarget;
        while (elapsedTime < durationSeconds) {
            var t = elapsedTime / durationSeconds;
            FieldOfView = Mathf.Lerp(startFieldOfView, newFieldOfView, t);
            CurrentLookatTarget = Vector3.Lerp(startLookatTarget, newLookatTarget, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetNextControlPoint();
    }

    public void SetSpotState(DetectorState newDetectorState, float lerp = 0f) {
        switch (newDetectorState) {
            case DetectorState.Idle:
                spotLight.color = Color.LerpUnclamped(kSpotLightGreen, kSpotLightOrange, lerp);
                break;
            case DetectorState.Searching:
                spotLight.color = Color.LerpUnclamped(kSpotLightOrange, kSpotLightRed, lerp);
                break;
            case DetectorState.Alarmed:
                spotLight.color = kSpotLightRed;
                break;
            case DetectorState.Distracted:
                spotLight.color = kSpotLightBlue;
                break;
        }

    }
}
