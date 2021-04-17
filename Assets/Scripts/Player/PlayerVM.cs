using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVM : MonoBehaviour
{
    [SerializeField] private EnemyEvents enemyEvents;
    [SerializeField] private PlayerEvents playerEvents;
    public PlayerEvents PlayerEvents {
        get => playerEvents; 
        private set => playerEvents = value;
    }

    private EnemyVM targetEnemy = null;
    private PlayerAbilities playerAbilities;

    void Awake() {
        playerAbilities = GetComponentInChildren<PlayerAbilities>();
    }
    
    void Start()
    {
        enemyEvents.OnCursorEnterEnemy += OnCursorEnterEnemy;
        enemyEvents.OnCurserExitEnemy += OnCurserExitEnemy;
    }

    void Update()
    {
        CheckPlayerInput();
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

    void CheckPlayerInput() {
        foreach (var keyCode in playerAbilities.RelevantKeyPresses) {
            if (Input.GetKeyDown(keyCode)) {
                playerAbilities.ExecuteAbility(playerAbilities.Abilities[keyCode], targetEnemy);
            }
        }
    }
}
