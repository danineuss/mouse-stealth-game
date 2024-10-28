using UnityEngine;

namespace Enemies.VisionCone
{
    public class VisionConeDistractPoint: IVisionConeControlPoint
    {
        public float FieldOfView { get; }
        public Vector3 Position { get; }
    
        public VisionConeDistractPoint(float fieldOfView, Vector3 position)
        {
            FieldOfView = fieldOfView;
            Position = position;
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(Position, 0.1f);
        }
    }
}
