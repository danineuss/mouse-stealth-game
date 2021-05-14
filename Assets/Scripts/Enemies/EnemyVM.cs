using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyVM: IIdentifiable
{
    EnemyEvents EnemyEvents { get; }

    bool GetDistracted();
    void PlaySound(Sound sound);
}

public class EnemyVM : MonoBehaviour, IEnemyVM
{
    [SerializeField] private EventsMono eventsMono = null;
    [SerializeField] private AudioVM audioVM = null;
    //TODO: maybe remove this public property
    public EnemyEvents EnemyEvents => eventsMono.EnemyEvents;

    public Guid ID => id;

    private EnemyIO enemyIO;
    private IPlayerDetector playerDetector;
    private SoundEmitter soundEmitter;
    private Guid id;

    public bool GetDistracted()
    {
        if (playerDetector.DetectorStateEnum != DetectorStateEnum.Idle)
        {
            return false;
        }

        playerDetector.SetStateDistracted();
        enemyIO.SetTextColor(DetectorStateEnum.Distracted);
        return true;
    }

    public void PlaySound(Sound sound)
    {
        soundEmitter.PlaySound(sound);
    }

    void Awake()
    {
        enemyIO = GetComponentInChildren<EnemyIO>();
        playerDetector = GetComponentInChildren<VisionConeMono>().PlayerDetector;
        soundEmitter = GetComponentInChildren<SoundEmitter>();
        id = new Guid();
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

    void OnCursorEnterEnemy(IEnemyVM enemyVM)
    {
        if (enemyVM.ID != this.ID)
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

        enemyIO.SetTextColor(playerDetector.DetectorStateEnum);
        audioVM.PlaySoundAtEnemy(this, playerDetector.DetectorStateEnum);

        if (playerDetector.DetectorStateEnum == DetectorStateEnum.Alarmed)
            EnemyEvents.FailGame();
    }

    void OnReceivePlayerLocation(
        IEnemyVM enemyVM, bool shouldDisplayText, Transform playerTransform = null
    )
    {
        if (enemyVM.ID != this.ID)
            return;

        enemyIO.SetTextFollowingPlayer(shouldDisplayText, playerTransform);
    }

    void OnRemovePlayerLocation(IEnemyVM enemyVM)
    {
        if (enemyVM.ID != this.ID)
            return;

        enemyIO.SetTextFollowingPlayer(false);
    }

    void OnPlayerAbilityExecuted(IPlayerAbility ability)
    {
        ability.Execute(this);
        enemyIO.UpdateCooldownForAbility(ability);
    }
}
