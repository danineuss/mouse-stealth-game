using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVM : MonoBehaviour {
    [SerializeField] private PlayerEventsMono playerEventsMono;
    [SerializeField] private EnemyEventsMono enemyEventsMono;
    [SerializeField] private AudioVM audioVM;
    public EnemyEvents EnemyEvents => enemyEventsMono.EnemyEvents;

    private EnemyIO enemyIO;
    private PlayerDetector playerDetector;
    private SoundEmitter soundEmitter;

    public bool GetDistracted() {
        if (playerDetector.DetectorState != DetectorState.Idle) {
            return false;
        }

        playerDetector.SetStateDistracted();
        enemyIO.SetTextColor(DetectorState.Distracted);
        return true;
    }

    public void PlaySound(Sound sound) {
        soundEmitter.PlaySound(sound);
    }
    
    void Awake() {
        enemyIO = GetComponentInChildren<EnemyIO>();
        playerDetector = GetComponentInChildren<PlayerDetector>();
        soundEmitter = GetComponentInChildren<SoundEmitter>();
    }

    void Start() {
        InitializeEvents();
    }

    void InitializeEvents() {
        enemyEventsMono.EnemyEvents.OnCursorEnterEnemy += OnCursorEnterEnemy;
        enemyEventsMono.EnemyEvents.OnCurserExitEnemy += OnCurserExitEnemy;
        enemyEventsMono.EnemyEvents.OnDetectorStateChanged += OnDetectorStateChanged;

        playerEventsMono.PlayerEvents.OnSendPlayerLocation += OnReceivePlayerLocation;
        playerEventsMono.PlayerEvents.OnRemovePlayerLocation += OnRemovePlayerLocation;
        playerEventsMono.PlayerEvents.OnAbilityExecuted += OnPlayerAbilityExecuted;
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
        audioVM.PlaySoundAtEnemy(this, playerDetector.DetectorState);
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
