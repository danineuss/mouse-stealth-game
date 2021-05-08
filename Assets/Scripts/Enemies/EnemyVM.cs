using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyVM
{
    EnemyEvents EnemyEvents { get; }

    bool GetDistracted();
    void PlaySound(Sound sound);
}

public class EnemyVM : MonoBehaviour, IEnemyVM
{
    [SerializeField] private EventsMono eventsMono;
    [SerializeField] private AudioVM audioVM;
    //TODO: maybe remove this public property
    public EnemyEvents EnemyEvents => eventsMono.EnemyEvents;

    private EnemyIO enemyIO;
    private PlayerDetector playerDetector;
    private SoundEmitter soundEmitter;

    public bool GetDistracted()
    {
        if (playerDetector.DetectorState != DetectorState.Idle)
        {
            return false;
        }

        playerDetector.SetStateDistracted();
        enemyIO.SetTextColor(DetectorState.Distracted);
        return true;
    }

    public void PlaySound(Sound sound)
    {
        soundEmitter.PlaySound(sound);
    }

    void Awake()
    {
        enemyIO = GetComponentInChildren<EnemyIO>();
        playerDetector = GetComponentInChildren<PlayerDetector>();
        soundEmitter = GetComponentInChildren<SoundEmitter>();
    }

    void Start()
    {
        InitializeEvents();
    }

    void InitializeEvents()
    {
        eventsMono.EnemyEvents.OnCursorEnterEnemy += OnCursorEnterEnemy;
        eventsMono.EnemyEvents.OnCurserExitEnemy += OnCurserExitEnemy;
        eventsMono.EnemyEvents.OnDetectorStateChanged += OnDetectorStateChanged;

        eventsMono.PlayerEvents.OnSendPlayerLocation += OnReceivePlayerLocation;
        eventsMono.PlayerEvents.OnRemovePlayerLocation += OnRemovePlayerLocation;
        eventsMono.PlayerEvents.OnAbilityExecuted += OnPlayerAbilityExecuted;
    }

    void OnCursorEnterEnemy(EnemyVM enemyVM)
    {
        if (enemyVM != this)
            return;

        enemyIO.SetDisplayVisibility(true);
    }

    void OnCurserExitEnemy()
    {
        enemyIO.SetDisplayVisibility(false);
    }

    void OnDetectorStateChanged(PlayerDetector playerDetector)
    {
        if (playerDetector != this.playerDetector)
            return;

        enemyIO.SetTextColor(playerDetector.DetectorState);
        audioVM.PlaySoundAtEnemy(this, playerDetector.DetectorState);
    }

    void OnReceivePlayerLocation(
        EnemyVM enemyVM, bool shouldDisplayText, Transform playerTransform = null
    )
    {
        if (enemyVM != this)
            return;

        enemyIO.SetTextFollowingPlayer(shouldDisplayText, playerTransform);
    }

    void OnRemovePlayerLocation(EnemyVM enemyVM)
    {
        if (enemyVM != this)
            return;

        enemyIO.SetTextFollowingPlayer(false);
    }

    void OnPlayerAbilityExecuted(IPlayerAbility ability)
    {
        ability.Execute(this);
        enemyIO.UpdateCooldownForAbility(ability);
    }
}
