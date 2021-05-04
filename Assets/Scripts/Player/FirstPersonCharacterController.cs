
using UnityEngine;

public class FirstPersonCharacterController : MonoBehaviour {
    public float speed;
    public IPlayerMovementRestictable currentRestictable = null;
    //Make Readonly
    private IPlayerInput playerInput;

    // REMOVE
    void Awake() {
        playerInput = new PlayerInput();
    }

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
        var input = new Vector3(playerInput.Horizontal, 0f, playerInput.Vertical).normalized;
        Vector3 playerMovement = input * speed * Time.deltaTime;

        transform.Translate(playerMovement, Space.Self);
    }

    void RestrictPlayerMovement() {
        if (currentRestictable == null)
            return;
        
        var newPosition = currentRestictable.RestrictPlayerMovement(transform.position);
        transform.position = newPosition;
    }
}
