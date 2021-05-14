using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IConeVisualizer
{
    void SetSpotState(DetectorStateEnum newDetectorState, float lerpDuration = 0);
    void UpdateConeOrientation(Vector3 currentTarget, float fieldOfView);
}

public class ConeVisualizer : IConeVisualizer
{
    private Transform coneTransform;
    private Transform coneScaleParent;
    private Transform coneScaleAnchor;
    private float kConeRangeMultiplier;

    private MeshRenderer coneMeshRenderer;
    private Material greenMaterial;
    private Material blueMaterial;
    private Outline outline;

    private Light spotLight;
    private Color kSpotLightGreen;
    private Color kSpotLightOrange;
    private Color kSpotLightRed;
    private Color kSpotLightBlue;

    public ConeVisualizer(
        Transform coneTransform,
        Transform coneScaleParent,
        Transform coneScaleAnchor,
        float kConeRangeMultiplier,
        MeshRenderer coneMeshRenderer,
        Material greenMaterial,
        Material blueMaterial,
        Outline outline,
        Light spotLight,
        Color kSpotLightGreen,
        Color kSpotLightOrange,
        Color kSpotLightRed,
        Color kSpotLightBlue)
    {
        this.coneTransform = coneTransform;
        this.coneScaleParent = coneScaleParent;
        this.coneScaleAnchor = coneScaleAnchor;
        this.kConeRangeMultiplier = kConeRangeMultiplier;

        this.coneMeshRenderer = coneMeshRenderer;
        this.greenMaterial = greenMaterial;
        this.blueMaterial = blueMaterial;
        this.outline = outline;

        this.spotLight = spotLight;
        this.kSpotLightGreen = kSpotLightGreen;
        this.kSpotLightOrange = kSpotLightOrange;
        this.kSpotLightRed = kSpotLightRed;
        this.kSpotLightBlue = kSpotLightBlue;
    }

    public void UpdateConeOrientation(Vector3 currentTarget, float fieldOfView)
    {
        var toCurrentTarget = currentTarget - coneTransform.position;
        var range = toCurrentTarget.magnitude * kConeRangeMultiplier;
        coneTransform.rotation = Quaternion.LookRotation(toCurrentTarget);

        spotLight.spotAngle = fieldOfView;
        spotLight.range = range;

        var distanceToScaleAnchor = (coneScaleAnchor.position - coneTransform.position).magnitude;
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
                outline.OutlineColor = kSpotLightGreen;
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
                outline.OutlineColor = kSpotLightBlue;
                break;
        }
    }
}