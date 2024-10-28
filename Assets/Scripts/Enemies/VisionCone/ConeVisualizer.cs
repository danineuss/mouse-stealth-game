using Infrastructure;
using UnityEngine;

namespace Enemies.VisionCone
{
    public interface IConeVisualizer
    {
        void SetSpotState(SpotLightState spotLightState, float detectionMeter = 0);
        void UpdateConeOrientation(Vector3 currentTarget, float fieldOfView);
    }

    public class ConeVisualizer : IConeVisualizer
    {
        private readonly Transform coneTransform;
        private readonly Transform coneScaleParent;
        private readonly Transform coneScaleAnchor;
        private readonly float coneRangeMultiplier;

        private readonly MeshRenderer coneMeshRenderer;
        private readonly Material greenMaterial;
        private readonly Material blueMaterial;
        private readonly OutlineMono outline;

        private readonly Light spotLight;
        private readonly Color spotLightGreen;
        private readonly Color spotLightOrange;
        private readonly Color spotLightRed;
        private readonly Color spotLightBlue;

        public ConeVisualizer(
            Transform coneTransform,
            Transform coneScaleParent,
            Transform coneScaleAnchor,
            float coneRangeMultiplier,
            MeshRenderer coneMeshRenderer,
            Material greenMaterial,
            Material blueMaterial,
            OutlineMono outline,
            Light spotLight,
            Color spotLightGreen,
            Color spotLightOrange,
            Color spotLightRed,
            Color spotLightBlue)
        {
            this.coneTransform = coneTransform;
            this.coneScaleParent = coneScaleParent;
            this.coneScaleAnchor = coneScaleAnchor;
            this.coneRangeMultiplier = coneRangeMultiplier;

            this.coneMeshRenderer = coneMeshRenderer;
            this.greenMaterial = greenMaterial;
            this.blueMaterial = blueMaterial;
            this.outline = outline;

            this.spotLight = spotLight;
            this.spotLightGreen = spotLightGreen;
            this.spotLightOrange = spotLightOrange;
            this.spotLightRed = spotLightRed;
            this.spotLightBlue = spotLightBlue;
        }

        public void UpdateConeOrientation(Vector3 currentTarget, float fieldOfView)
        {
            var toCurrentTarget = currentTarget - coneTransform.position;
            var range = toCurrentTarget.magnitude * coneRangeMultiplier;
            coneTransform.rotation = Quaternion.LookRotation(toCurrentTarget);

            spotLight.spotAngle = fieldOfView;
            spotLight.range = range;

            var distanceToScaleAnchor = (coneScaleAnchor.position - coneTransform.position).magnitude;
            var newScaleZ = coneScaleParent.localScale.z + 0.7f * (range - distanceToScaleAnchor) / range;
            var newScaleXY = 2 * newScaleZ * Mathf.Tan(fieldOfView / 2 * Mathf.Deg2Rad);
            coneScaleParent.localScale = new Vector3(newScaleXY, newScaleXY, newScaleZ);
        }

        public void SetSpotState(SpotLightState spotLightState, float detectionMeter = 0f)
        {
            switch (spotLightState)
            {
                case SpotLightState.Idle:
                    spotLight.color = spotLightGreen;
                    coneMeshRenderer.material = greenMaterial;
                    outline.OutlineColor = spotLightGreen;
                    break;
                case SpotLightState.Searching:
                    spotLight.color = Color.LerpUnclamped(
                        spotLightOrange, spotLightRed, detectionMeter);
                    break;
                case SpotLightState.Alarmed:
                    spotLight.color = spotLightRed;
                    break;
                case SpotLightState.Distracted:
                    spotLight.color = spotLightBlue;
                    coneMeshRenderer.material = blueMaterial;
                    outline.OutlineColor = spotLightBlue;
                    break;
            }
        }
    }
}