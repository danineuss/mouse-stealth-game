using System.Collections.Generic;
using UnityEngine;

public class PlayerVM 
{
    public PlayerEvents PlayerEvents => playerEvents;
    public IFirstPersonCameraController CameraController => cameraController;
    public IPlayerInput PlayerInput => playerInput;

    private Transform playerTransform;
    private IFirstPersonCameraController cameraController;
    private IFirstPersonCharacterController characterController;
    private IPlayerInput playerInput;
    private IPlayerAbilities playerAbilities;
    private EnemyEvents enemyEvents;
    private PlayerEvents playerEvents;
    private EnemyVM targetEnemy;

    // TODO: make playerEvents Iplayerevents
    public PlayerVM(
        Transform playerTransform,
        IFirstPersonCameraController cameraController, 
        IFirstPersonCharacterController characterController,
        IPlayerInput playerInput, 
        IPlayerAbilities playerAbilities,
        PlayerEvents playerEvents, 
        EnemyEvents enemyEvents)
    {
        this.playerTransform = playerTransform;
        this.cameraController = cameraController;
        this.characterController = characterController;
        this.playerInput = playerInput;
        this.playerAbilities = playerAbilities;
        this.playerEvents = playerEvents;
        this.enemyEvents = enemyEvents;

        targetEnemy = null;
        
        InitializeEvents();
    }

    void InitializeEvents() 
    {
        enemyEvents.OnCursorEnterEnemy += OnCursorEnterEnemy;
        enemyEvents.OnCurserExitEnemy += OnCurserExitEnemy;
        playerEvents.OnAbilityLearned += OnAbilityLearned;
    }

    public void Update() 
    {
        ApplyPlayerAbilityInput();
        characterController.MoveCharacter();
    }

    public void LateUpdate() 
    {
        cameraController.RotateForPlayerInput();
        characterController.RestrictCharacterMovement();
    }

    void ApplyPlayerAbilityInput() 
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
            playerEvents.SendPlayerLocation(enemyVM, true, playerTransform);
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

    // TODO: check that irrelevant.
    // void OnTriggerEnter(Collider collider) 
    // {
    //     characterController.OnTriggerEnter(collider);
    // }
}
