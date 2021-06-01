using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VisionConeControlPointsMono : MonoBehaviour {
    [SerializeField] private List<VisionConePatrolPointMono> patrolPoints = null;
    [SerializeField] private VisionConeDistractPointMono distractPoint = null;

    public List<IVisionConePatrolPoint> PatrolPoints => patrolPoints.Select(x => x.PatrolPoint).ToList();
    public IVisionConeControlPoint DistractPoint => distractPoint.DistractPoint; 

    public void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.25f);

        if (PatrolPoints.Count == 0)
            return;

        PatrolPoints.ForEach(controlPoint => DrawGizmosForPoint(controlPoint));

        Gizmos.color = Color.blue;
        DrawGizmosForPoint(DistractPoint);
    }

    private void DrawGizmosForPoint(IVisionConeControlPoint controlPoint) {
        Vector3 positionControlPoint = controlPoint.Position;
        var radius = (positionControlPoint - transform.position).magnitude 
                        * Mathf.Tan(controlPoint.FieldOfView / 2 * Mathf.Deg2Rad);

        Gizmos.DrawLine(transform.position, positionControlPoint);
        Gizmos.DrawWireSphere(positionControlPoint, radius);
    }
}
