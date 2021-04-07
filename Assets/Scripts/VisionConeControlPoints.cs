using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VisionConeControlPoints : MonoBehaviour
{
    public List<VisionConeControlPoint> controlPoints;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.25f);
        if (controlPoints.Count == 0) { return; }

        foreach (var controlPoint in controlPoints) {
            DrawGizmosForControlPoint(controlPoint);
        }
    }

    private void DrawGizmosForControlPoint(VisionConeControlPoint controlPoint) {
        Vector3 positionControlPoint = controlPoint.transform.position;
        var radius = (positionControlPoint - transform.position).magnitude * Mathf.Tan(controlPoint.FieldOfView / 2 * Mathf.Deg2Rad);

        Gizmos.DrawLine(transform.position, positionControlPoint);
        Gizmos.DrawWireSphere(positionControlPoint, radius);
    }
}
