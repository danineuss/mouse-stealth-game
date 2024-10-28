using System.Collections.Generic;
using Audio;
using Infrastructure;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerMono: MonoBehaviour 
    {
        [SerializeField] private EventsMono eventsMono;
        [FormerlySerializedAs("audioVM")] [SerializeField] private AudioViewModel audioViewModel;
        [SerializeField] private SoundEmitter soundEmitter;
        [SerializeField] private float minMovementSpeed;
        [SerializeField] private float maxMovementSpeed;
        [SerializeField] private float radiusStartSpeedDecrease;
        [SerializeField] private float radiusStartFear;
        [SerializeField] private float panicEscalationSpeed;
        [SerializeField] private float panicDeescalationSpeed;
        [SerializeField] private LayerMask safeRoomObjectsLayerMask;
        [SerializeField] private float rotationSpeed;
        public PlayerViewModel PlayerViewModel => playerViewModel;

        private PlayerViewModel playerViewModel;

        void Awake() 
        {
            var playerAbilities = new PlayerAbilities(
                eventsMono.PlayerEvents, new Dictionary<KeyCode, IPlayerAbility>()
            );
            var playerInput = new PlayerInput(eventsMono.PlayerEvents);

            var cameraTransform = GetComponentInChildren<Camera>().gameObject.transform;
            var cameraController = new FirstPersonCameraController(
                transform, cameraTransform, playerInput, rotationSpeed
            );
        
            var characterController = new FirstPersonCharacterController(
                transform, 
                minMovementSpeed, 
                maxMovementSpeed,
                radiusStartSpeedDecrease,
                radiusStartFear,
                safeRoomObjectsLayerMask,
                playerInput,
                eventsMono.PlayerEvents
            );

            var panicMeter = 
                new PanicMeter(panicEscalationSpeed, panicDeescalationSpeed, eventsMono.PlayerEvents);
            var panicNoiseEmitter = new PanicNoiseEmitter(eventsMono.PlayerEvents, soundEmitter, audioViewModel);

            playerViewModel = new PlayerViewModel(
                gameObject.transform, 
                cameraController, 
                characterController, 
                playerInput, 
                playerAbilities, 
                panicMeter,
                panicNoiseEmitter,
                eventsMono.PlayerEvents, 
                eventsMono.EnemyEvents,
                eventsMono.SceneEvents
            );
        }

        void Update() 
        {
            playerViewModel.Update();
        }

        void LateUpdate() 
        {
            playerViewModel.LateUpdate();
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0);
            Gizmos.DrawWireSphere(transform.position, radiusStartFear);

            Gizmos.color = new Color(0.9f, 0.4f, 0.4f);
            Gizmos.DrawWireSphere(transform.position, radiusStartSpeedDecrease);
        }
    }
}
