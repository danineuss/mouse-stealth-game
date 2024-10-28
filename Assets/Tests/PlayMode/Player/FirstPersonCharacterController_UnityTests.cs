using System.Collections;
using NSubstitute;
using NUnit.Framework;
using Player;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class FirstPersonCharacterController_UnityTests
    {
        private float minimumSpeed = 0.5f;
        private float maximumSpeed = 2.0f;
        private readonly LayerMask safeRoomObjectsLayer = 8;
        private readonly float FloatingPointDelta = 0.005f;

        GameObject SetupSafeRoomObject(Vector3 size, Vector3 position, LayerMask? layerMask = null)
        {
            var safeRoomObject = new GameObject("Safe Room Object");
            safeRoomObject.transform.position = position;
            safeRoomObject.layer = layerMask ?? safeRoomObjectsLayer;
            var collider = safeRoomObject.AddComponent<BoxCollider>();
            collider.size = size;
            return safeRoomObject;
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

            GameObject.Destroy(playerGameObject);
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
            
            GameObject.Destroy(playerGameObject);
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
            
            GameObject.Destroy(playerGameObject);
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
            
            GameObject.Destroy(playerGameObject);
        }

        [UnityTest]
        public IEnumerator should_move_at_minimum_speed_without_any_protective_objects_nearby()
        {
            var playerInput = Substitute.For<IPlayerInput>();
            playerInput.Vertical.Returns(1f);
            var playerGameObject = PlayerMono_Mock.Dummy(playerInput, null, null, minimumSpeed, maximumSpeed);
            float startTime = Time.time;

            yield return new WaitForSeconds(0.2f);

            float expectedPositionZ = minimumSpeed * (Time.time - startTime);
            float notExpectedPositionZ = maximumSpeed * (Time.time - startTime);
            Assert.LessOrEqual(
                Mathf.Abs(playerGameObject.transform.position.z - expectedPositionZ), FloatingPointDelta);
            Assert.Greater(
                Mathf.Abs(playerGameObject.transform.position.z - notExpectedPositionZ), FloatingPointDelta);
            
            GameObject.Destroy(playerGameObject);
        }

        [UnityTest]
        public IEnumerator should_move_at_maximum_speed_with_safe_object_very_close_to_character()
        {
            var playerInput = Substitute.For<IPlayerInput>();
            playerInput.Vertical.Returns(1f);
            var playerGameObject = PlayerMono_Mock.Dummy(
                playerInput, null, null, minimumSpeed, maximumSpeed, 0.5f, 2f, 1<<safeRoomObjectsLayer);
            var safeObject = SetupSafeRoomObject(
                new Vector3(1f, 1f, 10f),
                new Vector3(0.6f, 0f, 0f)
            );
            float startTime = Time.time;

            yield return new WaitForSeconds(0.2f);

            float expectedPositionZ = maximumSpeed * (Time.time - startTime);
            float notExpectedPositionZ = minimumSpeed * (Time.time - startTime);
            Assert.LessOrEqual(
                Mathf.Abs(playerGameObject.transform.position.z - expectedPositionZ), FloatingPointDelta);
            Assert.Greater(
                Mathf.Abs(playerGameObject.transform.position.z - notExpectedPositionZ), FloatingPointDelta);
            
            GameObject.Destroy(playerGameObject);
            GameObject.Destroy(safeObject);
        }

        [UnityTest]
        public IEnumerator should_move_at_minimum_speed_with_safe_object_very_far_from_character()
        {
            var playerInput = Substitute.For<IPlayerInput>();
            playerInput.Vertical.Returns(1f);
            var playerGameObject = PlayerMono_Mock.Dummy(
                playerInput, null, null, minimumSpeed, maximumSpeed, 0.5f, 2f, 1<<safeRoomObjectsLayer);
            var safeObject = SetupSafeRoomObject(
                new Vector3(1f, 1f, 10f),
                new Vector3(10f, 0f, 0f)
            );
            float startTime = Time.time;

            yield return new WaitForSeconds(0.2f);

            float expectedPositionZ = minimumSpeed * (Time.time - startTime);
            float notExpectedPositionZ = maximumSpeed * (Time.time - startTime);
            Assert.LessOrEqual(
                Mathf.Abs(playerGameObject.transform.position.z - expectedPositionZ), FloatingPointDelta);
            Assert.Greater(
                Mathf.Abs(playerGameObject.transform.position.z - notExpectedPositionZ), FloatingPointDelta);
            
            GameObject.Destroy(playerGameObject);
            GameObject.Destroy(safeObject);
        }

        [UnityTest]
        public IEnumerator should_not_send_any_events_without_safe_objects_nearby()
        {
            var playerEvents = Substitute.For<IPlayerEvents>();
            var playerInput = Substitute.For<IPlayerInput>();
            var playerGameObject = PlayerMono_Mock.Dummy(playerInput, playerEvents);
            float startTime = Time.time;

            yield return null;

            playerEvents.DidNotReceiveWithAnyArgs().ChangeCharacterInCover(default);
            
            GameObject.Destroy(playerGameObject);
        }

        [UnityTest]
        public IEnumerator should_send_character_in_cover_with_safe_object_nearby()
        {
            var playerEvents = Substitute.For<IPlayerEvents>();
            var playerInput = Substitute.For<IPlayerInput>();
            var playerGameObject = PlayerMono_Mock.Dummy(
                playerInput, playerEvents, null, null, null, null, null, 1<<safeRoomObjectsLayer);
            var safeObject = SetupSafeRoomObject(
                new Vector3(1f, 1f, 10f),
                new Vector3(0.6f, 0f, 0f)
            );
            float startTime = Time.time;

            yield return null;

            playerEvents.ReceivedWithAnyArgs(1).ChangeCharacterInCover(default);
            playerEvents.Received(1).ChangeCharacterInCover(true);

            GameObject.Destroy(playerGameObject);
            GameObject.Destroy(safeObject);
        }

        [UnityTest]
        public IEnumerator should_send_a_second_call_for_not_in_cover_when_moving_away_from_safe_object()
        {
            var playerEvents = Substitute.For<IPlayerEvents>();
            var playerInput = Substitute.For<IPlayerInput>();
            playerInput.Horizontal.Returns(-1f);
            var playerGameObject = PlayerMono_Mock.Dummy(
                playerInput, playerEvents, null, null, null, 0.5f, 0.6f, 1<<safeRoomObjectsLayer);
            var safeObject = SetupSafeRoomObject(
                new Vector3(1f, 1f, 10f),
                new Vector3(1f, 0f, 0f)
            );
            float startTime = Time.time;

            yield return new WaitForSeconds(0.2f);

            playerEvents.ReceivedWithAnyArgs(2).ChangeCharacterInCover(default);
            playerEvents.Received(1).ChangeCharacterInCover(true);
            playerEvents.Received(1).ChangeCharacterInCover(false);

            GameObject.Destroy(playerGameObject);
            GameObject.Destroy(safeObject);
        }
    }
}
