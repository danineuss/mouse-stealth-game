using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVM : MonoBehaviour {
    [SerializeField] private PlayerEvents playerEvents;
    [SerializeField] private EnemyEvents enemyEvents;
    public EnemyEvents EnemyEvents { 
        get => enemyEvents;
        private set => enemyEvents = value;
    }
    
    private EnemyIO enemyIO;
    private PlayerDetector playerDetector;
    public bool GetDistracted() {
        if (playerDetector.DetectorState != DetectorState.Idle) {
            return false;
        }

        playerDetector.SetStateDistracted();
        enemyIO.SetTextColor(DetectorState.Distracted);
        return true;
    }
    
    void Awake() {
        enemyIO = GetComponentInChildren<EnemyIO>();
        playerDetector = GetComponentInChildren<PlayerDetector>();
    }

    void Start() {
        InitializeEvents();
    }

    void InitializeEvents() {
        enemyEvents.OnCursorEnterEnemy += OnCursorEnterEnemy;
        enemyEvents.OnCurserExitEnemy += OnCurserExitEnemy;
        enemyEvents.OnDetectorStateChanged += OnDetectorStateChanged;

        playerEvents.OnSendPlayerLocation += OnReceivePlayerLocation;
        playerEvents.OnRemovePlayerLocation += OnRemovePlayerLocation;
        playerEvents.OnAbilityExecuted += OnPlayerAbilityExecuted;
    }
    
    void OnCursorEnterEnemy(EnemyVM enemyVM) {
        if (enemyVM != this)
            return;
    
        enemyIO.SetDisplayVisibility(true);
    }

    void OnCurserExitEnemy() {
        enemyIO.SetDisplayVisibility(false);
    }

    void OnDetectorStateChanged(PlayerDetector playerDetector) {
        if (playerDetector != this.playerDetector)
            return;
        
        enemyIO.SetTextColor(playerDetector.DetectorState);
    }

    void OnReceivePlayerLocation(
        EnemyVM enemyVM, bool shouldDisplayText, Transform playerTransform = null 
    ) {
        if (enemyVM != this)
            return;

        enemyIO.SetTextFollowingPlayer(shouldDisplayText, playerTransform);
    }

    void OnRemovePlayerLocation(EnemyVM enemyVM) {
        if (enemyVM != this)
            return;

        enemyIO.SetTextFollowingPlayer(false);
    }

    void OnPlayerAbilityExecuted(IPlayerAbility ability) {
        enemyIO.UpdateCooldownForAbility(ability);
    }
}
