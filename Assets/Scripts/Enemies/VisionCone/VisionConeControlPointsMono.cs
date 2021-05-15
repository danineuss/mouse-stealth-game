using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VisionConeControlPointsMono : MonoBehaviour {
    public List<VisionConePatrolPoint> patrolPoints;
    public VisionConeDistractPoint distractPoint;

    public void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.25f);

        if (patrolPoints.Count == 0)
            return;

        foreach (var controlPoint in patrolPoints) {
            DrawGizmosForPoint(controlPoint);
        }

        Gizmos.color = Color.blue;
        DrawGizmosForPoint(distractPoint);
    }

    private void DrawGizmosForPoint(IVisionConeControlPoint controlPoint) {
        Vector3 positionControlPoint = controlPoint.Position;
        var radius = (positionControlPoint - transform.position).magnitude 
                        * Mathf.Tan(controlPoint.FieldOfView / 2 * Mathf.Deg2Rad);

        Gizmos.DrawLine(transform.position, positionControlPoint);
        Gizmos.DrawWireSphere(positionControlPoint, radius);
    }
}
