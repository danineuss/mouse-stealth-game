using UnityEngine;

namespace Enemies.VisionCone
{
    public class VisionConeDistractPointMono : MonoBehaviour {
        [SerializeField] private float fieldOfView = 0f; 
    
        public IVisionConeControlPoint DistractPoint => new VisionConeDistractPoint(
            fieldOfView,
            transform.position
        );
    
        public void OnDrawGizmos() {
            DistractPoint.OnDrawGizmos();
        }
    }
}
