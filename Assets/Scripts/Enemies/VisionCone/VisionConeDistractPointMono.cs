using UnityEngine;

namespace Enemies.VisionCone
{
    public class VisionConeDistractPointMono : MonoBehaviour {
        [SerializeField] private float fieldOfView; 
    
        public IVisionConeControlPoint DistractPoint => new VisionConeDistractPoint(
            fieldOfView,
            transform.position
        );
    
        public void OnDrawGizmos() {
            DistractPoint.OnDrawGizmos();
        }
    }
}
