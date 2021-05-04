using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVM : MonoBehaviour {
    [SerializeField] private EnemyEvents enemyEvents;
    [SerializeField] private PlayerEvents playerEvents;
    public PlayerEvents PlayerEvents {
        get => playerEvents; 
        private set => playerEvents = value;
    }

    private EnemyVM targetEnemy = null;
    private PlayerAbilities playerAbilities;
    private IPlayerInput playerInput;

    void Awake() {
        playerAbilities = GetComponentInChildren<PlayerAbilities>();
        playerInput = new PlayerInput();

    }
    
    void Start() {
        InitializeEvents();
    }

    void InitializeEvents() {
        enemyEvents.OnCursorEnterEnemy += OnCursorEnterEnemy;
        enemyEvents.OnCurserExitEnemy += OnCurserExitEnemy;
        playerEvents.OnAbilityLearned += OnAbilityLearned;
    }

    void Update() {
        CheckPlayerInput();
    }

    void CheckPlayerInput() {
        foreach (var keyCode in playerAbilities.RelevantKeyPresses) {
            if (playerInput.GetKeyDown(keyCode)) 
                playerAbilities.ExecuteAbility(playerAbilities.Abilities[keyCode], targetEnemy);
        }
    }

    void OnCursorEnterEnemy(EnemyVM enemyVM) {
        targetEnemy = enemyVM;
        if (playerAbilities.RelevantAbilities.Count > 0) {
            playerEvents.SendPlayerLocation(enemyVM, true, transform);
        } else {
            playerEvents.SendPlayerLocation(enemyVM, false, null);
        }
    }

    void OnCurserExitEnemy() {
        playerEvents.RemovePlayerLocation(targetEnemy);
        targetEnemy = null;
    }

    void OnAbilityLearned(IPlayerAbility ability) {
        playerAbilities.LearnAbility(ability);
    }
}
