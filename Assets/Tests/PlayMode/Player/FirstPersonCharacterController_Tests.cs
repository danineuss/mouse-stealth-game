using System.Collections;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class FirstPersonCharacterController_Tests
    {
        [UnityTest]
        public IEnumerator should_move_forward_when_vertical_is_pressed()
        {
            GameObject playerGameObject = new GameObject("Player");
            PlayerMono playerMono = playerGameObject.AddComponent<PlayerMono>();
            var cameraController = Substitute.For<IFirstPersonCameraController>();
            var playerAbilities = Substitute.For<IPlayerAbilities>();
            // TODO; make events into interfaces
            var playerEvents = new PlayerEvents();
            var enemyEvents = new EnemyEvents();

            var playerInput = Substitute.For<IPlayerInput>();
            playerInput.Vertical.Returns(1f);
            var characterController = new FirstPersonCharacterController(playerGameObject.transform, playerInput, 1f);
            playerMono.PlayerVM = new PlayerVM(
                playerGameObject.transform, 
                cameraController, 
                characterController,
                playerInput,
                playerAbilities, 
                playerEvents,
                enemyEvents);
            
            yield return new WaitForSeconds(1f);

            Assert.Greater(playerMono.transform.position.z, 0f);
            Assert.AreEqual(0f, playerMono.transform.position.x);
            Assert.AreEqual(0f, playerMono.transform.position.y);
        }
    }
}
