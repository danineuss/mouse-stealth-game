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

        [Test]
        public void should_send_player_location_with_abilities_non_empty()
        {
            var cameraController = Substitute.For<IFirstPersonCameraController>();
            var characterController = Substitute.For<IFirstPersonCharacterController>();
            var playerInput = Substitute.For<IPlayerInput>();
            var playerAbilities = Substitute.For<IPlayerAbilities>();
            var relevantAbilities = new List<IPlayerAbility>() { new DummyAbility(KeyCode.A, 10f) };
            playerAbilities.RelevantAbilities.Returns(relevantAbilities);
            var playerEvents = Substitute.For<IPlayerEvents>();
            var enemyEvents = Substitute.For<IEnemyEvents>();
            var playerVM = new PlayerVM(
                gameObject.transform, 
                cameraController,
                characterController,
                playerInput,
                playerAbilities,
                playerEvents,
                enemyEvents
            );
            var enemyVM = Substitute.For<IEnemyVM>();
            enemyVM.ID.Returns(new Guid());
            
            enemyEvents.OnCursorEnterEnemy += Raise.Event<Action<IEnemyVM>>(enemyVM);
            
            playerEvents.Received().SendPlayerLocation(enemyVM, true, gameObject.transform);
            playerEvents.DidNotReceive().SendPlayerLocation(enemyVM, false, null);
        }

        [Test]
        public void should_not_send_player_location_with_abilities_empty()
        {
            var cameraController = Substitute.For<IFirstPersonCameraController>();
            var characterController = Substitute.For<IFirstPersonCharacterController>();
            var playerInput = Substitute.For<IPlayerInput>();
            var playerAbilities = Substitute.For<IPlayerAbilities>();
            playerAbilities.RelevantAbilities.Returns(new List<IPlayerAbility>());
            var playerEvents = Substitute.For<IPlayerEvents>();
            var enemyEvents = Substitute.For<IEnemyEvents>();
            var playerVM = new PlayerVM(
                gameObject.transform, 
                cameraController,
                characterController,
                playerInput,
                playerAbilities,
                playerEvents,
                enemyEvents
            );
            var enemyVM = Substitute.For<IEnemyVM>();
            enemyVM.ID.Returns(new Guid());
            
            enemyEvents.OnCursorEnterEnemy += Raise.Event<Action<IEnemyVM>>(enemyVM);
            
            playerEvents.DidNotReceive().SendPlayerLocation(enemyVM, true, gameObject.transform);
            playerEvents.Received().SendPlayerLocation(enemyVM, false, null);
        }

        [Test]
        public void should_remove_player_location_for_enemy()
        {
            var cameraController = Substitute.For<IFirstPersonCameraController>();
            var characterController = Substitute.For<IFirstPersonCharacterController>();
            var playerInput = Substitute.For<IPlayerInput>();
            var playerAbilities = Substitute.For<IPlayerAbilities>();
            playerAbilities.RelevantAbilities.Returns(new List<IPlayerAbility>());
            var playerEvents = Substitute.For<IPlayerEvents>();
            var enemyEvents = Substitute.For<IEnemyEvents>();
            var playerVM = new PlayerVM(
                gameObject.transform, 
                cameraController,
                characterController,
                playerInput,
                playerAbilities,
                playerEvents,
                enemyEvents
            );
            var enemyVM = Substitute.For<IEnemyVM>();
            enemyVM.ID.Returns(new Guid());
            
            enemyEvents.OnCursorEnterEnemy += Raise.Event<Action<IEnemyVM>>(enemyVM);
            enemyEvents.OnCurserExitEnemy += Raise.Event<Action>();
            
            playerEvents.Received().RemovePlayerLocation(enemyVM);
        }

        [Test]
        public void should_not_remove_player_location_for_enemy_iff_no_enemy()
        {
            var cameraController = Substitute.For<IFirstPersonCameraController>();
            var characterController = Substitute.For<IFirstPersonCharacterController>();
            var playerInput = Substitute.For<IPlayerInput>();
            var playerAbilities = Substitute.For<IPlayerAbilities>();
            var playerEvents = Substitute.For<IPlayerEvents>();
            var enemyEvents = Substitute.For<IEnemyEvents>();
            var playerVM = new PlayerVM(
                gameObject.transform, 
                cameraController,
                characterController,
                playerInput,
                playerAbilities,
                playerEvents,
                enemyEvents
            );
            var enemyVM = Substitute.For<IEnemyVM>();
            enemyVM.ID.Returns(new Guid());
            
            enemyEvents.OnCurserExitEnemy += Raise.Event<Action>();
            
            playerEvents.DidNotReceiveWithAnyArgs().RemovePlayerLocation(default);
        }

        [Test]
        public void should_pass_along_ability_for_abiltiy_learned_event()
        {
            var cameraController = Substitute.For<IFirstPersonCameraController>();
            var characterController = Substitute.For<IFirstPersonCharacterController>();
            var playerInput = Substitute.For<IPlayerInput>();
            var playerAbilities = Substitute.For<IPlayerAbilities>();
            var playerEvents = Substitute.For<IPlayerEvents>();
            var enemyEvents = Substitute.For<IEnemyEvents>();
            var playerVM = new PlayerVM(
                gameObject.transform, 
                cameraController,
                characterController,
                playerInput,
                playerAbilities,
                playerEvents,
                enemyEvents
            );
            var ability = new DummyAbility(KeyCode.A, 10f);

            playerEvents.OnAbilityLearned += Raise.Event<Action<IPlayerAbility>>(ability);

            playerAbilities.Received().LearnAbility(ability);
        }

        [Test]
        public void should_trigger_ability_only_for_correct_input()
        {
            var ability = new DummyAbility(KeyCode.A, 10f);
            var cameraController = Substitute.For<IFirstPersonCameraController>();
            var characterController = Substitute.For<IFirstPersonCharacterController>();
            var playerEvents = Substitute.For<IPlayerEvents>();
            var enemyEvents = Substitute.For<IEnemyEvents>();

            var playerInput = Substitute.For<IPlayerInput>();
            playerInput.GetKeyDown(default).ReturnsForAnyArgs(false);
            var playerAbilities = Substitute.For<IPlayerAbilities>();
            playerAbilities.Abilities.Returns(new Dictionary<KeyCode, IPlayerAbility>() { 
                { ability.AssociatedKey, ability } 
            });
            playerAbilities.RelevantKeyPresses.Returns(new List<KeyCode>(){ ability.AssociatedKey });
            var playerVM = new PlayerVM(
                gameObject.transform, 
                cameraController,
                characterController,
                playerInput,
                playerAbilities,
                playerEvents,
                enemyEvents
            );

            playerVM.Update();

            playerAbilities.DidNotReceiveWithAnyArgs().ExecuteAbility(default);

            playerInput.GetKeyDown(ability.AssociatedKey).Returns(true);
            playerVM = new PlayerVM(
                gameObject.transform, 
                cameraController,
                characterController,
                playerInput,
                playerAbilities,
                playerEvents,
                enemyEvents
            );

            playerVM.Update();
            playerAbilities.Received().ExecuteAbility(ability);
        }
    }
}