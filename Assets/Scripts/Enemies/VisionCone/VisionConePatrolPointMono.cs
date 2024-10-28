using UnityEngine;

namespace Enemies.VisionCone
{
    public class VisionConePatrolPointMono: MonoBehaviour
    {
        [SerializeField] private float fieldOfView = 0f;
        [SerializeField] private float durationTowardsPoint = 1f;
        [SerializeField] private float waitTimeAtTarget = 0.5f;

        public IVisionConePatrolPoint PatrolPoint => new VisionConePatrolPoint(
            fieldOfView,
            transform.position,
            durationTowardsPoint,
            waitTimeAtTarget
        );

        public void OnDrawGizmos()
        {
            PatrolPoint.OnDrawGizmos();
        }
    }
}