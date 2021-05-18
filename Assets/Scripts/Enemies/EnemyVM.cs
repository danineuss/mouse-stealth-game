using System;
using UnityEngine;

public interface IEnemyVM: IIdentifiable
{
    void PlaySound(Sound sound);
}

public class EnemyVM : IEnemyVM
{
    public Guid ID { get; private set; }

    private IPlayerDetector playerDetector;
    private AudioVM audioVM;
    private IEnemyIO enemyIO;
    private SoundEmitter soundEmitter;
    private IPlayerEvents playerEvents;
    private IEnemyEvents enemyEvents;

    public void PlaySound(Sound sound)
    {
        soundEmitter.PlaySound(sound);
    }

    public EnemyVM(
        IPlayerDetector playerDetector,
        AudioVM audioVM,
        IEnemyIO enemyIO,
        SoundEmitter soundEmitter,
        IPlayerEvents playerEvents,
        IEnemyEvents enemyEvents)
    {
        this.playerDetector = playerDetector;
        this.audioVM = audioVM;
        this.enemyIO = enemyIO;
        this.soundEmitter = soundEmitter;
        this.playerEvents = playerEvents;
        this.enemyEvents = enemyEvents;
        
        ID = Guid.NewGuid();
        this.enemyIO.SetEnemyID(ID);

        InitializeEvents();
    }

    void InitializeEvents()
    {
        enemyEvents.OnCursorEnterEnemy += OnCursorEnterEnemy;
        enemyEvents.OnCurserExitEnemy += OnCurserExitEnemy;
        enemyEvents.OnDetectorStateChanged += OnDetectorStateChanged;

        playerEvents.OnPlayerLocationSent += OnReceivePlayerLocation;
        playerEvents.OnPlayerLocationRemoved += OnRemovePlayerLocation;
        playerEvents.OnAbilityExecuted += OnPlayerAbilityExecuted;
        playerEvents.OnEnemyDistracted += OnEnemyDistracted;
    }

    void OnCursorEnterEnemy(Guid enemyID)
    {
        if (enemyID != this.ID)
            return;

        enemyIO.SetDisplayVisibility(true);
    }

    void OnCurserExitEnemy()
    {
        enemyIO.SetDisplayVisibility(false);
    }

    void OnDetectorStateChanged(Guid detectorID)
    {
        if (detectorID != playerDetector.ID)
            return;

        enemyIO.SetTextColor(playerDetector.DetectorStateEnum);
        audioVM.PlaySoundAtEnemy(this, playerDetector.DetectorStateEnum);

        if (playerDetector.DetectorStateEnum == DetectorStateEnum.Alarmed)
            enemyEvents.FailGame();
    }

    void OnReceivePlayerLocation(
        Guid enemyID, bool shouldDisplayText, Transform playerTransform = null)
    {
        if (enemyID != this.ID)
            return;

        enemyIO.SetTextFollowingPlayer(shouldDisplayText, playerTransform);
    }

    void OnRemovePlayerLocation(Guid enemyID)
    {
        if (enemyID != this.ID)
            return;

        enemyIO.SetTextFollowingPlayer(false);
    }

    void OnPlayerAbilityExecuted(IPlayerAbility ability)
    {
        enemyIO.UpdateCooldownForAbility(ability);
    }

    void OnEnemyDistracted(Guid targetID)
    {
        if (targetID != this.ID || 
            playerDetector.DetectorStateEnum != DetectorStateEnum.Idle)
            return;

        playerDetector.SetStateDistracted();
        enemyIO.SetTextColor(DetectorStateEnum.Distracted);
    }
}
