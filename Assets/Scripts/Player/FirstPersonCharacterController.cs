
using UnityEngine;

public interface IFirstPersonCharacterController
{
    void MoveCharacter();
    void RestrictCharacterMovement();
    void OnTriggerEnter(Collider collider);
}

public class FirstPersonCharacterController : IFirstPersonCharacterController
{
    private Transform characterTransform;
    private readonly IPlayerInput playerInput;
    private IPlayerMovementRestictable currentRestictable;
    private float movementSpeed;

    public FirstPersonCharacterController(Transform character, IPlayerInput playerInput, float movementSpeed) 
    {
        characterTransform = character;
        this.playerInput = playerInput;
        currentRestictable = null;
    }

    public void MoveCharacter()
    {
        var input = new Vector3(playerInput.Horizontal, 0f, playerInput.Vertical).normalized;
        Vector3 playerMovement = input * movementSpeed * Time.deltaTime;

        characterTransform.Translate(playerMovement, Space.Self);
    }

    public void RestrictCharacterMovement()
    {
        if (currentRestictable == null)
            return;

        var newPosition = currentRestictable.RestrictPlayerMovement(characterTransform.position);
        characterTransform.position = newPosition;
    }
    
    public void OnTriggerEnter(Collider collider)
    {
        IPlayerMovementRestictable restictable = collider.GetComponentInParent<IPlayerMovementRestictable>();
        if (restictable == null || restictable == currentRestictable)
            return;

        currentRestictable = restictable;
    }
}
