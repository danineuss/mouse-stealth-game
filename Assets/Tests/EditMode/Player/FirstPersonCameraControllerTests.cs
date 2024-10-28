using NSubstitute;
using NUnit.Framework;
using Player;
using UnityEngine;

namespace Tests 
{
    public class FirstPersonCameraControllerTests 
    {
        private FirstPersonCameraController cameraController;
        private Transform playerTransform;
        private Transform cameraTransform;
        private readonly float floatingPointDelta = 0.001f;

        private void SetupCameraController(IPlayerInput playerInput, float movementSpeed) 
        {
            playerTransform = new GameObject().transform;
            cameraTransform = new GameObject().transform;

            cameraController = new FirstPersonCameraController(
                playerTransform, cameraTransform, playerInput, movementSpeed
            );
        }

        [Test]
        public void should_rotate_player_in_Y_when_cursorX_is_moved() 
        {
            IPlayerInput playerInput = Substitute.For<IPlayerInput>();
            playerInput.CursorX.Returns(10);
            SetupCameraController(playerInput, 1f);

            cameraController.RotateForPlayerInput();

            Assert.AreEqual(10f, playerTransform.eulerAngles.y, floatingPointDelta);

            playerInput.CursorX.Returns(-20);
            SetupCameraController(playerInput, 1f);

            cameraController.RotateForPlayerInput();

            Assert.AreEqual(340f, playerTransform.eulerAngles.y, floatingPointDelta);
        }

        [Test]
        public void should_rotate_camera_in_XY_when_cursor_moved() 
        {
            IPlayerInput playerInput = Substitute.For<IPlayerInput>();
            playerInput.CursorX.Returns(10);
            playerInput.CursorY.Returns(30);
            SetupCameraController(playerInput, 1f);

            cameraController.RotateForPlayerInput();

            Assert.AreEqual(330f, cameraTransform.eulerAngles.x, floatingPointDelta);
            Assert.AreEqual(10f, cameraTransform.eulerAngles.y, floatingPointDelta);

            playerInput.CursorX.Returns(-20);
            playerInput.CursorY.Returns(-5);
            SetupCameraController(playerInput, 1f);

            cameraController.RotateForPlayerInput();

            Assert.AreEqual(5f, cameraTransform.eulerAngles.x, floatingPointDelta);
            Assert.AreEqual(340f, cameraTransform.eulerAngles.y, floatingPointDelta);
        }

        [Test]
        public void should_clamp_cursorY() 
        {
            IPlayerInput playerInput = Substitute.For<IPlayerInput>();
            playerInput.CursorY.Returns(-70);
            SetupCameraController(playerInput, 1f);

            cameraController.RotateForPlayerInput();

            Assert.AreEqual(60f, cameraTransform.eulerAngles.x, floatingPointDelta);

            playerInput.CursorY.Returns(45);
            SetupCameraController(playerInput, 1f);

            cameraController.RotateForPlayerInput();

            Assert.AreEqual(325, cameraTransform.eulerAngles.x, floatingPointDelta);
        }

        [Test]
        public void should_not_rotate_anything_when_speed_is_0() 
        {
            IPlayerInput playerInput = Substitute.For<IPlayerInput>();
            playerInput.CursorX.Returns(10);
            playerInput.CursorY.Returns(20);
            SetupCameraController(playerInput, 0f);

            cameraController.RotateForPlayerInput();

            Assert.AreEqual(0f, playerTransform.eulerAngles.y, floatingPointDelta);
            Assert.AreEqual(0f, cameraTransform.eulerAngles.x, floatingPointDelta);
            Assert.AreEqual(0f, cameraTransform.eulerAngles.y, floatingPointDelta);
        }

        [Test]
        public void should_not_rotate_anything_when_cursor_is_not_locked() 
        {
            IPlayerInput playerInput = Substitute.For<IPlayerInput>();
            playerInput.CursorX.Returns(10);
            playerInput.CursorY.Returns(20);
            SetupCameraController(playerInput, 1f);

            cameraController.LockCursor(false);
            cameraController.RotateForPlayerInput();

            Assert.AreEqual(0f, playerTransform.eulerAngles.y, floatingPointDelta);
            Assert.AreEqual(0f, cameraTransform.eulerAngles.x, floatingPointDelta);
            Assert.AreEqual(0f, cameraTransform.eulerAngles.y, floatingPointDelta);
        }        
    }
}
