using UnityEngine;
using UnityEngine.Assertions;

public class PlayerRoamingArea : MonoBehaviour, IPlayerMovementRestictable {
    private Collider boxCollider;
    
    void Start() {
        boxCollider = GetComponent<Collider>();
        Assert.IsNotNull(boxCollider);
    }

    public Vector3 RestrictPlayerMovement (Vector3 playerPosition) {
        var bounds = boxCollider.bounds;
        var posX = Mathf.Clamp(playerPosition.x, bounds.min.x, bounds.max.x);
        var posZ = Mathf.Clamp(playerPosition.z, bounds.min.z, bounds.max.z);
        return new Vector3(posX, playerPosition.y, posZ);
    }
}
