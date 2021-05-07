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
        public IEnumerator should_only_move_forward_when_vertical_is_positive()
        {
            var playerInput = Substitute.For<IPlayerInput>();
            playerInput.Vertical.Returns(1f);
            var playerGameObject = PlayerMono_Mock.Dummy(playerInput);
            
            yield return new WaitForSeconds(1f);

            Assert.Greater(playerGameObject.transform.position.z, 0f);
            Assert.AreEqual(0f, playerGameObject.transform.position.x);
            Assert.AreEqual(0f, playerGameObject.transform.position.y);
        }

        [UnityTest]
        public IEnumerator should_only_move_backwards_when_vertical_is_negative()
        {
            var playerInput = Substitute.For<IPlayerInput>();
            playerInput.Vertical.Returns(-1f);
            var playerGameObject = PlayerMono_Mock.Dummy(playerInput);
            
            yield return new WaitForSeconds(1f);

            Assert.Greater(0f, playerGameObject.transform.position.z);
            Assert.AreEqual(0f, playerGameObject.transform.position.x);
            Assert.AreEqual(0f, playerGameObject.transform.position.y);
        }

        [UnityTest]
        public IEnumerator should_only_move_right_when_horizontal_is_positive()
        {
            var playerInput = Substitute.For<IPlayerInput>();
            playerInput.Horizontal.Returns(1f);
            var playerGameObject = PlayerMono_Mock.Dummy(playerInput);
            
            yield return new WaitForSeconds(1f);

            Assert.Greater(playerGameObject.transform.position.x, 0f);
            Assert.AreEqual(0f, playerGameObject.transform.position.y);
            Assert.AreEqual(0f, playerGameObject.transform.position.z);
        }

        [UnityTest]
        public IEnumerator should_only_move_left_when_horizontal_is_negative()
        {
            var playerInput = Substitute.For<IPlayerInput>();
            playerInput.Horizontal.Returns(-1f);
            var playerGameObject = PlayerMono_Mock.Dummy(playerInput);
            
            yield return new WaitForSeconds(1f);

            Assert.Greater(0f, playerGameObject.transform.position.x);
            Assert.AreEqual(0f, playerGameObject.transform.position.y);
            Assert.AreEqual(0f, playerGameObject.transform.position.z);
        }
    }
}
