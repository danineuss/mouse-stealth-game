using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeVisualizer : MonoBehaviour
{
    public float Range { get; private set; }

    [SerializeField] private Transform coneScaleParent;
    [SerializeField] private Color kSpotLightGreen = new Color(0f, 183f, 18f, 1f);
    [SerializeField] private Color kSpotLightOrange = new Color(183f, 102f, 0f, 1f);
    [SerializeField] private Color kSpotLightRed = new Color(191, 0f, 10f, 1f);
    [SerializeField] private Color kSpotLightBlue = new Color(0f, 23f, 183f, 1f);
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material blueMaterial;
    private MeshRenderer coneMeshRenderer;
    private Light spotLight;
    private Outline coneOutline;
    private float kConeRangeMultiplier = 1.5f;

    public void UpdateConeOrientation(Vector3 currentTarget, float fieldOfView) {
        var deltaVector = currentTarget - transform.position;
        Range = deltaVector.magnitude * kConeRangeMultiplier;
        transform.rotation = Quaternion.LookRotation(deltaVector);

        spotLight.spotAngle = fieldOfView;
        spotLight.range = Range;

        // The '6' is the result of the scale of the Cone 3D Model.
        var newScaleXY = 2 * Range / 6 * Mathf.Tan(fieldOfView / 2 * Mathf.Deg2Rad);
        coneScaleParent.localScale = new Vector3(newScaleXY, newScaleXY, Range / 6);
    }

    public void SetSpotState(DetectorState newDetectorState, float lerpDuration = 0f) {
        switch (newDetectorState) {
            case DetectorState.Idle:
                spotLight.color = Color.LerpUnclamped(kSpotLightGreen, kSpotLightOrange, lerpDuration);
                coneMeshRenderer.material = greenMaterial;
                coneOutline.OutlineColor = kSpotLightGreen;
                break;
            case DetectorState.Searching:
                spotLight.color = Color.LerpUnclamped(kSpotLightOrange, kSpotLightRed, lerpDuration);
                break;
            case DetectorState.Alarmed:
                spotLight.color = kSpotLightRed;
                break;
            case DetectorState.Distracted:
                spotLight.color = kSpotLightBlue;
                coneMeshRenderer.material = blueMaterial;
                coneOutline.OutlineColor = kSpotLightBlue;
                break;
        }
    }

    void Start() {
        spotLight = GetComponentInChildren<Light>();
        coneMeshRenderer = GetComponentInChildren<MeshRenderer>();
        coneOutline = GetComponentInChildren<Outline>();
    }
}