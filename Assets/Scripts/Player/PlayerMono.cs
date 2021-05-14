using System.Collections.Generic;
using UnityEngine;

public class PlayerMono: MonoBehaviour 
{
    [SerializeField] private EventsMono eventsMono = null;
    [SerializeField] private float movementSpeed = 0f;
    [SerializeField] private float rotationSpeed = 0f;
    public PlayerVM PlayerVM => playerVM;

    private PlayerVM playerVM;

    void Awake() 
    {
        var playerAbilities = new PlayerAbilities(
            eventsMono.PlayerEvents, new Dictionary<KeyCode, IPlayerAbility>()
        );
        var playerInput = new PlayerInput(eventsMono.PlayerEvents);

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
            eventsMono.EnemyEvents,
            eventsMono.SceneEvents
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
