using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
    public class PlayerVM_Tests
    {
        [Test]
        public void should_send_player_location_with_abilities_non_empty()
        {
            GameObject gameObject = new GameObject("Player");
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
            GameObject gameObject = new GameObject("Player");
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
            GameObject gameObject = new GameObject("Player");
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
            GameObject gameObject = new GameObject("Player");
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
    }
}