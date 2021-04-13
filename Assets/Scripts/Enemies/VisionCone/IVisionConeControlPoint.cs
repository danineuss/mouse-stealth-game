using UnityEngine;

public interface IVisionConeControlPoint {
    float FieldOfView { get; }
    Vector3 Position { get; }
    void OnDrawGizmos();
}
