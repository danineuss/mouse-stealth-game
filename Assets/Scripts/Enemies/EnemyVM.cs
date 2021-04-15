using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVM : MonoBehaviour
{
    [SerializeField] private PlayerEvents playerEvents;
    [SerializeField] private EnemyEvents enemyEvents;
    public EnemyEvents EnemyEvents { 
        get => enemyEvents;
        private set => enemyEvents = value;
    }
    
    private EnemyIO enemyIO;
    private PlayerDetector playerDetector;
    
    void Start()
    {
        enemyIO = GetComponentInChildren<EnemyIO>();
        playerDetector = GetComponentInChildren<PlayerDetector>();

        enemyEvents.OnCursorEnterEnemy += OnCursorEnterEnemy;
        enemyEvents.OnCurserExitEnemy += OnCurserExitEnemy;
        enemyEvents.OnDetectorChangedState += OnDetectorChangeState;

        playerEvents.OnSendPlayerLocation += OnReceivePlayerLocation;
        playerEvents.OnRemovePlayerLocation += OnRemovePlayerLocation;
    }

    public void GetDistracted() {
        playerDetector.SetStateDistracted();
        enemyIO.SetInteractible(DetectorState.Distracted);
    }
    
    void OnCursorEnterEnemy(EnemyVM enemyVM) {
        enemyIO.SetDisplayVisibility(true);
    }

    void OnCurserExitEnemy() {
        enemyIO.SetDisplayVisibility(false);
    }

    void OnDetectorChangeState(PlayerDetector playerDetector) {
        if (playerDetector != this.playerDetector) {
            return;
        }
        
        enemyIO.SetInteractible(playerDetector.DetectorState);
    }

    void OnReceivePlayerLocation(Transform playerTransform) {
        enemyIO.SetTextFollowingPlayer(playerTransform);
    }

    void OnRemovePlayerLocation() {
        enemyIO.SetTextFollowingPlayer(null);
    }
}
