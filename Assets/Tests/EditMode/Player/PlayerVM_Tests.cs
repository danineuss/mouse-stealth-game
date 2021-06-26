using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
    public class PlayerVM_Tests
    {
        private GameObject gameObject = new GameObject("Player");
        private IFirstPersonCameraController cameraController;
        private IFirstPersonCharacterController characterController;
        private IPlayerInput playerInput;
        private IPlayerAbilities playerAbilities;
        private IPanicMeter panicMeter;
        private IPlayerEvents playerEvents;
        private IEnemyEvents enemyEvents;
        private ISceneEvents sceneEvents;
        private PlayerVM playerVM;

        private void SetupDependecies()
        {
            cameraController = Substitute.For<IFirstPersonCameraController>();
            characterController = Substitute.For<IFirstPersonCharacterController>();
            playerInput = Substitute.For<IPlayerInput>();
            playerAbilities = Substitute.For<IPlayerAbilities>();
            panicMeter = Substitute.For<IPanicMeter>();
            playerEvents = Substitute.For<IPlayerEvents>();
            enemyEvents = Substitute.For<IEnemyEvents>();
            sceneEvents = Substitute.For<ISceneEvents>(); 
        }

        private void SetupPlayerVM()
        {
            SetupDependecies(); 
            playerVM = new PlayerVM(
                gameObject.transform, 
                cameraController,
                characterController,
                playerInput,
                playerAbilities,
                panicMeter,
                playerEvents,
                enemyEvents,
                sceneEvents
            );
        }

        private void SetupPlayerVM(IPlayerAbilities playerAbilities)
        {
            SetupDependecies(); 
            playerVM = new PlayerVM(
                gameObject.transform, 
                cameraController,
                characterController,
                playerInput,
                playerAbilities,
                panicMeter,
                playerEvents,
                enemyEvents,
                sceneEvents
            );
        }

        [Test]
        public void should_send_player_location_with_abilities_non_empty()
        {
            var playerAbilities = Substitute.For<IPlayerAbilities>();
            var relevantAbilities = new List<IPlayerAbility>() { new DummyAbility(KeyCode.A, 10f) };
            playerAbilities.RelevantAbilities.Returns(relevantAbilities);
            var enemyVM = Substitute.For<IEnemyVM>();
            enemyVM.ID.Returns(Guid.NewGuid());
            SetupPlayerVM(playerAbilities);
            
            enemyEvents.OnCursorEnterEnemy += Raise.Event<Action<Guid>>(enemyVM.ID);
            
            playerEvents.Received().SendPlayerLocation(enemyVM.ID, true, gameObject.transform);
            playerEvents.DidNotReceive().SendPlayerLocation(enemyVM.ID, false, null);
        }

        [Test]
        public void should_not_send_player_location_with_abilities_empty()
        {
            var playerAbilities = Substitute.For<IPlayerAbilities>();
            playerAbilities.RelevantAbilities.Returns(new List<IPlayerAbility>());
            var enemyVM = Substitute.For<IEnemyVM>();
            enemyVM.ID.Returns(Guid.NewGuid());
            SetupPlayerVM(playerAbilities);
            
            enemyEvents.OnCursorEnterEnemy += Raise.Event<Action<Guid>>(enemyVM.ID);
            
            playerEvents.DidNotReceive().SendPlayerLocation(enemyVM.ID, true, gameObject.transform);
            playerEvents.Received().SendPlayerLocation(enemyVM.ID, false, null);
        }

        [Test]
        public void should_remove_player_location_for_enemy()
        {
            var playerAbilities = Substitute.For<IPlayerAbilities>();
            playerAbilities.RelevantAbilities.Returns(new List<IPlayerAbility>());
            var enemyVM = Substitute.For<IEnemyVM>();
            enemyVM.ID.Returns(Guid.NewGuid());
            SetupPlayerVM(playerAbilities);
            
            enemyEvents.OnCursorEnterEnemy += Raise.Event<Action<Guid>>(enemyVM.ID);
            enemyEvents.OnCurserExitEnemy += Raise.Event<Action>();
            
            playerEvents.Received().RemovePlayerLocation(enemyVM.ID);
        }

        [Test]
        public void should_not_remove_player_location_for_enemy_iff_no_enemy()
        {
            var enemyVM = Substitute.For<IEnemyVM>();
            enemyVM.ID.Returns(Guid.NewGuid());
            SetupPlayerVM();
            
            enemyEvents.OnCurserExitEnemy += Raise.Event<Action>();
            
            playerEvents.DidNotReceiveWithAnyArgs().RemovePlayerLocation(default);
        }

        [Test]
        public void should_pass_along_ability_for_abiltiy_learned_event()
        {
            SetupPlayerVM();
            var ability = new DummyAbility(KeyCode.A, 10f);

            playerEvents.OnAbilityLearned += Raise.Event<Action<IPlayerAbility>>(ability);

            playerAbilities.Received().LearnAbility(ability);
        }

        [Test]
        public void should_trigger_ability_only_for_correct_input()
        {
            SetupPlayerVM();
            var ability = new DummyAbility(KeyCode.A, 10f);
            var playerInput = Substitute.For<IPlayerInput>();
            playerInput.GetKeyDown(default).ReturnsForAnyArgs(false);
            var playerAbilities = Substitute.For<IPlayerAbilities>();
            playerAbilities.Abilities.Returns(new Dictionary<KeyCode, IPlayerAbility>() { 
                { ability.AssociatedKey, ability } 
            });
            playerAbilities.RelevantKeyPresses.Returns(new List<KeyCode>(){ ability.AssociatedKey });
            playerVM = new PlayerVM(
                gameObject.transform, 
                cameraController,
                characterController,
                playerInput,
                playerAbilities,
                panicMeter,
                playerEvents,
                enemyEvents,
                sceneEvents
            );

            playerVM.Update();

            playerAbilities.DidNotReceiveWithAnyArgs().ExecuteAbility(default, default);

            playerInput.GetKeyDown(ability.AssociatedKey).Returns(true);
            playerVM = new PlayerVM(
                gameObject.transform, 
                cameraController,
                characterController,
                playerInput,
                playerAbilities,
                panicMeter,
                playerEvents,
                enemyEvents,
                sceneEvents
            );

            playerVM.Update();
            playerAbilities.Received().ExecuteAbility(ability, Guid.Empty);
        }

        [Test]
        public void should_lock_cursor_when_game_paused_and_reverse()
        {
            var playerAbilities = Substitute.For<IPlayerAbilities>();
            playerAbilities.RelevantAbilities.Returns(new List<IPlayerAbility>());
            SetupPlayerVM(playerAbilities);
            
            sceneEvents.OnGamePaused += Raise.Event<Action<bool>>(true);
            
            cameraController.Received().LockCursor(false);

            sceneEvents.OnGamePaused += Raise.Event<Action<bool>>(false);
            
            cameraController.Received().LockCursor(true);
        }

        [Test]
        public void should_lock_cursor_when_dialog_opened_and_reverse()
        {
            SetupPlayerVM();
            
            var dialogVM = Substitute.For<IDialogVM>();
            sceneEvents.OnDialogOpened += Raise.Event<Action<IDialogVM>>(dialogVM);
            
            cameraController.Received().LockCursor(false);

            sceneEvents.OnDialogClosed += Raise.Event<Action<IDialogVM>>(dialogVM);
            
            cameraController.Received().LockCursor(true);
        }

        [Test]
        public void should_lock_cursor_when_game_failed()
        {
            SetupPlayerVM();
            
            enemyEvents.OnGameFailed += Raise.Event<Action>();
            
            cameraController.Received().LockCursor(false);

            enemyEvents.OnGameFailed += Raise.Event<Action>();
            
            cameraController.Received().LockCursor(false);
        }
    }
}