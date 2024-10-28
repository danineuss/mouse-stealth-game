using Infrastructure;
using UnityEngine;

namespace Enemies.VisionCone
{
    public enum SpotLightState
    {
        Idle,
        Searching,
        Alarmed,
        Distracted
    }

    public interface IConeVisualizer
    {
        void SetSpotState(SpotLightState spotLightState, float detectionMeter = 0);
        void UpdateConeOrientation(Vector3 currentTarget, float fieldOfView);
    }

    public class ConeVisualizer : IConeVisualizer
    {
        private Transform coneTransform;
        private Transform coneScaleParent;
        private Transform coneScaleAnchor;
        private float ConeRangeMultiplier;

        private MeshRenderer coneMeshRenderer;
        private Material greenMaterial;
        private Material blueMaterial;
        private OutlineMono outline;

        private Light spotLight;
        private Color SpotLightGreen;
        private Color SpotLightOrange;
        private Color SpotLightRed;
        private Color SpotLightBlue;

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
            this.ConeRangeMultiplier = coneRangeMultiplier;

            this.coneMeshRenderer = coneMeshRenderer;
            this.greenMaterial = greenMaterial;
            this.blueMaterial = blueMaterial;
            this.outline = outline;

            this.spotLight = spotLight;
            this.SpotLightGreen = spotLightGreen;
            this.SpotLightOrange = spotLightOrange;
            this.SpotLightRed = spotLightRed;
            this.SpotLightBlue = spotLightBlue;
        }

        public void UpdateConeOrientation(Vector3 currentTarget, float fieldOfView)
        {
            var toCurrentTarget = currentTarget - coneTransform.position;
            var range = toCurrentTarget.magnitude * ConeRangeMultiplier;
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
                    spotLight.color = SpotLightGreen;
                    coneMeshRenderer.material = greenMaterial;
                    outline.OutlineColor = SpotLightGreen;
                    break;
                case SpotLightState.Searching:
                    spotLight.color = Color.LerpUnclamped(
                        SpotLightOrange, SpotLightRed, detectionMeter);
                    break;
                case SpotLightState.Alarmed:
                    spotLight.color = SpotLightRed;
                    break;
                case SpotLightState.Distracted:
                    spotLight.color = SpotLightBlue;
                    coneMeshRenderer.material = blueMaterial;
                    outline.OutlineColor = SpotLightBlue;
                    break;
            }
        }
    }
}