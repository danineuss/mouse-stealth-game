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
        private float floatingPointDelta = 0.015f;

        GameObject SetupRestictable(Vector3 size)
        {
            var restictableGameObject = new GameObject("Restictable");
            var collider = restictableGameObject.AddComponent<BoxCollider>();
            collider.size = size;
            collider.isTrigger = true;
            restictableGameObject.AddComponent<PlayerRoamingArea>();
            return restictableGameObject;
        }

        [UnityTest]
        public IEnumerator should_only_move_forward_when_vertical_is_positive()
        {
            var playerInput = Substitute.For<IPlayerInput>();
            playerInput.Vertical.Returns(1f);
            var playerGameObject = PlayerMono_Mock.Dummy(playerInput);
            
            yield return new WaitForSeconds(0.2f);

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
            
            yield return new WaitForSeconds(0.2f);

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
            
            yield return new WaitForSeconds(0.2f);

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
            
            yield return new WaitForSeconds(0.2f);

            Assert.Greater(0f, playerGameObject.transform.position.x);
            Assert.AreEqual(0f, playerGameObject.transform.position.y);
            Assert.AreEqual(0f, playerGameObject.transform.position.z);
        }

        [UnityTest]
        public IEnumerator should_move_within_restraint_when_being_restricted()
        {
            var playerInput = Substitute.For<IPlayerInput>();
            playerInput.Vertical.Returns(1f);
            var playerGameObject = PlayerMono_Mock.Dummy(playerInput);
            var playerMonoMock = playerGameObject.GetComponent<PlayerMono_Mock>();

            var restictableGameObject = SetupRestictable(new Vector3(2, 2, 2));
            playerMonoMock.OnTriggerEnter(restictableGameObject.GetComponent<Collider>());

            yield return new WaitForSeconds(1.2f);

            Assert.Greater(playerGameObject.transform.position.z, 0f);
            Assert.AreEqual(1f, playerGameObject.transform.position.z, floatingPointDelta);
        }

        [UnityTest]
        public IEnumerator should_move_within_second_restraint_when_updated()
        {
            var playerInput = Substitute.For<IPlayerInput>();
            playerInput.Vertical.Returns(1f);
            var playerGameObject = PlayerMono_Mock.Dummy(playerInput);
            var playerMonoMock = playerGameObject.GetComponent<PlayerMono_Mock>();

            var restictableGameObject = SetupRestictable(new Vector3(2, 2, 2));
            playerMonoMock.OnTriggerEnter(restictableGameObject.GetComponent<Collider>());

            yield return null;

            var secondRestictableObject = SetupRestictable(new Vector3(1, 1, 1));
            playerMonoMock.OnTriggerEnter(secondRestictableObject.GetComponent<Collider>());

            yield return new WaitForSeconds(0.7f);

            Assert.Greater(playerGameObject.transform.position.z, 0f);
            Assert.AreEqual(0.5f, playerGameObject.transform.position.z, floatingPointDelta);
        }
    }
}
