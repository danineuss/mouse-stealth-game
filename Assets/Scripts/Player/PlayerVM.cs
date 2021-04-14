using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVM : MonoBehaviour
{
    [SerializeField] private EnemyEvents enemyEvents;
    [SerializeField] private PlayerEvents playerEvents;
    private EnemyVM targetEnemy = null;
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
        playerEvents.SendPlayerLocation(transform);
    }

    void OnCurserExitEnemy() {
        targetEnemy = null;
        playerEvents.RemovePlayerLocation();
    }

    void CheckPlayerInput() {      
        if (targetEnemy == null) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            targetEnemy.GetDistracted();
        }
    }
}
