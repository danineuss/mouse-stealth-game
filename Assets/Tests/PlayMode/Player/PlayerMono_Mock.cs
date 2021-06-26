using NSubstitute;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMono_Mock : MonoBehaviour {
    public PlayerVM PlayerVM;

    public static GameObject Dummy(IPlayerInput playerInput)
    {
        return Dummy(playerInput, null);
    }

    public static GameObject Dummy(IPanicMeter panicMeter, IPlayerEvents playerEvents = null)
    {
        return Dummy(null, playerEvents, panicMeter);
    }

    public static GameObject Dummy(
        IPlayerInput playerInput = null, 
        IPlayerEvents playerEvents = null,
        IPanicMeter panicMeter = null,
        float? minMovementSpeed = null, 
        float? maxMovementSpeed = null,
        float? radiusStartSpeedDecrease = null,
        float? radiusStartFear = null,
        LayerMask? safeRoomObjectsLayerMask = null)
    {
        GameObject playerGameObject = new GameObject("Player");
        var playerMono = playerGameObject.AddComponent<PlayerMono_Mock>();
        var defaultPlayerInput = Substitute.For<IPlayerInput>();
        var cameraController = Substitute.For<IFirstPersonCameraController>();
        var playerAbilities = Substitute.For<IPlayerAbilities>();
        playerAbilities.Abilities.Returns(new Dictionary<KeyCode, IPlayerAbility>());
        var defaultPlayerEvents = Substitute.For<IPlayerEvents>();
        var enemyEvents = Substitute.For<IEnemyEvents>();
        var sceneEvents = Substitute.For<ISceneEvents>(); 

        var characterController = new FirstPersonCharacterController(
            playerGameObject.transform, 
            minMovementSpeed ?? 1f, 
            maxMovementSpeed ?? 1f,
            radiusStartSpeedDecrease ?? 1f,
            radiusStartFear ?? 2f,
            safeRoomObjectsLayerMask ?? new LayerMask(),
            playerInput ?? defaultPlayerInput, 
            playerEvents ?? defaultPlayerEvents
        );

        playerMono.PlayerVM = new PlayerVM(
            playerGameObject.transform, 
            cameraController, 
            characterController,
            playerInput ?? defaultPlayerInput,
            playerAbilities, 
            playerEvents ?? defaultPlayerEvents,
            enemyEvents,
            sceneEvents
        );
        
        return playerGameObject;
    }

    void Update()
    {
        PlayerVM.Update();
    }

    void LateUpdate()
    {
        PlayerVM.LateUpdate();
    }
}