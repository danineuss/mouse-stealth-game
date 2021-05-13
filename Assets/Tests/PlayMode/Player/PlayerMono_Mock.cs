using NSubstitute;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMono_Mock : MonoBehaviour {
    public IEnemyEvents EnemyEvents;
    public IPlayerEvents PlayerEvents;
    public float MovementSpeed;
    public float RotationSpeed;
    public PlayerVM PlayerVM;

    public static GameObject Dummy(IPlayerInput playerInput)
    {
        GameObject playerGameObject = new GameObject("Player");
        var playerMono = playerGameObject.AddComponent<PlayerMono_Mock>();
        var cameraController = Substitute.For<IFirstPersonCameraController>();
        var playerAbilities = Substitute.For<IPlayerAbilities>();
        playerAbilities.Abilities.Returns(new Dictionary<KeyCode, IPlayerAbility>());
        var playerEvents = Substitute.For<IPlayerEvents>();
        var enemyEvents = Substitute.For<IEnemyEvents>();
        var sceneEvents = Substitute.For<ISceneEvents>(); 

        var characterController = new FirstPersonCharacterController(playerGameObject.transform, playerInput, 1f);
        playerMono.PlayerVM = new PlayerVM(
            playerGameObject.transform, 
            cameraController, 
            characterController,
            playerInput,
            playerAbilities, 
            playerEvents,
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

    public void OnTriggerEnter(Collider collider) 
    {
        PlayerVM.OnTriggerEnter(collider);
    }
}