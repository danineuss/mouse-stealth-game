using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VisionConeGizmoVisualizer : MonoBehaviour
{
    public List<VisionControlPoint> visionControlPoints;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.25f);
        if (visionControlPoints.Count == 0) { return; }

        foreach (var controlPoint in visionControlPoints) {
            DrawGizmosForControlPoint(controlPoint);
        }
    }

    private void DrawGizmosForControlPoint(VisionControlPoint controlPoint) {
        Vector3 positionControlPoint = controlPoint.transform.position;
        var radius = (positionControlPoint - transform.position).magnitude * Mathf.Tan(controlPoint.FieldOfView / 2 * Mathf.Deg2Rad);

        Gizmos.DrawLine(transform.position, positionControlPoint);
        Gizmos.DrawWireSphere(positionControlPoint, radius);
    }
}
