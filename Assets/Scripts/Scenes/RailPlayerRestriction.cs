using UnityEngine;

public class RailPlayerRestriction : MonoBehaviour, IPlayerMovementRestictable {
    private Collider railCollider;    
    
    void Awake() {
        railCollider = GetComponent<Collider>();
    }
    public Vector3 RestrictPlayerMovement (Vector3 PlayerPosition) {        
        var closestPoint = railCollider.ClosestPoint(PlayerPosition);
        return new Vector3(closestPoint.x, PlayerPosition.y, closestPoint.z);
    }
}
