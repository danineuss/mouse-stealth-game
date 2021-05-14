using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConeVisualizer : MonoBehaviour {
    [SerializeField] private Color kSpotLightGreen = new Color(0f, 183f, 18f, 1f);
    [SerializeField] private Color kSpotLightOrange = new Color(183f, 102f, 0f, 1f);
    [SerializeField] private Color kSpotLightRed = new Color(191, 0f, 10f, 1f);
    [SerializeField] private Color kSpotLightBlue = new Color(0f, 23f, 183f, 1f);
    [SerializeField] private Material greenMaterial = null;
    [SerializeField] private Material blueMaterial = null;
    [SerializeField] private float kConeRangeMultiplier = 1.5f;

    private MeshRenderer coneMeshRenderer;
    private Light spotLight;
    private Outline coneOutline;
    private Transform coneScaleParent;
    private Transform coneScaleAnchor;

    public void UpdateConeOrientation(Vector3 currentTarget, float fieldOfView) {
        var toCurrentTarget = currentTarget - transform.position;
        var range = toCurrentTarget.magnitude * kConeRangeMultiplier;
        transform.rotation = Quaternion.LookRotation(toCurrentTarget);

        spotLight.spotAngle = fieldOfView;
        spotLight.range = range;

        var distanceToScaleAnchor = (coneScaleAnchor.position - transform.position).magnitude;
        var newScaleZ = coneScaleParent.localScale.z + 0.7f * (range - distanceToScaleAnchor) / range;
        var newScaleXY = 2 * newScaleZ * Mathf.Tan(fieldOfView / 2 * Mathf.Deg2Rad);
        coneScaleParent.localScale = new Vector3(newScaleXY, newScaleXY, newScaleZ);
    }

    public void SetSpotState(DetectorStateEnum newDetectorState, float lerpDuration = 0f)
    {
        switch (newDetectorState)
        {
            case DetectorStateEnum.Idle:
                spotLight.color = Color.LerpUnclamped(kSpotLightGreen, kSpotLightOrange, lerpDuration);
                coneMeshRenderer.material = greenMaterial;
                coneOutline.OutlineColor = kSpotLightGreen;
                break;
            case DetectorStateEnum.Searching:
                spotLight.color = Color.LerpUnclamped(kSpotLightOrange, kSpotLightRed, lerpDuration);
                break;
            case DetectorStateEnum.Alarmed:
                spotLight.color = kSpotLightRed;
                break;
            case DetectorStateEnum.Distracted:
                spotLight.color = kSpotLightBlue;
                coneMeshRenderer.material = blueMaterial;
                coneOutline.OutlineColor = kSpotLightBlue;
                break;
        }
    }

    void Awake() {
        spotLight = GetComponentInChildren<Light>();
        coneMeshRenderer = GetComponentInChildren<MeshRenderer>();
        coneOutline = GetComponentInChildren<Outline>();
        coneScaleParent = GetComponentsInChildren<Transform>()
                            .Where(x => x.CompareTag("ScaleParent"))
                            .First();
        coneScaleAnchor = GetComponentsInChildren<Transform>()
                            .Where(x => x.CompareTag("ScaleAnchor"))
                            .First();
    }
}