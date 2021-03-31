using UnityEngine;
using UnityEngine.Assertions;

public class RailPlayerRestriction : MonoBehaviour, IPlayerMovementRestictable {
    private Collider railCollider;    
    void Start()
    {
        railCollider = GetComponent<Collider>();
        Assert.IsNotNull(railCollider);
    }
    
    public Vector3 RestrictPlayerMovement (Vector3 PlayerPosition) {        
        var closestPoint = railCollider.ClosestPoint(PlayerPosition);
        return new Vector3(closestPoint.x, PlayerPosition.y, closestPoint.z);
    }
}
