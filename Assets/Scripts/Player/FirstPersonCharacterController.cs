using System.Linq;
using UnityEngine;

namespace Player
{
    public interface IFirstPersonCharacterController
    {
        void UpdateCharacterPosition();
    }

    public class FirstPersonCharacterController : IFirstPersonCharacterController
    {
        private readonly Transform characterTransform;
        private float movementSpeed;
        private bool CharacterInCover {
            get => characterInCover;
            set {
                if (characterInCover == value)
                    return;
            
                characterInCover = value;
                playerEvents.ChangeCharacterInCover(value);
            }
        }
        private bool characterInCover;

        private readonly float MinMovementSpeed;
        private readonly float MaxMovementSpeed;
        private readonly float RadiusStartSpeedDecrease;
        private readonly float RadiusStartFear;
        private readonly LayerMask safeRoomObjectsLayerMask;
        private readonly IPlayerInput playerInput;
        private readonly IPlayerEvents playerEvents;

        public FirstPersonCharacterController(
            Transform characterTransform, 
            float minMovementSpeed, 
            float maxMovementSpeed, 
            float radiusStartSpeedDecrease,
            float radiusStartFear, 
            LayerMask safeRoomObjectsLayerMask,
            IPlayerInput playerInput, 
            IPlayerEvents playerEvents) 
        {
            this.characterTransform = characterTransform;
            MinMovementSpeed = minMovementSpeed;
            MaxMovementSpeed = maxMovementSpeed;
            RadiusStartSpeedDecrease = radiusStartSpeedDecrease;
            RadiusStartFear = radiusStartFear;
            this.safeRoomObjectsLayerMask = safeRoomObjectsLayerMask;
            this.playerInput = playerInput;
            this.playerEvents = playerEvents;
        
            movementSpeed = minMovementSpeed;
        }

        public void UpdateCharacterPosition()
        {
            var safeObjects = FindCoveringSafeObjects();
            movementSpeed = UpdateMovementSpeed(safeObjects);
            TranslateCharacter();
        }

        private Collider[] FindCoveringSafeObjects()
        {
            var safeObjects =
                Physics.OverlapSphere(characterTransform.position, RadiusStartFear, safeRoomObjectsLayerMask);
            CharacterInCover = safeObjects.Count() == 0 ? false : true;
            return safeObjects;
        }

        private float UpdateMovementSpeed(Collider[] safeObjects)
        {        
            if (safeObjects.Count() == 0)
                return MinMovementSpeed;

            var shortestDistance = safeObjects
                .Select(x => x.ClosestPointOnBounds(characterTransform.position))
                .Min(x => Vector3.Distance(x, characterTransform.position));

            if (shortestDistance <= RadiusStartSpeedDecrease)
                return MaxMovementSpeed;

            return LerpMovementSpeed(shortestDistance);
        }

        private float LerpMovementSpeed(float shortestDistance)
        {
            var lerpFactorBetweenRadii =
                (shortestDistance - RadiusStartSpeedDecrease) / (RadiusStartFear - RadiusStartSpeedDecrease);
            return MaxMovementSpeed - (MaxMovementSpeed - MinMovementSpeed) * lerpFactorBetweenRadii;
        }

        private void TranslateCharacter()
        {
            var input = new Vector3(playerInput.Horizontal, 0f, playerInput.Vertical).normalized;
            Vector3 playerMovement = input * movementSpeed * Time.deltaTime;
            characterTransform.Translate(playerMovement, Space.Self);
        }
    }
}