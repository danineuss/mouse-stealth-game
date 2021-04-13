using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVM : MonoBehaviour
{
    [SerializeField] private EnemyEvents enemyEvents;
    public EnemyEvents EnemyEvents { 
        get {
            return enemyEvents;
        }
        private set {
            enemyEvents = value;
        }
    }
    
    private EnemyIO enemyIO;
    private PlayerDetector playerDetector;
    
    void Start()
    {
        enemyIO = GetComponentInChildren<EnemyIO>();
        playerDetector = GetComponentInChildren<PlayerDetector>();

        EnemyEvents.OnCursorEnterEnemy += OnCursorEnterEnemy;
        EnemyEvents.OnCurserExitEnemy += OnCurserExitEnemy;
    }

    public void GetDistracted() {
        playerDetector.SetStateDistracted();
    }
    
    private void OnCursorEnterEnemy(EnemyVM enemyVM) {
        enemyIO.ToggleDisplayVisibility(true);
    }

    private void OnCurserExitEnemy() {
        enemyIO.ToggleDisplayVisibility(false);
    }
}
