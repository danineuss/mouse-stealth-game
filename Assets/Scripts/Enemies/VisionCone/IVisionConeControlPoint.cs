using UnityEngine;

namespace Enemies.VisionCone
{
    public interface IVisionConeControlPoint {
        float FieldOfView { get; }
        Vector3 Position { get; }
        void OnDrawGizmos();
    }
}
