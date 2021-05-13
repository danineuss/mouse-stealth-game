using System;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerVM
{
    void Update();
    void LateUpdate();
    void LockCursor(bool locked);
    void OnTriggerEnter(Collider collider);
}

public class PlayerVM : IPlayerVM
{
    private Transform playerTransform;
    private IPlayerInput playerInput;
    private IFirstPersonCameraController cameraController;
    private IFirstPersonCharacterController characterController;
    private IPlayerAbilities playerAbilities;
    private IPlayerEvents playerEvents;
    private IEnemyEvents enemyEvents;
    private IEnemyVM targetEnemy;

    public PlayerVM(
        Transform playerTransform,
        IFirstPersonCameraController cameraController,
        IFirstPersonCharacterController characterController,
        IPlayerInput playerInput,
        IPlayerAbilities playerAbilities,
        IPlayerEvents playerEvents,
        IEnemyEvents enemyEvents)
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
        playerInput.HandleGenericPlayerInput();
        characterController.MoveCharacter();
    }

    public void LateUpdate()
    {
        cameraController.RotateForPlayerInput();
        characterController.RestrictCharacterMovement();
    }

    public void OnTriggerEnter(Collider collider)
    {
        characterController.OnTriggerEnter(collider);
    }

    public void LockCursor(bool locked)
    {
        cameraController.LockCursor(locked);
    }

    void ApplyPlayerAbilityInput()
    {
        if (playerAbilities.Abilities.Count == 0)
            return;

        foreach (var keyCode in playerAbilities.RelevantKeyPresses)
        {
            if (playerInput.GetKeyDown(keyCode))
                playerAbilities.ExecuteAbility(playerAbilities.Abilities[keyCode], targetEnemy);
        }
    }

    void OnCursorEnterEnemy(IEnemyVM enemyVM)
    {
        targetEnemy = enemyVM;
        if (playerAbilities.RelevantAbilities.Count > 0)
        {
            playerEvents.SendPlayerLocation(enemyVM, true, playerTransform);
        }
        else
        {
            playerEvents.SendPlayerLocation(enemyVM, false, null);
        }
    }

    void OnCurserExitEnemy()
    {
        if(targetEnemy == null)
            return;

        playerEvents.RemovePlayerLocation(targetEnemy);
        targetEnemy = null;
    }

    void OnAbilityLearned(IPlayerAbility ability)
    {
        playerAbilities.LearnAbility(ability);
    }
}
