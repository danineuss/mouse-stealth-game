using System.Collections.Generic;
using UnityEngine;

// public class PlayerMono_Mock : PlayerMono {
//     public EnemyEvents enemyEvents;
//     public PlayerEvents playerEvents;
//     public float movementSpeed;
//     public float rotationSpeed;
//     public new PlayerVM PlayerVM;

//     public override void Update()
//     {
//         PlayerVM.Update();
//     }

//     public override void LateUpdate()
//     {
//         PlayerVM.LateUpdate();
//     }
// }

public class PlayerMono: MonoBehaviour 
{
    [SerializeField] private EnemyEvents enemyEvents;
    [SerializeField] private PlayerEvents playerEvents;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    public PlayerVM PlayerVM;

    void Awake() 
    {
        var playerAbilities = new PlayerAbilities(playerEvents, new Dictionary<KeyCode, IPlayerAbility>());
        var playerInput = new PlayerInput();

        var cameraTransform = GetComponentInChildren<Camera>().gameObject.transform;
        var cameraController = new FirstPersonCameraController(transform, cameraTransform, playerInput, rotationSpeed);
        var characterController = new FirstPersonCharacterController(transform, playerInput, movementSpeed);

        PlayerVM = new PlayerVM(
            gameObject, cameraController, characterController, playerInput, playerAbilities, playerEvents, enemyEvents);
    }

    public virtual void Update() 
    {
        PlayerVM.Update();
    }

    public virtual void LateUpdate() 
    {
        PlayerVM.LateUpdate();
    }
}
