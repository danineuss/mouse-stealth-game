
using UnityEngine;

public class ThirdPersonCharacterController : MonoBehaviour {
    public float speed;
    public IPlayerMovementRestictable currentRestictable = null;

    void Update() {
        MovementInput();
    }

    void LateUpdate() {
        RestrictPlayerMovement();        
    }

    void OnTriggerEnter (Collider collider) {
        IPlayerMovementRestictable restictable = collider.GetComponentInParent<IPlayerMovementRestictable>();
        if (restictable == null || restictable == currentRestictable) { 
            return; 
        }

        currentRestictable = restictable;
    }

    void MovementInput() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 playerMovement = new Vector3(horizontal, 0f, vertical).normalized * speed * Time.deltaTime;

        transform.Translate(playerMovement, Space.Self);
    }

    void RestrictPlayerMovement() {
        if (currentRestictable == null) { return; } 
        
        var newPosition = currentRestictable.RestrictPlayerMovement(transform.position);
        transform.position = newPosition;
    }
}
