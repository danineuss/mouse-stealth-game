using UnityEngine;

public class PlayerVM : MonoBehaviour 
{
    [SerializeField] private EnemyEvents enemyEvents;
    [SerializeField] private PlayerEvents playerEvents;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    public PlayerEvents PlayerEvents => playerEvents;
    public IFirstPersonCameraController CameraController => cameraController;
    public IPlayerInput PlayerInput => playerInput;

    private EnemyVM targetEnemy;
    private PlayerAbilities playerAbilities;
    private IFirstPersonCameraController cameraController;
    private IFirstPersonCharacterController characterController;
    private IPlayerInput playerInput;

    void Awake() 
    {
        // Make non-monobehaviour
        playerAbilities = GetComponentInChildren<PlayerAbilities>();
        playerInput = new PlayerInput();

        var cameraTransform = GetComponentInChildren<Camera>().gameObject.transform;
        cameraController = new FirstPersonCameraController(transform, cameraTransform, playerInput, rotationSpeed);
        characterController = new FirstPersonCharacterController(transform, playerInput, movementSpeed);

        targetEnemy = null;
    }
    
    void Start() 
    {
        InitializeEvents();
    }

    void InitializeEvents() 
    {
        enemyEvents.OnCursorEnterEnemy += OnCursorEnterEnemy;
        enemyEvents.OnCurserExitEnemy += OnCurserExitEnemy;
        playerEvents.OnAbilityLearned += OnAbilityLearned;
    }

    void Update() 
    {
        CheckPlayerInput();
        characterController.MoveCharacter();
    }

    void LateUpdate() 
    {
        cameraController.RotateForPlayerInput();
        characterController.RestrictCharacterMovement();
    }

    void CheckPlayerInput() 
    {
        foreach (var keyCode in playerAbilities.RelevantKeyPresses) {
            if (playerInput.GetKeyDown(keyCode)) 
                playerAbilities.ExecuteAbility(playerAbilities.Abilities[keyCode], targetEnemy);
        }
    }

    void OnCursorEnterEnemy(EnemyVM enemyVM) 
    {
        targetEnemy = enemyVM;
        if (playerAbilities.RelevantAbilities.Count > 0) {
            playerEvents.SendPlayerLocation(enemyVM, true, transform);
        } else {
            playerEvents.SendPlayerLocation(enemyVM, false, null);
        }
    }

    void OnCurserExitEnemy() 
    {
        playerEvents.RemovePlayerLocation(targetEnemy);
        targetEnemy = null;
    }

    void OnAbilityLearned(IPlayerAbility ability) 
    {
        playerAbilities.LearnAbility(ability);
    }

    void OnTriggerEnter(Collider collider) 
    {
        characterController.OnTriggerEnter(collider);
    }
}
