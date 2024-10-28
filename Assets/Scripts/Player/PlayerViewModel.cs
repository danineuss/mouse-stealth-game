using System;
using Enemies;
using Infrastructure;
using Scenes;
using UnityEngine;

namespace Player
{
    public interface IPlayerViewModel: IUpdatable, ILateUpdatable {}

    public class PlayerViewModel : IPlayerViewModel
    {
        private Guid targetEnemyID;

        private readonly Transform playerTransform;
        private readonly IPlayerInput playerInput;
        private readonly IFirstPersonCameraController cameraController;
        private readonly IFirstPersonCharacterController characterController;
        private readonly IPanicMeter panicMeter;
        private readonly IPlayerAbilities playerAbilities;
        private readonly IPlayerEvents playerEvents;
        private readonly IEnemyEvents enemyEvents;
        private readonly ISceneEvents sceneEvents;

        public PlayerViewModel(
            Transform playerTransform,
            IFirstPersonCameraController cameraController,
            IFirstPersonCharacterController characterController,
            IPlayerInput playerInput,
            IPlayerAbilities playerAbilities,
            IPanicMeter panicMeter,
            IPlayerEvents playerEvents,
            IEnemyEvents enemyEvents,
            ISceneEvents sceneEvents)
        {
            this.playerTransform = playerTransform;
            this.cameraController = cameraController;
            this.characterController = characterController;
            this.playerInput = playerInput;
            this.playerAbilities = playerAbilities;
            this.panicMeter = panicMeter;
            this.playerEvents = playerEvents;
            this.enemyEvents = enemyEvents;
            this.sceneEvents = sceneEvents;

            targetEnemyID = Guid.Empty;

            InitializeEvents();
        }

        void InitializeEvents()
        {
            enemyEvents.OnCursorEnterEnemy += OnCursorEnterEnemy;
            enemyEvents.OnCurserExitEnemy += OnCurserExitEnemy;
            enemyEvents.OnGameFailed += delegate{ cameraController.LockCursor(false); };
            sceneEvents.OnGamePaused += ToggleCursorLockForGamePaused;
            sceneEvents.OnDialogOpened += delegate{ cameraController.LockCursor(false); };
            sceneEvents.OnDialogClosed += delegate{ cameraController.LockCursor(true); };
            playerEvents.OnAbilityLearned += OnAbilityLearned;
        }

        public void Update()
        {
            ApplyPlayerAbilityInput();
            playerInput.HandleGenericPlayerInput();
            characterController.UpdateCharacterPosition();
            panicMeter.UpdatePanicLevel();
        }

        void ApplyPlayerAbilityInput()
        {
            if (playerAbilities.Abilities.Count == 0)
                return;

            foreach (var keyCode in playerAbilities.RelevantKeyPresses)
            {
                if (playerInput.GetKeyDown(keyCode))
                    playerAbilities.ExecuteAbility(playerAbilities.Abilities[keyCode], targetEnemyID);
            }
        }

        public void LateUpdate()
        {
            cameraController.RotateForPlayerInput();
        }

        void OnCursorEnterEnemy(Guid enemyID)
        {
            targetEnemyID = enemyID;
            if (playerAbilities.RelevantAbilities.Count > 0)
            {
                playerEvents.SendPlayerLocation(enemyID, true, playerTransform);
            }
            else
            {
                playerEvents.SendPlayerLocation(enemyID, false, null);
            }
        }

        void OnCurserExitEnemy()
        {
            if(targetEnemyID == Guid.Empty)
                return;

            playerEvents.RemovePlayerLocation(targetEnemyID);
            targetEnemyID = Guid.Empty;
        }

        void ToggleCursorLockForGamePaused(bool gamePaused)
        {
            cameraController.LockCursor(!gamePaused);
        }

        void OnAbilityLearned(IPlayerAbility ability)
        {
            playerAbilities.LearnAbility(ability);
        }
    }
}