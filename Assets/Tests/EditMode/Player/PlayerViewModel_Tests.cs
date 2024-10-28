using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Audio;
using Enemies;
using Player;
using Scenes;
using UI;
using UnityEngine;

namespace Tests
{
    public class PlayerViewModel_Tests
    {
        private GameObject gameObject = new GameObject("Player");
        private IFirstPersonCameraController cameraController;
        private IFirstPersonCharacterController characterController;
        private IPlayerInput playerInput;
        private IPlayerAbilities playerAbilities;
        private IPanicMeter panicMeter;
        private IPanicNoiseEmitter panicNoiseEmitter;
        private IPlayerEvents playerEvents;
        private IEnemyEvents enemyEvents;
        private ISceneEvents sceneEvents;
        private PlayerViewModel playerViewModel;

        private void SetupDependecies()
        {
            cameraController = Substitute.For<IFirstPersonCameraController>();
            characterController = Substitute.For<IFirstPersonCharacterController>();
            playerInput = Substitute.For<IPlayerInput>();
            playerAbilities = Substitute.For<IPlayerAbilities>();
            panicMeter = Substitute.For<IPanicMeter>();
            panicNoiseEmitter = Substitute.For<IPanicNoiseEmitter>();
            playerEvents = Substitute.For<IPlayerEvents>();
            enemyEvents = Substitute.For<IEnemyEvents>();
            sceneEvents = Substitute.For<ISceneEvents>(); 
        }

        private void SetupPlayerViewModel()
        {
            SetupDependecies(); 
            playerViewModel = new PlayerViewModel(
                gameObject.transform, 
                cameraController,
                characterController,
                playerInput,
                playerAbilities,
                panicMeter,
                panicNoiseEmitter,
                playerEvents,
                enemyEvents,
                sceneEvents
            );
        }

        private void SetupPlayerViewModel(IPlayerAbilities playerAbilities)
        {
            SetupDependecies(); 
            playerViewModel = new PlayerViewModel(
                gameObject.transform, 
                cameraController,
                characterController,
                playerInput,
                playerAbilities,
                panicMeter,
                panicNoiseEmitter,
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
            var enemyVM = Substitute.For<IEnemyViewModel>();
            enemyVM.ID.Returns(Guid.NewGuid());
            SetupPlayerViewModel(playerAbilities);
            
            enemyEvents.OnCursorEnterEnemy += Raise.Event<Action<Guid>>(enemyVM.ID);
            
            playerEvents.Received().SendPlayerLocation(enemyVM.ID, true, gameObject.transform);
            playerEvents.DidNotReceive().SendPlayerLocation(enemyVM.ID, false, null);
        }

        [Test]
        public void should_not_send_player_location_with_abilities_empty()
        {
            var playerAbilities = Substitute.For<IPlayerAbilities>();
            playerAbilities.RelevantAbilities.Returns(new List<IPlayerAbility>());
            var enemyVM = Substitute.For<IEnemyViewModel>();
            enemyVM.ID.Returns(Guid.NewGuid());
            SetupPlayerViewModel(playerAbilities);
            
            enemyEvents.OnCursorEnterEnemy += Raise.Event<Action<Guid>>(enemyVM.ID);
            
            playerEvents.DidNotReceive().SendPlayerLocation(enemyVM.ID, true, gameObject.transform);
            playerEvents.Received().SendPlayerLocation(enemyVM.ID, false, null);
        }

        [Test]
        public void should_remove_player_location_for_enemy()
        {
            var playerAbilities = Substitute.For<IPlayerAbilities>();
            playerAbilities.RelevantAbilities.Returns(new List<IPlayerAbility>());
            var enemyVM = Substitute.For<IEnemyViewModel>();
            enemyVM.ID.Returns(Guid.NewGuid());
            SetupPlayerViewModel(playerAbilities);
            
            enemyEvents.OnCursorEnterEnemy += Raise.Event<Action<Guid>>(enemyVM.ID);
            enemyEvents.OnCurserExitEnemy += Raise.Event<Action>();
            
            playerEvents.Received().RemovePlayerLocation(enemyVM.ID);
        }

        [Test]
        public void should_not_remove_player_location_for_enemy_iff_no_enemy()
        {
            var enemyVM = Substitute.For<IEnemyViewModel>();
            enemyVM.ID.Returns(Guid.NewGuid());
            SetupPlayerViewModel();
            
            enemyEvents.OnCurserExitEnemy += Raise.Event<Action>();
            
            playerEvents.DidNotReceiveWithAnyArgs().RemovePlayerLocation(default);
        }

        [Test]
        public void should_pass_along_ability_for_abiltiy_learned_event()
        {
            SetupPlayerViewModel();
            var ability = new DummyAbility(KeyCode.A, 10f);

            playerEvents.OnAbilityLearned += Raise.Event<Action<IPlayerAbility>>(ability);

            playerAbilities.Received().LearnAbility(ability);
        }

        [Test]
        public void should_trigger_ability_only_for_correct_input()
        {
            SetupPlayerViewModel();
            var ability = new DummyAbility(KeyCode.A, 10f);
            var playerInput = Substitute.For<IPlayerInput>();
            playerInput.GetKeyDown(default).ReturnsForAnyArgs(false);
            var playerAbilities = Substitute.For<IPlayerAbilities>();
            playerAbilities.Abilities.Returns(new Dictionary<KeyCode, IPlayerAbility>() { 
                { ability.AssociatedKey, ability } 
            });
            playerAbilities.RelevantKeyPresses.Returns(new List<KeyCode>(){ ability.AssociatedKey });
            playerViewModel = new PlayerViewModel(
                gameObject.transform, 
                cameraController,
                characterController,
                playerInput,
                playerAbilities,
                panicMeter,
                panicNoiseEmitter,
                playerEvents,
                enemyEvents,
                sceneEvents
            );

            playerViewModel.Update();

            playerAbilities.DidNotReceiveWithAnyArgs().ExecuteAbility(default, default);

            playerInput.GetKeyDown(ability.AssociatedKey).Returns(true);
            playerViewModel = new PlayerViewModel(
                gameObject.transform, 
                cameraController,
                characterController,
                playerInput,
                playerAbilities,
                panicMeter,
                panicNoiseEmitter,
                playerEvents,
                enemyEvents,
                sceneEvents
            );

            playerViewModel.Update();
            playerAbilities.Received().ExecuteAbility(ability, Guid.Empty);
        }

        [Test]
        public void should_lock_cursor_when_game_paused_and_reverse()
        {
            var playerAbilities = Substitute.For<IPlayerAbilities>();
            playerAbilities.RelevantAbilities.Returns(new List<IPlayerAbility>());
            SetupPlayerViewModel(playerAbilities);
            
            sceneEvents.OnGamePaused += Raise.Event<Action<bool>>(true);
            
            cameraController.Received().LockCursor(false);

            sceneEvents.OnGamePaused += Raise.Event<Action<bool>>(false);
            
            cameraController.Received().LockCursor(true);
        }

        [Test]
        public void should_lock_cursor_when_dialog_opened_and_reverse()
        {
            SetupPlayerViewModel();
            
            var dialogVM = Substitute.For<IDialogViewModel>();
            sceneEvents.OnDialogOpened += Raise.Event<Action<IDialogViewModel>>(dialogVM);
            
            cameraController.Received().LockCursor(false);

            sceneEvents.OnDialogClosed += Raise.Event<Action<IDialogViewModel>>(dialogVM);
            
            cameraController.Received().LockCursor(true);
        }

        [Test]
        public void should_lock_cursor_when_game_failed()
        {
            SetupPlayerViewModel();
            
            enemyEvents.OnGameFailed += Raise.Event<Action>();
            
            cameraController.Received().LockCursor(false);

            enemyEvents.OnGameFailed += Raise.Event<Action>();
            
            cameraController.Received().LockCursor(false);
        }
    }
}