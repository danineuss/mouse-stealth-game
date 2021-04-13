using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVM : MonoBehaviour
{
    [SerializeField] private EnemyEvents enemyEvents;
    private EnemyVM targetEnemy = null;
    void Start()
    {
        enemyEvents.OnCursorEnterEnemy += OnCursorEnterEnemy;
        enemyEvents.OnCurserExitEnemy += OnCurserExitEnemy;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) {
            if (targetEnemy == null) {
                return;
            }
            targetEnemy.GetDistracted();
        }
    }

    void OnCursorEnterEnemy(EnemyVM enemyVM) {
        targetEnemy = enemyVM;
    }

    void OnCurserExitEnemy() {
        targetEnemy = null;
    }
}
