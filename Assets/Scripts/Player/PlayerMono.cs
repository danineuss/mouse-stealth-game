using System.Collections.Generic;
using UnityEngine;

public class PlayerMono: MonoBehaviour 
{
    [SerializeField] private EventsMono eventsMono;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    public PlayerVM PlayerVM => playerVM;

    private PlayerVM playerVM;

    void Awake() 
    {
        var playerAbilities = new PlayerAbilities(
            eventsMono.PlayerEvents, new Dictionary<KeyCode, IPlayerAbility>()
        );
        var playerInput = new PlayerInput();

        var cameraTransform = GetComponentInChildren<Camera>().gameObject.transform;
        var cameraController = new FirstPersonCameraController(
            transform, cameraTransform, playerInput, rotationSpeed
        );
        var characterController = new FirstPersonCharacterController(
            transform, playerInput, movementSpeed
        );

        playerVM = new PlayerVM(
            gameObject.transform, 
            cameraController, 
            characterController, 
            playerInput, 
            playerAbilities, 
            eventsMono.PlayerEvents, 
            eventsMono.EnemyEvents
        );
    }

    void Update() 
    {
        playerVM.Update();
    }

    void LateUpdate() 
    {
        playerVM.LateUpdate();
    }   

    void OnTriggerEnter(Collider collider) 
    {
        playerVM.OnTriggerEnter(collider);
    }
}
