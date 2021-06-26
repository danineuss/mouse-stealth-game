using System.Collections.Generic;
using UnityEngine;

public class PlayerMono: MonoBehaviour 
{
    [SerializeField] private EventsMono eventsMono = null;
    [SerializeField] private float minMovementSpeed = 0f;
    [SerializeField] private float maxMovementSpeed = 0f;
    [SerializeField] private float radiusStartSpeedDecrease = 0f;
    [SerializeField] private float radiusStartFear = 0f;
    [SerializeField] private float panicEscalationSpeed = 0f;
    [SerializeField] private float panicDeescalationSpeed = 0f;
    [SerializeField] private LayerMask safeRoomObjectsLayerMask = new LayerMask();
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
            transform, 
            minMovementSpeed, 
            maxMovementSpeed,
            radiusStartSpeedDecrease,
            radiusStartFear,
            safeRoomObjectsLayerMask,
            playerInput,
            eventsMono.PlayerEvents
        );

        var panicMeter = 
            new PanicMeter(panicEscalationSpeed, panicDeescalationSpeed, eventsMono.PlayerEvents);
        playerVM = new PlayerVM(
            gameObject.transform, 
            cameraController, 
            characterController, 
            playerInput, 
            playerAbilities, 
            panicMeter,
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

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0);
        Gizmos.DrawWireSphere(transform.position, radiusStartFear);

        Gizmos.color = new Color(0.9f, 0.4f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, radiusStartSpeedDecrease);
    }
}
