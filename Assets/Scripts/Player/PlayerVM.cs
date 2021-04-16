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
    void Start()
    {
        playerAbilities = GetComponentInChildren<PlayerAbilities>();
        enemyEvents.OnCursorEnterEnemy += OnCursorEnterEnemy;
        enemyEvents.OnCurserExitEnemy += OnCurserExitEnemy;
    }

    void Update()
    {
        CheckPlayerInput();
    }

    void OnCursorEnterEnemy(EnemyVM enemyVM) {
        targetEnemy = enemyVM;
        playerEvents.SendPlayerLocation(transform, playerAbilities.RelevantAbilities);
    }

    void OnCurserExitEnemy() {
        targetEnemy = null;
        playerEvents.RemovePlayerLocation();
    }

    void CheckPlayerInput() {
        foreach (var keyCode in playerAbilities.RelevantKeyPresses) {
            if (Input.GetKeyDown(keyCode)) {
                playerAbilities.ExecuteAbility(playerAbilities.Abilities[keyCode], targetEnemy);
            }
        }
    }
}
